using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    public class WindsorSource : IHandlerSource
    {
        private readonly IKernel _kernel;
        

        public WindsorSource(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public IEnumerable<object> ResolveHandlersFor(Type messageType)
        {
            var handlerType = messageType.GetHandlerType();
            return _kernel.ResolveAll(handlerType).Cast<object>();
        }
    }
}