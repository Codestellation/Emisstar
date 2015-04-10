using System;
using System.Collections.Generic;
using Codestellation.Emisstar.Impl;
using LightInject;

namespace Emisstar.LightInject
{
    public class LightInjectSource : IHandlerSource
    {
        private readonly IServiceFactory _container;

        public LightInjectSource(IServiceFactory container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public IEnumerable<object> ResolveHandlersFor(Type messageType)
        {
            var handlerType = messageType.GetHandlerType();
            return _container.GetAllInstances(handlerType);
        }
    }
}