using System;
using System.Linq;
using NLog;

namespace Codestellation.Emisstar.Impl
{
    public class Publisher : IPublisher
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Publisher).FullName);
        private readonly IHandlerSource _handlerSource;
        private readonly IDispatcher[] _dispatchers;

        public Publisher(IHandlerSource handlerSource, IDispatcher[] dispatchers)
        {
            if (handlerSource == null)
            {
                throw new ArgumentNullException("handlerSource");
            }

            if (dispatchers == null)
            {
                throw new ArgumentNullException("dispatchers");
            }

            if (dispatchers.Any(x => ReferenceEquals(x, null)))
            {
                throw new ArgumentException("At least one of dispatches is null.", "dispatchers");
            }

            _handlerSource = handlerSource;
            _dispatchers = dispatchers;
        }

        public void Publish(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message", "Message should not be null");
            }

            var handlers = _handlerSource.ResolveHandlersFor(message.GetType());

            var invokedHandlers = 0;

            foreach (var handler in handlers)
            {
                var tuple = new MessageHandlerTuple(message, handler);

                bool invoked = false;
                for (int i = 0; i < _dispatchers.Length; i++)
                {
                    var dispatcher = _dispatchers[i];
                    invoked = dispatcher.TryInvoke(ref tuple);
                    if (invoked && Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Message {0} dispatched to {1} using {2}", message.GetType(), handler.GetType(), dispatcher.GetType());
                        break;
                    }
                }

                if (!invoked)
                {
                    throw new InvalidOperationException(string.Format("Dispatcher not found. Message '{0}'", message.GetType()));
                }
                invokedHandlers++;
            }

            if (invokedHandlers == 0 && Logger.IsWarnEnabled)
            {
                Logger.Warn("Handler not found. Message '{0}'", message.GetType());
            }
        }
    }
}