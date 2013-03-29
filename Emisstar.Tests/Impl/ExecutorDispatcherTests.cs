using Codestellation.DarkFlow.Execution;
using Codestellation.Emisstar.Impl;
using Codestellation.Emisstar.Testing;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class ExecutorDispatcherTests
    {
        [InvokeViaExecutor]
        public class MarkedMessage { }

        [Test]
        public void Can_invokes_return_true_if_message_marked()
        {
            var executor = new SynchronousExecutor();
            var dispatcher = new ExecutorDispatcher(executor);
            var message = new MarkedMessage();
            var handler = new TestHandler<MarkedMessage>();

            Assert.IsTrue(dispatcher.CanInvoke(message, handler));
        }

        [Test]
        public void Invokes_handler_using_executor()
        {
            var executor = new SynchronousExecutor();
            var dispatcher = new ExecutorDispatcher(executor);
            var message = new MarkedMessage();
            var handler = new TestHandler<MarkedMessage>();

            dispatcher.Invoke(message, handler);

            Assert.That(handler.WasCalledOnce);
        }
    }
}