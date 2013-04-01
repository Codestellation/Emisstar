using System;
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

        protected override void IntervalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            var task = new InvokeHandlerTask<TMessage>(message, handler);

            _executor.Execute(task);
        }
    }
}