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
            var tuple = new MessageHandlerTuple(message, handler);

            var result = simplePublishWay.TryInvoke(ref tuple);

            Assert.That(result, Is.True);
            Assert.That(handler.TestMessage, Is.SameAs(message));
        }
    }
}