﻿namespace Codestellation.Emisstar.Impl
{
    public class RuleBasedDispatcher : IDispatcher
    {
        private readonly IDispatchRule[] _rules;

        protected RuleBasedDispatcher(params IDispatchRule[] rules)
        {
            _rules = rules;
        }

        public virtual bool CanInvoke(ref MessageHandlerTuple tuple)
        {
            if (HandlerInvoker.IsHandler(ref tuple))
            {
                for (int i = 0; i < _rules.Length; i++)
                {
                    var result = _rules[i].CanDispatch(ref tuple);

                    if (result) return true;
                }
            }
            return false;
        }

        public virtual void Invoke(ref MessageHandlerTuple tuple)
        {
            HandlerInvoker.Invoke(ref tuple);
        }
    }
}