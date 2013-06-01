using System.Collections.Generic;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.Emisstar.DarkFlowIntegration;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    public class EmisstarFacility : AbstractFacility
    {
        private List<ComponentRegistration<IDispatcher>> _dispatcherRegistrations;

        public EmisstarFacility()
        {
            _dispatcherRegistrations = new List<ComponentRegistration<IDispatcher>>();
        }
        protected override void Init()
        {
            AddHandlersResolver();
            RegisterHandlerSources();
            RegisterDispatchers();
            RegisterPublisher();
            Clean();
        }

        private void Clean()
        {
            _dispatcherRegistrations = null;
        }

        private void AddHandlersResolver()
        {
            Kernel.Resolver.AddSubResolver(new PublisherSpecificCollectionResolver(Kernel));
        }

        protected virtual void RegisterHandlerSources()
        {
            Kernel.Register(
                Component
                    .For<CompositeHandlerSource, IHandlerSource>()
                    .LifestyleSingleton(),
                Component
                    .For<IHandlerSource>()
                    .ImplementedBy<WindsorSource>()
                    .LifestyleSingleton(),
                Component
                    .For<ISubscriber, IHandlerSource>()
                    .ImplementedBy<SimpleSubscriber>()
                    .LifestyleSingleton());
        }

        protected virtual void RegisterDispatchers()
        {
            _dispatcherRegistrations.Add(
                Component
                    .For<IDispatcher>()
                    .ImplementedBy<SimpleDispatcher>()
                    .LifestyleSingleton());
            
            Kernel.Register(_dispatcherRegistrations.ToArray());
        }

        protected virtual void RegisterPublisher()
        {
            Kernel.Register(
                Component
                    .For<IPublisher>()
                    .ImplementedBy<Publisher>()
                    .LifestyleSingleton());
        }

        public EmisstarFacility UseDarkFlowDispatcher()
        {
            var asmBuilder = new IntegrationAssemblyBuilder();

            _dispatcherRegistrations.Add(
                Component
                    .For<IDispatcher>()
                    .ImplementedBy(asmBuilder.GeneratedDispatcherType)
                    .LifestyleSingleton());

            return this;
        }
    }
}