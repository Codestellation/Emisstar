using System;
using System.Collections;
using System.Linq;
using NLog;

namespace Codestellation.Emisstar.Impl
{
    public class Publisher : IPublisher
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Publisher).FullName);
        private readonly PublisherSettings _settings;
        private readonly IHandlerSource _handlerSource;
        private readonly IDispatcher[] _dispatchers;

        public Publisher(IHandlerSource handlerSource, IDispatcher[] dispatchers)
            : this(PublisherSettings.Default, handlerSource, dispatchers)
        {

        }
        public Publisher(PublisherSettings settings, IHandlerSource handlerSource, IDispatcher[] dispatchers)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
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

            _settings = settings;
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

                var dispatchedTimes = 0;
                for (int i = 0; i < _dispatchers.Length; i++)
                {
                    var dispatcher = _dispatchers[i];
                    if (dispatcher.TryInvoke(ref tuple))
                    {
                        dispatchedTimes++;
                    }

                    if (dispatchedTimes > 0 && Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Message {0} dispatched to {1} using {2}", message.GetType(), handler.GetType(), dispatcher.GetType());
                        break;
                    }
                }

                if (dispatchedTimes == 0 && !_settings.IgnoreNoDispatcher)
                {
                    var exceptionMessage = string.Format("Dispatcher not found. Message '{0}'", message);
                    throw new InvalidOperationException(exceptionMessage);
                }
                if (dispatchedTimes > 1 && !_settings.IgnoreMultipleDispatcher)
                {
                    var exceptionMessage = string.Format("Message {0} delivered to multiple dispatchers", message);
                    throw new InvalidOperationException(exceptionMessage);
                }

                invokedHandlers++;
            }

            if (invokedHandlers == 0 && Logger.IsWarnEnabled)
            {
                Logger.Warn("Handler not found. Message '{0}'", message.GetType());
            }

            if (invokedHandlers == 0 && !_settings.IgnoreNoHandlers)
            {
                var exceptionMessage = string.Format("Could not find handler for message {0}", message);
                throw new InvalidOperationException(exceptionMessage);
            }
        }

        public void Publish(IEnumerable messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages", "Messages should not be null");
            }

            foreach (var message in messages)
            {
                Publish(message);
            }
        }
    }
}