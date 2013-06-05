using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public class SynchronizationContextDispatcher : RuleBasedDispatcher
    {
        private readonly SynchronizationContext _synchronizationContext;

        public SynchronizationContextDispatcher(SynchronizationContext synchronizationContext)
            : this(synchronizationContext, new Rule(tuple => true))
        {
        }

        public SynchronizationContextDispatcher(SynchronizationContext synchronizationContext, params IDispatchRule[] rules)
            : base(rules)
        {
            _synchronizationContext = synchronizationContext;
        }

        public override void Invoke(ref MessageHandlerTuple tuple)
        {
            _synchronizationContext.Post(PostMessage, tuple);
        }

        private void PostMessage(object state)
        {
            var tuple = (MessageHandlerTuple) state;
            base.Invoke(ref tuple);
        }
    }

    public class SynchronizationContextDispatcher<TSynchronizationContext> : SynchronizationContextDispatcher
        where TSynchronizationContext : SynchronizationContext, new()
    {
        public SynchronizationContextDispatcher()
            : base(new TSynchronizationContext(), new Rule(tuple => true))
        {
        }

        public SynchronizationContextDispatcher(params IDispatchRule[] rules)
            : base(new TSynchronizationContext(), rules)
        {
        }
    }
}