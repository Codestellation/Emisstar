using System;
using Codestellation.Emisstar.Impl;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class PublisherTests
    {
        [Test]
        public void Publish_will_send_the_message_to_listner()
        {
            var assignee = new SimpleAssignee();
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
            new Publisher(new SimpleAssignee(), new IDispatcher[] {null});
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_will_throw_if_handler_source_is_null()
        {
            new Publisher(null, new IDispatcher[] {new SimpleDispatcher()});
        }
    }
}