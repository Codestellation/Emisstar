using System;
using Codestellation.Emisstar.Impl;
using LightInject;

namespace Codestellation.Emisstar.LightInject
{
    public static class Builder
    {
        public static void RegisterPublisher(this IServiceRegistry self, Action<PublisherBuilder> build)
        {
            var builder = new PublisherBuilder(self);

            build(builder);

            builder.CompleteRegistration();
        }
    }

    public class PublisherBuilder
    {
        private readonly IServiceRegistry _container;
        private readonly PublisherSettings _settings;

        internal PublisherBuilder(IServiceRegistry container)
        {
            _container = container;
            _settings = new PublisherSettings();
        }

        internal void CompleteRegistration()
        {
            _container.RegisterInstance(typeof (IServiceFactory), _container);
            _container.RegisterInstance(_settings);
            _container.Register<IDispatchRule>(factory => new Rule(x => true), new PerContainerLifetime());
            _container.Register<IHandlerSource, LightInjectSource>(new PerContainerLifetime());
            _container.Register<IDispatcher, SimpleDispatcher>();
            _container.Register<IPublisher, Publisher>(new PerContainerLifetime());
        }
    }
}