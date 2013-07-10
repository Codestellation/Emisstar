namespace Codestellation.Emisstar.Impl
{
    public interface IDispatcher
    {
        /// <summary>
        /// Invoke a handler with a message.
        /// </summary>
        bool TryInvoke(ref MessageHandlerTuple tuple);
    }
}