namespace Codestellation.Emisstar.Impl
{
    public interface IDispatcher
    {
        /// <summary>
        /// Checks if a handler can dispatch a message
        /// </summary>
        /// <returns>Returns true if can dispatch the message to the handler. Returns false otherwise.</returns>
        bool CanInvoke(ref MessageHandlerTuple tuple);
        
        /// <summary>
        /// Invoke a handler with a message.
        /// </summary>
        void Invoke(ref MessageHandlerTuple tuple);
    }
}