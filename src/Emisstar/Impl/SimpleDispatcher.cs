namespace Codestellation.Emisstar.Impl
{
    public class SimpleDispatcher : RuleBasedDispatcher
    {
        public SimpleDispatcher() : base(new Rule(tuple => true))
        {
            
        }

        public SimpleDispatcher(params IDispatchRule[] rules) : base(rules)
        {
            
        }
    }
}