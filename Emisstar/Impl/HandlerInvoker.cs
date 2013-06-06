using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public delegate void Invoker(MessageHandlerTuple tuple);

    public static class HandlerInvoker
    {
        private static Dictionary<Type, Invoker> _invokersCache;
        private static Dictionary<Type, HashSet<Type>> _handlerMessageTypesCache;

        static HandlerInvoker()
        {
            _invokersCache = new Dictionary<Type, Invoker>();
            _handlerMessageTypesCache  = new Dictionary<Type, HashSet<Type>>();
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
            Dictionary<Type, Invoker> cacheBeforeCAS;
            Dictionary<Type, Invoker> cacheAfterCAS;
            
            do
            {
                cacheBeforeCAS = _invokersCache;
                Thread.MemoryBarrier();

                Invoker cached;

                //May be another thread filled cache already.
                if (cacheBeforeCAS.TryGetValue(messageType, out cached))
                {
                    return cached;
                }

                //Do not build expression twice during looping. 
                result = result ?? BuildExpression(messageType);

                //Adds new expression to cache preserving already cached expressions. 
                var newCache = new Dictionary<Type, Invoker>(cacheBeforeCAS) { { messageType, result } };

                cacheAfterCAS = Interlocked.CompareExchange(ref _invokersCache, newCache, cacheBeforeCAS);

            } while (cacheAfterCAS != cacheBeforeCAS);
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

        public static bool IsHandler(ref MessageHandlerTuple tuple)
        {
            HashSet<Type> messagesTypes = null;
            if (!_handlerMessageTypesCache.TryGetValue(tuple.Handler.GetType(), out messagesTypes))
            {
                messagesTypes = CacheMessagesTypes(tuple.Handler.GetType());
            }
            return messagesTypes.Contains(tuple.Message.GetType());
        }

        private static HashSet<Type> CacheMessagesTypes(Type handlerType)
        {
            HashSet<Type> result;
            Dictionary<Type, HashSet<Type>> cacheBeforeCAS;
            Dictionary<Type, HashSet<Type>> cacheAfterCAS;

            do
            {
                cacheBeforeCAS = _handlerMessageTypesCache;
                Thread.MemoryBarrier();
                
                if (cacheBeforeCAS.TryGetValue(handlerType, out result))
                {
                    return result;
                }
                result = HandlerMessages(handlerType);

                var newCache = new Dictionary<Type, HashSet<Type>>(cacheBeforeCAS) {{handlerType , result}};

                cacheAfterCAS = Interlocked.CompareExchange(ref _handlerMessageTypesCache, newCache, cacheBeforeCAS);

            } while (cacheBeforeCAS != cacheAfterCAS);
            
            return result;
        }

        private static HashSet<Type> HandlerMessages(Type self)
        {
            var findInterfaces = self.FindInterfaces((@interface, nomatter) => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof (IHandler<>), null);
            var messages = findInterfaces.Select(x => x.GetGenericArguments()[0]);
            return new HashSet<Type>(messages);
        }
    }
}