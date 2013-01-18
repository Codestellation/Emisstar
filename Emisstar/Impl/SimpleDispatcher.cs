namespace Codestellation.Emisstar.Impl
{
    public class SimpleDispatcher : RuleBasedDispatcher
    {
        public SimpleDispatcher() : base(new Rule((message, handler) => true))
        {
            
        }

        public SimpleDispatcher(params IDispatchRule[] rules) : base(rules)
        {
            
        }

        protected override void IntervalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            handler.Handle(message);
        }
    }
}