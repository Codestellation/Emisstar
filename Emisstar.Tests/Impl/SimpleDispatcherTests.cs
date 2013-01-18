using Codestellation.Emisstar.Impl;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class SimpleDispatcherTests
    {
        [Test]
        public void Publish_calls_the_method_on_subscriber()
        {
            var simplePublishWay = new SimpleDispatcher();
            var message = new TestMessage();
            var handler = new TestHandler();

            simplePublishWay.Invoke(message, handler);

            Assert.That(handler.TestMessage, Is.SameAs(message));
        }
    }
}