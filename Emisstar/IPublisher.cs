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
        /// <typeparam name="TMessage">Message type.</typeparam>
        /// <param name="message">An instance of message.</param>
        void Publish<TMessage>(TMessage message);
    }
}
