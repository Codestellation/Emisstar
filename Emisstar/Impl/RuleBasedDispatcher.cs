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
            IntervalInvoke(message, handler);
        }

        protected abstract void IntervalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler);
    }
}