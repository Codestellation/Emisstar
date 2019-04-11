using System;

namespace Codestellation.Emisstar.Impl
{
    public class Rule : IDispatchRule
    {
        private readonly Func<MessageHandlerTuple, bool> _rule;

        public Rule(Func<MessageHandlerTuple, bool> rule)
        {
            _rule = rule;
            
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
        }

        public bool CanDispatch(ref MessageHandlerTuple tuple)
        {
            return _rule(tuple);
        }
    }
}