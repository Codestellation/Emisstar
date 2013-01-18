using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public class SynchronizationContextDispatcher : RuleBasedSubDispatcher
    {
        private readonly SynchronizationContext _synchronizationContext;

        public SynchronizationContextDispatcher(SynchronizationContext synchronizationContext)
            : this(synchronizationContext, new Rule((message, handler) => true))
        {
        }

        public SynchronizationContextDispatcher(SynchronizationContext synchronizationContext, params IDispatchRule[] rules)
            : base(rules)
        {
            _synchronizationContext = synchronizationContext;
        }

        protected override void IntervalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            _synchronizationContext.Post(x => handler.Handle(message), null);
        }
    }

    public class SynchronizationContextDispatcher<TSynchornizationContext> : SynchronizationContextDispatcher
        where TSynchornizationContext : SynchronizationContext, new()
    {
        public SynchronizationContextDispatcher()
            : base(new TSynchornizationContext(), new Rule((message, handler) => true))
        {
        }

        public SynchronizationContextDispatcher(params IDispatchRule[] rules)
            : base(new TSynchornizationContext(), new Rule((message, handler) => true))
        {
        }
    }
}