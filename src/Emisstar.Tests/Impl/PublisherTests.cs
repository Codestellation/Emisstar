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
        public void Constructor_will_throw_if_dispatcher_is_null()
        {
            Assert.Throws<ArgumentException>(()=> new Publisher(new SimpleSubscriber(), new IDispatcher[] { null }));
        }
        
        [Test]
        public void Constructor_will_throw_if_handler_source_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new Publisher(null, new IDispatcher[] {new SimpleDispatcher()}));
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

        [Test]
        public void Should_throw_if_no_handlers_found()
        {
            var assignee = new SimpleSubscriber();
            var dispatcher = new SimpleDispatcher();
            var message = new TestMessage();
            var publisher = new Publisher(assignee, new[] { dispatcher});

            Should.Throw<InvalidOperationException>(() => publisher.Publish(message));
        }
        
        [Test]
        public void Should_throw_if_no_handlers_found_and_set_to_ignore_this()
        {
            var assignee = new SimpleSubscriber();
            var dispatcher = new SimpleDispatcher();
            var message = new TestMessage();
            var publisher = new Publisher(new PublisherSettings{IgnoreNoHandlers = true},  assignee, new[] { dispatcher});

            Should.NotThrow(() => publisher.Publish(message));
        }

        [Test]
        public void Should_throw_if_no_dispatcher_found()
        {
            var assignee = new SimpleSubscriber();
            var handler = new TestHandler();
            var message = new TestMessage();
            var publisher = new Publisher(assignee, new[] { new SimpleDispatcher(new Rule(x => false)) });

            assignee.Subscribe(handler);

            Should.Throw<InvalidOperationException>(() => publisher.Publish(message));
        }
        
        [Test]
        public void Should_no_throw_if_no_dispatcher_found_and_set_to_ignore()
        {
            var assignee = new SimpleSubscriber();
            var handler = new TestHandler();
            var message = new TestMessage();
            var publisher = new Publisher(new PublisherSettings{IgnoreNoDispatcher = true},assignee, new[] { new SimpleDispatcher(new Rule(x => false)) });

            assignee.Subscribe(handler);

            Should.NotThrow(() => publisher.Publish(message));
        }

        [Test]
        public void Should_throw_if_multiple_dispather_can_dispatch_a_message()
        {
            var assignee = new SimpleSubscriber();
            var dispatcher = new SimpleDispatcher();
            var handler = new TestHandler();
            var message = new TestMessage();
            var publisher = new Publisher(assignee, new[] { dispatcher, dispatcher });

            assignee.Subscribe(handler);

            Should.Throw<InvalidOperationException>(() => publisher.Publish(message));
        }
        
        [Test]
        public void Should_not_throw_if_multiple_dispather_can_dispatch_a_message_and_set_to_ignore()
        {
            var assignee = new SimpleSubscriber();
            var dispatcher = new SimpleDispatcher();
            var handler = new TestHandler();
            var message = new TestMessage();
            var publisher = new Publisher(new PublisherSettings{IgnoreMultipleDispatcher = true},  assignee, new[] { dispatcher, dispatcher });

            assignee.Subscribe(handler);

            Should.NotThrow(() => publisher.Publish(message));
        }
    }
}