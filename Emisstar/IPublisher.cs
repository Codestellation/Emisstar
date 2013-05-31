using System.Collections;

namespace Codestellation.Emisstar
{
    /// <summary>
    /// Publisher of events.
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="messages">Messages to deliver.</param>
        void Publish(params object[] messages);

        /// <summary>
        /// Publish messages to subscribed handlers.
        /// </summary>
        /// <param name="messages">Messages to deliver.</param>
        void Publish(IEnumerable messages);
    }
}
