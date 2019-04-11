namespace Codestellation.Emisstar
{
    /// <summary>
    /// Can subscribe and unsubscribe handlers.
    /// </summary>
    public interface ISubscriber
    {
        //TODO Consider return IDisposable to unsubscribe from events.
        /// <summary>
        /// Subscribes handler to events.
        /// </summary>
        /// <param name="handler">Object to subscribe.
        /// <remarks>If handler already subscribed implement <see cref="IHandler{TMessage}"/> it will be silently ignored.</remarks>
        /// </param>
        void Subscribe(object handler);

        /// <summary>
        /// Unsubscribes handler from events. 
        /// </summary>
        /// <param name="handler">Object to unsubscirbed
        /// <remarks>If handler already unsubscribed it will be silently ignored.</remarks>
        /// </param>
        void Unsubscribe(object handler);
    }
}
