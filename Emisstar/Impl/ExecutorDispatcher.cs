using System;
using System.Diagnostics;
using Codestellation.DarkFlow;

namespace Codestellation.Emisstar.Impl
{
    public class ExecutorDispatcher : RuleBasedDispatcher
    {
        private readonly IExecutor _executor;

        public ExecutorDispatcher(IExecutor executor): base(new InvokeUsingExecutorRule())
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }

            _executor = executor;
        }

        private class InvokeHandlerTask<TMessage> : ITask
        {
            private readonly TMessage _message;
            private readonly IHandler<TMessage> _handler;

            public InvokeHandlerTask(TMessage message, IHandler<TMessage> handler)
            {
                Debug.Assert(message != null);
                Debug.Assert(handler != null);
                _message = message;
                _handler = handler;
            }

            public void Execute()
            {
                _handler.Handle(_message);
            }
        }
        protected override void IntervalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            var task = new InvokeHandlerTask<TMessage>(message, handler);

            _executor.Execute(task);
        }
    }
}