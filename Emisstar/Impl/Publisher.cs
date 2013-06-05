using System;
using System.Collections;
using System.Linq;
using NLog;

namespace Codestellation.Emisstar.Impl
{
    public class Publisher : IPublisher
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Publisher).FullName);
        private readonly IHandlerSource _handlerSource;
        private readonly IDispatcher[] _dispatchers;

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
            var handlers = _handlerSource.ResolveHandlersFor(message.GetType());

            var invokedHandlers = 0;

            foreach (var handler in handlers)
            {
                //TODO: Need some kind of caching??
                var tuple = new MessageHandlerTuple(message, handler);

                var dispatcher = _dispatchers.FirstOrDefault(x => x.CanInvoke(ref tuple));

                if (dispatcher == null)
                {
                    throw  new InvalidOperationException(string.Format("Dispatcher not found. Message '{0}'", message.GetType()));
                }

                invokedHandlers++;
                dispatcher.Invoke(ref tuple);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Message {0} dispatched to {1} using {2}", message.GetType(), handler.GetType(), dispatcher.GetType());
                }
            }

            if (invokedHandlers == 0 && Logger.IsWarnEnabled)
            {
                Logger.Warn("Handler not found. Message '{0}'", message.GetType());
            }
        }
    }
}