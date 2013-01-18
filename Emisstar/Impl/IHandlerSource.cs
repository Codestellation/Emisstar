using System.Collections.Generic;

namespace Codestellation.Emisstar.Impl
{
    public interface IHandlerSource
    {
        IEnumerable<IHandler<TMessage>> ResolveHandlersFor<TMessage>();
    }
}
