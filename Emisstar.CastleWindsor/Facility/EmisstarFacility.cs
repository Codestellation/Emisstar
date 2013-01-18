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
            RegisterDispatcher();
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
                    .For<CompositeHandlerSource, IHandlerSource>(),
                Component
                    .For<IHandlerSource>()
                    .ImplementedBy<WindsorSource>(),
                Component
                    .For<IAssignee, IHandlerSource>()
                    .ImplementedBy<SimpleAssignee>());
        }

        protected virtual void RegisterDispatcher()
        {
            Kernel.Register(
                Component
                .For<IDispatcher>()
                .ImplementedBy<SimpleDispatcher>());
        }

        protected virtual void RegisterPublisher()
        {
            Kernel.Register(
                Component
                    .For<IPublisher>()
                    .ImplementedBy<Publisher>());
        }
    }
}