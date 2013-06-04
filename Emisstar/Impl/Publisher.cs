using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using NLog;

namespace Codestellation.Emisstar.Impl
{
    public class Publisher : IPublisher
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Publisher).FullName);
        private readonly IHandlerSource _handlerSource;
        private readonly IDispatcher[] _dispatchers;
        private readonly MethodInfo _method;
        private static Dictionary<Type, Action<Publisher, object>> _invokersCache;

        static Publisher()
        {
            _invokersCache = new Dictionary<Type, Action<Publisher, object>>();
        }

        public Publisher(IHandlerSource handlerSource, IDispatcher[] dispatchers)
        {
            if (handlerSource == null)
            {
                throw new ArgumentNullException("handlerSource");
            }
            
            if (dispatchers == null)
            {
                throw new ArgumentNullException("dispatchers");
            }

            if (dispatchers.Any(x => ReferenceEquals(x, null)))
            {
                throw new ArgumentException("At least one of dispatches is null.", "dispatchers");
            }

            _handlerSource = handlerSource;
            _dispatchers = dispatchers;
            _method = typeof (Publisher).GetMethod("InternalPublish", BindingFlags.Instance | BindingFlags.NonPublic);
        }


        public void Publish(params object[] messages)
        {
            Publish((IEnumerable) messages);
        }

        public void Publish(IEnumerable messages)
        {
            if (messages == null)
            {
                throw new ArgumentException("messages");
            }

            foreach (var message in messages)
            {
                if (message == null)
                {
                    throw new ArgumentException("Every message must be not null object or struct.");
                }

                Publish(message);
            }
        }

        private void Publish(object message)
        {
            Action<Publisher, object> invoker;
            var messageType = message.GetType();
            if (!_invokersCache.TryGetValue(messageType, out invoker))
            {
                invoker = BuildAndCacheInvoker(messageType);
            }
            invoker(this, message);
        }

        private Action<Publisher, object> BuildAndCacheInvoker(Type messageType)
        {
            Action<Publisher, object> result = null;
            Dictionary<Type, Action<Publisher, object>> original;
            Dictionary<Type, Action<Publisher, object>> cache;
            do
            {
                cache = _invokersCache;
                Thread.MemoryBarrier();
                
                Action<Publisher, object> cached;

                //May be another thread filled cache already.
                if (cache.TryGetValue(messageType, out cached))
                {
                    return cached;
                }

                //Do not build expression twice during looping. 
                result = result ?? BuildExpression(messageType);

                //Adds new expression to cache preserving already cached expressions. 
                var newCache = new Dictionary<Type, Action<Publisher, object>>(cache) { { messageType, result } };

                original = Interlocked.CompareExchange(ref _invokersCache, newCache, cache);

            } while (original != cache);
            return result;
        }

        private Action<Publisher, object> BuildExpression(Type messageType)
        {
            var publisherParameter = Expression.Parameter(typeof(Publisher));

            var messageParameter = Expression.Parameter(typeof (object));

            var castedArgument = Expression.Convert(messageParameter, messageType);
            
            var method = _method.MakeGenericMethod(messageType);

            var internalPublish = Expression.Call(publisherParameter, method, castedArgument);

            var lambda = Expression.Lambda<Action<Publisher, object>>(internalPublish, publisherParameter, messageParameter);
            
            return lambda.Compile();
        }
        
        //This method is used by expression calls;
        private void InternalPublish<TMessage>(TMessage message)
        {
            var handlers = _handlerSource.ResolveHandlersFor<TMessage>();

            var invokedHandlers = 0;

            foreach (var handler in handlers)
            {
                //TODO: Need some kind of caching??
                var dispatcher = _dispatchers.FirstOrDefault(x => x.CanInvoke(message, handler));

                if (dispatcher == null)
                {
                    throw  new InvalidOperationException(string.Format("Dispatcher not found. Message '{0}'", message.GetType()));
                }

                invokedHandlers++;
                dispatcher.Invoke(message, handler);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Message {0} dispatched to {1} using {2}", message.GetType(), handler.GetType(), dispatcher.GetType());
                }
            }

            if (invokedHandlers == 0 && Logger.IsWarnEnabled)
            {
                Logger.Warn("Handler not found. Message '{0}'}'", message.GetType());
            }
        }
    }
}