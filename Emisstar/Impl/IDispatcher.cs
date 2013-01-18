namespace Codestellation.Emisstar.Impl
{
    public interface IDispatcher
    {
        /// <summary>
        /// Checks if a handler can dispatch a message
        /// </summary>
        /// <typeparam name="TMessage">The type of message.</typeparam>
        /// <param name="message">Message to be delivered to handler.</param>
        /// <param name="handler">Handler that should process the message.</param>
        /// <returns>Returns true if can dispatch the message to the handler. Returns false otherwise.</returns>
        bool CanInvoke<TMessage>(TMessage message, IHandler<TMessage> handler);
        
        /// <summary>
        /// Invoke a handler with a message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message.</typeparam>
        /// <param name="message">Message to be delivered to handler.</param>
        /// <param name="handler">Handler that should process the message.</param>
        void Invoke<TMessage>(TMessage message, IHandler<TMessage> handler);
    }
}