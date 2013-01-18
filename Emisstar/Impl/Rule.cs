using System;

namespace Codestellation.Emisstar.Impl
{
    public class Rule : IDispatchRule
    {
        private readonly Func<object, object, bool> _rule;

        public Rule(Func<object, object, bool> rule)
        {
            _rule = rule;
            if(rule == null)
                throw new ArgumentNullException("rule");
        }

        public bool CanDispatch(object message, object handler)
        {
            return _rule(message, handler);
        }
    }
}