using System;
using System.Linq;

namespace Codestellation.Emisstar.Impl
{
    public class Publisher : IPublisher
    {
        private readonly IHandlerSource _handlerSource;
        private readonly IDispatcher[] _dispatchers;

        public Publisher(IHandlerSource handlerSource, IDispatcher[] dispatchers)
        {
            if(handlerSource == null) 
                throw new ArgumentNullException("handlerSource");
            if(dispatchers == null)
                throw new ArgumentNullException("dispatchers");
            if(dispatchers.Any(x => ReferenceEquals(x, null)))
                throw new ArgumentException("At least one of dispatches is null.", "dispatchers");

            _handlerSource = handlerSource;
            _dispatchers = dispatchers;
        }

        public virtual void Publish<TMessage>(TMessage message)
        {
            foreach (var handler in _handlerSource.ResolveHandlersFor<TMessage>())
            {
                //TODO: Need somekind of caching.
                var dispatcher = _dispatchers.FirstOrDefault(x => x.CanInvoke(message, handler));

                if (dispatcher == null)
                {
                    //TODO Log here something meaningful
                    return;
                }
                
                dispatcher.Invoke(message, handler);
            }
        }
    }
}