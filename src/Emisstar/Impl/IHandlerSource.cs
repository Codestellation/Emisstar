using System;
using System.Collections.Generic;

namespace Codestellation.Emisstar.Impl
{
    public interface IHandlerSource
    {
        IEnumerable<object> ResolveHandlersFor(Type messageType);
    }
}