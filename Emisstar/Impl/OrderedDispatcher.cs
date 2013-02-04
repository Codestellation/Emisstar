using System;
using System.Diagnostics;
using Codestellation.DarkFlow;

namespace Codestellation.Emisstar.Impl
{
    public class OrderedDispatcher : RuleBasedDispatcher
    {
        private readonly IExecutor _orderedExecutor;

        public OrderedDispatcher(IExecutor orderedExecutor): base(new OrderedRule())
        {
            if (orderedExecutor == null)
            {
                throw new ArgumentNullException("orderedExecutor");
            }

            _orderedExecutor = orderedExecutor;
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

            _orderedExecutor.Execute(task);
        }
    }
}