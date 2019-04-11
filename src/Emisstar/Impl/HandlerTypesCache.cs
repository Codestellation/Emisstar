using System;
using System.Collections.Generic;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public static class HandlerTypesCache
    {
        private static Dictionary<Type, Type> _cache = new Dictionary<Type, Type>();

        public static Type GetHandlerType(this Type messageType)
        {
            Type handlerType;
            if (!_cache.TryGetValue(messageType, out handlerType))
            {
                handlerType = BuildAndCache(messageType);
            }
            return handlerType;
        }

        //TODO: Move threading to submodule or Common project
        private static Type BuildAndCache(Type messageType)
        {
            Type result;
            Dictionary<Type, Type> cacheBeforeCAS;
            Dictionary<Type, Type> cacheAfterCAS;

            do
            {
                cacheBeforeCAS = _cache;
                Thread.MemoryBarrier();

                if (cacheBeforeCAS.TryGetValue(messageType, out result))
                {
                    return result;
                }
                result = typeof (IHandler<>).MakeGenericType(messageType);

                var newCache = new Dictionary<Type, Type>(cacheBeforeCAS) {{messageType, result}};

                cacheAfterCAS = Interlocked.CompareExchange(ref _cache, newCache, cacheBeforeCAS);
            } while (cacheBeforeCAS != cacheAfterCAS);

            return result;
        }
    }
}