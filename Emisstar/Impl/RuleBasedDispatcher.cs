using System.Linq;

namespace Codestellation.Emisstar.Impl
{
    public abstract class RuleBasedDispatcher : IDispatcher
    {
        private readonly IDispatchRule[] _rules;

        protected RuleBasedDispatcher(params IDispatchRule[] rules)
        {
            _rules = rules;
        }

        public bool CanInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            return _rules.Any(x => x.CanDispatch(message, handler));
        }

        public virtual void Invoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            InternalInvoke(message, handler);
        }

        protected abstract void InternalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler);
    }
}