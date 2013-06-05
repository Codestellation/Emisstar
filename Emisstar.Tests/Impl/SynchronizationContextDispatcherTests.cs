using System.Threading;
using Codestellation.Emisstar.Impl;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class SynchronizationContextDispatcherTests
    {
        [Test]
        public void Publish_will_call_listner()
        {
            AssertDispatchesCall(new SynchronizationContextDispatcher(new SynchronizationContext()));
        }

        [Test]
        public void Generic_publish_way_creates_synchronization_context()
        {
            AssertDispatchesCall(new SynchronizationContextDispatcher<SynchronizationContext>());
        }

        private static void AssertDispatchesCall(SynchronizationContextDispatcher dispatcher)
        {
            var handler = new TestHandler();
            var message = new TestMessage();

            var tuple = new MessageHandlerTuple(message, handler);

            dispatcher.Invoke(ref tuple);

            handler.Called.WaitOne(1000);

            Assert.That(handler.TestMessage, Is.SameAs(message));
        }
    }
}