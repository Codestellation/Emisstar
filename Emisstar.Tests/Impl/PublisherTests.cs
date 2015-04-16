using System;
using Codestellation.Emisstar.Impl;
using NUnit.Framework;
using Shouldly;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class PublisherTests
    {
        [Test]
        public void Publish_will_send_the_message_to_listner()
        {
            var assignee = new SimpleSubscriber();
            var dispatcher = new SimpleDispatcher();
            var handler = new TestHandler();
            var message = new TestMessage();
            var publisher = new Publisher(assignee, new[] {dispatcher});

            assignee.Subscribe(handler);

            publisher.Publish(message);

            Assert.That(handler.TestMessage, Is.SameAs(message));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_will_throw_if_dispatcher_is_null()
        {
            new Publisher(new SimpleSubscriber(), new IDispatcher[] {null});
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_will_throw_if_handler_source_is_null()
        {
            new Publisher(null, new IDispatcher[] {new SimpleDispatcher()});
        }

        [Test]
        public void Should_not_fail_if_onle_one_dispatcher_can_dispath_message()
        {
            var assignee = new SimpleSubscriber();
            var dispatcher = new SimpleDispatcher();
            var handler = new TestHandler();
            var message = new TestMessage();
            var publisher = new Publisher(assignee, new[] { dispatcher, new SimpleDispatcher(new Rule(x => false)) });

            assignee.Subscribe(handler);

            Should.NotThrow(() => publisher.Publish(message));
        }
    }
}