using System;
using System.Collections.Generic;
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
                throw new ArgumentNullException("kernel");

            _kernel = kernel;
        }

        public virtual IEnumerable<IHandler<TMessage>> ResolveHandlersFor<TMessage>()
        {
            return _kernel.ResolveAll<IHandler<TMessage>>();
        }
    }
}