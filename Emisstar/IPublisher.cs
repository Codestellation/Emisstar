namespace Codestellation.Emisstar
{
    /// <summary>
    /// Publisher of events.
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Publishes a message to subscribed handlers.
        /// </summary>
        /// <param name="message">An instance of message.</param>
        void Publish(object message);
    }
}
