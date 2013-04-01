using System.Diagnostics;
using Codestellation.DarkFlow;

namespace Codestellation.Emisstar.Impl
{
    public class InvokeHandlerTask<TMessage> : ITask
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
}