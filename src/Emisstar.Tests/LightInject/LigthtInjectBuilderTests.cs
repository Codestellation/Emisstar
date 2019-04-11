using System;
using Codestellation.Emisstar.LightInject;
using Codestellation.Emisstar.Testing;
using Codestellation.Emisstar.Tests.Windsor;
using LightInject;
using NUnit.Framework;
using Shouldly;

namespace Codestellation.Emisstar.Tests.LightInject
{
    [TestFixture]
    public class LigthtInjectBuilderTests
    {
        private ServiceContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new ServiceContainer();
        }

        [Test]
        public void Should_deliver_message_to_handler()
        {
            InitializeContainer(builder => { });
            _container.Register<IHandler<Message>, TestHandler<Message>>(new PerContainerLifetime());

            //act
            var publisher = _container.GetInstance<IPublisher>();
            publisher.Publish(new Message());

            //assert
            var handler = (TestHandler<Message>) _container.GetInstance<IHandler<Message>>();
            Assert.That(handler.WasCalledOnce);
        }

        [Test]
        public void Should_register_publisher_as_singleton()
        {
            InitializeContainer(builder => { });
            var publisher1 = _container.GetInstance<IPublisher>();
            var publisher2 = _container.GetInstance<IPublisher>();

            publisher1.ShouldBeSameAs(publisher2);
        }

        private void InitializeContainer(Action<PublisherBuilder> action)
        {
            _container.RegisterPublisher(action);
        }
    }
}