using System.Threading;
using NLog;

namespace Codestellation.Emisstar.Impl
{
    public class SynchronizationContextDispatcher : RuleBasedDispatcher
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof (SynchronizationContextDispatcher).FullName);

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
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Direct invoke '{0}' to '{1}'", tuple.Message, tuple.Handler);
                }

                base.Invoke(ref tuple);
            }
            else
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("SyncContext post '{0}' to '{1}'", tuple.Message, tuple.Handler);
                }
                _synchronizationContext.Post(PostMessage, tuple);
            }
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