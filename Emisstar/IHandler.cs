namespace Codestellation.Emisstar
{
    /// <summary>
    /// A handle of messages of specified type.
    /// </summary>
    /// <typeparam name="TMessage">Message type to handle.</typeparam>
    public interface IHandler<TMessage>
    {
        /// <summary>
        /// Handles message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        void Handle(TMessage message);
    }
}
