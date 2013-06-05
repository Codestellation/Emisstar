using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public delegate void Invoker(MessageHandlerTuple tuple);

    public class HandlerInvoker
    {
        private static Dictionary<Type, Invoker> _invokersCache;

        static HandlerInvoker()
        {
            _invokersCache = new Dictionary<Type, Invoker>();
        }

        public static void Invoke(ref MessageHandlerTuple tuple)
        {
            Invoker invoker;
            var messageType = tuple.Message.GetType();
            if (!_invokersCache.TryGetValue(messageType, out invoker))
            {
                invoker = BuildAndCacheInvoker(messageType);
            }
            invoker(tuple);
        }

        private static Invoker BuildAndCacheInvoker(Type messageType)
        {
            Invoker result = null;
            Dictionary<Type, Invoker> original;
            Dictionary<Type, Invoker> cache;
            do
            {
                cache = _invokersCache;
                Thread.MemoryBarrier();

                Invoker cached;

                //May be another thread filled cache already.
                if (cache.TryGetValue(messageType, out cached))
                {
                    return cached;
                }

                //Do not build expression twice during looping. 
                result = result ?? BuildExpression(messageType);

                //Adds new expression to cache preserving already cached expressions. 
                var newCache = new Dictionary<Type, Invoker>(cache) { { messageType, result } };

                original = Interlocked.CompareExchange(ref _invokersCache, newCache, cache);

            } while (original != cache);
            return result;
        }

        private static Invoker BuildExpression(Type messageType)
        {
            var closedHandlerType = typeof(IHandler<>).MakeGenericType(messageType);

            var tupleParameter = Expression.Parameter(typeof(MessageHandlerTuple), "tuple");

            //publisherParameter.IsByRef = true; how to make it by ref? 

            var handler = Expression.PropertyOrField(tupleParameter, "Handler");

            var castedHandler = Expression.Convert(handler, typeof (IHandler<>).MakeGenericType(messageType));

            var message = Expression.PropertyOrField(tupleParameter, "Message");

            var castedMessage = Expression.Convert(message, messageType);

            var handleMethodInfo = closedHandlerType.GetMethod("Handle");

            var handleCall = Expression.Call(castedHandler, handleMethodInfo, castedMessage);

            var lambda = Expression.Lambda<Invoker>(handleCall, tupleParameter);

            return lambda.Compile();
        }
    }
}