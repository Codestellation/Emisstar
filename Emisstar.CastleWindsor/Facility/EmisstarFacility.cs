using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    public class EmisstarFacility : AbstractFacility
    {
        protected override void Init()
        {
            AddHandlersResolver();
            RegisterHandlerSources();
            RegisterDispatchers();
            RegisterPublisher();
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
            Kernel.Register(
                Component
                    .For<IDispatcher>()
                    .ImplementedBy<ExecutorDispatcher>()
                    .LifestyleSingleton(),

                Component
                    .For<IDispatcher>()
                    .ImplementedBy<SimpleDispatcher>()
                    .LifestyleSingleton());
        }

        protected virtual void RegisterPublisher()
        {
            Kernel.Register(
                Component
                    .For<IPublisher>()
                    .ImplementedBy<Publisher>()
                    .LifestyleSingleton());
        }
    }
}