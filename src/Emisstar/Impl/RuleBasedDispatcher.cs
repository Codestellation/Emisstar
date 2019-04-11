namespace Codestellation.Emisstar.Impl
{
    public class RuleBasedDispatcher : IDispatcher
    {
        private readonly IDispatchRule[] _rules;

        protected RuleBasedDispatcher(params IDispatchRule[] rules)
        {
            _rules = rules;
        }

        public virtual bool TryInvoke(ref MessageHandlerTuple tuple)
        {
            if (HandlerInvoker.IsHandler(ref tuple))
            {
                for (int i = 0; i < _rules.Length; i++)
                {
                    if (_rules[i].CanDispatch(ref tuple))
                    {
                        Invoke(ref tuple);
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void Invoke(ref MessageHandlerTuple tuple)
        {
            HandlerInvoker.Invoke(ref tuple);
        }
    }
}