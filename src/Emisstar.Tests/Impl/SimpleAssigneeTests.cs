using System.Collections;
using System.Linq;
using Codestellation.Emisstar.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class SimpleAssigneeTests
    {
        [Test]
        public void Should_return_empty_collection_if_no_handlers()
        {
            var handlers = new SimpleSubscriber().ResolveHandlersFor(typeof(TestMessage));

            Assert.That(handlers, Is.Empty);
        }

        [Test]
        public void Subscribe_adds_it_to_the_source_so_it_can_be_resolved()
        {
            var assignee = new SimpleSubscriber();
            var handler = new TestHandler();
            assignee.Subscribe(handler);

            var handlers = assignee.ResolveHandlersFor(typeof(TestMessage));

            Assert.That(handlers, Has.Member(handler));
        }

        [Test]
        public void Subscribe_will_not_add_same_object_twice()
        {
            var assignee = new SimpleSubscriber();
            var handler = new TestHandler();
            assignee.Subscribe(handler);
            assignee.Subscribe(handler);

            IEnumerable handlers = assignee.ResolveHandlersFor(typeof(TestMessage));

            Assert.That(handlers, Has.Member(handler));
            //Assert.That(handlers.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Unsubscribe_removes_it_from_source_so_it_could_be_resolved()
        {
            var assignee = new SimpleSubscriber();
            var handler = new TestHandler();
            assignee.Subscribe(handler);
            assignee.Unsubscribe(handler);

            var handlers = assignee.ResolveHandlersFor(typeof(TestMessage));

            Assert.That(handlers, Has.No.Member(handler));
        }

        [Test]
        public void Returns_same_object_for_different_handlers()
        {
            var assignee = new SimpleSubscriber();
            var handler = new MultiHandler();

            assignee.Subscribe(handler);

            var messageHandlers = assignee.ResolveHandlersFor(typeof(TestMessage));
            var anotherMessageHandlers = assignee.ResolveHandlersFor(typeof(AnotherMessage));

            CollectionAssert.AreEqual(messageHandlers, anotherMessageHandlers);
        }

        [Test]
        public void Unsubscribe_if_not_subscribed_will_be_skipped_silently()
        {
            new SimpleSubscriber().Unsubscribe(new TestHandler());
        }
    }
}