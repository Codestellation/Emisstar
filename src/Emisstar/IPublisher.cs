using System.Collections;

namespace Codestellation.Emisstar
{
    /// <summary>
    /// Publisher of events.
    /// </summary>
    public interface IPublisher
    {
        void Publish(object message);

        void Publish(IEnumerable messages);
    }
}
