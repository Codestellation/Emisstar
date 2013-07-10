using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.MicroKernel;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    public class WindsorSource : IHandlerSource
    {
        private readonly IKernel _kernel;
        private Dictionary<Type, Type> _cache;

        public WindsorSource(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _cache = new Dictionary<Type, Type>();
            _kernel = kernel;
        }

        public IEnumerable<object> ResolveHandlersFor(Type messageType)
        {
            Type handlerType;
            if (!_cache.TryGetValue(messageType, out handlerType))
            {
                handlerType = BuildAndCache(messageType);
            }

            return _kernel.ResolveAll(handlerType).Cast<object>();
        }

        //TODO: Move it to submodule or Common project
        private Type BuildAndCache(Type messageType)
        {
            Type result;
            Dictionary<Type,Type> cacheBeforeCAS;
            Dictionary<Type, Type> cacheAfterCAS;

            do
            {
                cacheBeforeCAS = _cache;
                Thread.MemoryBarrier();

                if (cacheBeforeCAS.TryGetValue(messageType, out result))
                {
                    return result;
                }
                result = typeof(IHandler<>).MakeGenericType(messageType);

                var newCache = new Dictionary<Type, Type>(cacheBeforeCAS) { { messageType, result } };

                cacheAfterCAS = Interlocked.CompareExchange(ref _cache, newCache, cacheBeforeCAS);

            } while (cacheBeforeCAS != cacheAfterCAS);

            return result;
        }
    }
}