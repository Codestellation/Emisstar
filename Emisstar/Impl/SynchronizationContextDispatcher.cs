using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public class SynchronizationContextDispatcher : RuleBasedDispatcher
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

        protected override void InternalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
        {
            _synchronizationContext.Post(x => handler.Handle(message), null);
        }
    }

    public class SynchronizationContextDispatcher<TSynchronizationContext> : SynchronizationContextDispatcher
        where TSynchronizationContext : SynchronizationContext, new()
    {
        public SynchronizationContextDispatcher()
            : base(new TSynchronizationContext(), new Rule((message, handler) => true))
        {
        }

        public SynchronizationContextDispatcher(params IDispatchRule[] rules)
            : base(new TSynchronizationContext(), rules)
        {
        }
    }
}