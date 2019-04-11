using System.Linq;
using Codestellation.Emisstar.Impl;
using Codestellation.Emisstar.Testing;
using Codestellation.Emisstar.Tests.Impl;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Testing
{
    [TestFixture]
    public class TestPublisherTests
    {
        [Test]
        public void Creates_test_handler_and_adds_it_to_publisher()
        {
            var publisher = new TestPublisher();

            var handler = publisher.RegisterTestHandler<TestMessage>();

            Assert.That(handler, Is.InstanceOf<TestHandler<TestMessage>>());
            Assert.That(((IHandlerSource)publisher.GetAssignee()).ResolveHandlersFor(typeof(TestMessage)).First(), Is.SameAs(handler));
        }
    }
}