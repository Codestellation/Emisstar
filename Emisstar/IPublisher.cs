namespace Codestellation.Emisstar
{
    /// <summary>
    /// Publisher of events.
    /// </summary>
    public interface IPublisher
    {
        void Publish(object message);
    }
}
