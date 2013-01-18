using System.Collections.Generic;
using System.Linq;
using Codestellation.Emisstar.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class SimpleAssigneeTest
    {
        [Test]
        public void Should_return_empty_collection_if_no_handlers()
        {
            var handlers = new SimpleAssignee().ResolveHandlersFor<TestMessage>();

            Assert.That(handlers, Is.Empty);
        }

        [Test]
        public void Subscribe_adds_it_to_the_source_so_it_can_be_resolved()
        {
            var assignee = new SimpleAssignee();
            var handler = new TestHandler();
            assignee.Subscribe(handler);

            var handlers = assignee.ResolveHandlersFor<TestMessage>();

            Assert.That(handlers, Has.Member(handler));
        }

        [Test]
        public void Subscribe_will_not_add_same_object_twice()
        {
            var assignee = new SimpleAssignee();
            var handler = new TestHandler();
            assignee.Subscribe(handler);
            assignee.Subscribe(handler);

            var handlers = assignee.ResolveHandlersFor<TestMessage>();

            Assert.That(handlers, Has.Member(handler));
            Assert.That(handlers.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Unsubscribe_removes_it_from_source_so_it_could_be_resolved()
        {
            var assignee = new SimpleAssignee();
            var handler = new TestHandler();
            assignee.Subscribe(handler);
            assignee.Unsubscribe(handler);

            var handlers = new List<IHandler<TestMessage>>(assignee.ResolveHandlersFor<TestMessage>());

            Assert.That(handlers, Has.No.Member(handler));
        }

        [Test]
        public void Returns_same_object_for_different_handlers()
        {
            var assignee = new SimpleAssignee();
            var handler = new MultiHandler();

            assignee.Subscribe(handler);

            var messageHandlers = assignee.ResolveHandlersFor<TestMessage>();
            var anotherMessageHandlers = assignee.ResolveHandlersFor<AnotherMessage>();

            handler.Satisfy(h =>
                            messageHandlers.Contains(h) &&
                            anotherMessageHandlers.Contains(h));

        }

        [Test]
        public void Unsubscribe_if_not_subscribed_will_be_skipped_silently()
        {
            new SimpleAssignee().Unsubscribe(new TestHandler());
        }
    }
}