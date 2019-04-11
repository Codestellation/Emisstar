using System;
using System.Collections.Generic;
using System.Threading;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    public class EmisstarFacility : AbstractFacility
    {
        private List<IRegistration> _dispatcherRegistrations;

        public EmisstarFacility()
        {
            _dispatcherRegistrations = new List<IRegistration>();
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

        public EmisstarFacility UseSimpleDispatcher(params IDispatchRule[] rules)
        {
            return RegisterRuleBaseDispatcher<SimpleDispatcher>(rules);
        }

        public EmisstarFacility UseSynchronizationContextDispatcher<TSynchronizationContext>(params IDispatchRule[] rules)
            where TSynchronizationContext : SynchronizationContext, new()
        {
            return RegisterRuleBaseDispatcher<SynchronizationContextDispatcher<TSynchronizationContext>>(rules);
        }
        
        public EmisstarFacility UseSynchronizationContextDispatcher(Func<SynchronizationContext> contextFactory, params IDispatchRule[] rules)
        {
            var contextRegistration = Component
                .For<SynchronizationContext>()
                .UsingFactoryMethod(contextFactory);

            _dispatcherRegistrations.Add(contextRegistration);

            return RegisterRuleBaseDispatcher<SynchronizationContextDispatcher>(rules);
        }
         
        public EmisstarFacility RegisterRuleBaseDispatcher<TDispatcher>(params IDispatchRule[] rules)
            where TDispatcher : RuleBasedDispatcher
        {
            var componentRegistration =
                Component
                    .For<IDispatcher>()
                    .ImplementedBy<TDispatcher>()
                    .LifestyleSingleton();

            if (rules != null && rules.Length > 0)
            {
                componentRegistration
                    .DependsOn(Dependency.OnValue<IDispatchRule[]>(rules));
            }

            _dispatcherRegistrations.Add(componentRegistration);
            return this;
        }

        private void RegisterPublisher()
        {
            Kernel.Register(
                Component
                    .For<IPublisher>()
                    .ImplementedBy<Publisher>()
                    .LifestyleSingleton());
        }

        private void RegisterDispatchers()
        {
            if (_dispatcherRegistrations.Count == 0 && !Kernel.HasComponent(typeof(IDispatcher)))
            {
                UseSimpleDispatcher();
            }

            Kernel.Register(_dispatcherRegistrations.ToArray());
        }
    }
}