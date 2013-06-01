using System;
using System.Collections.Concurrent;
using System.Linq;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.DarkFlowIntegration
{
    public class InvokeUsingExecutorRule : IDispatchRule
    {
        private readonly ConcurrentDictionary<Type, bool> _messageTypes;

        public InvokeUsingExecutorRule()
        {
            _messageTypes = new ConcurrentDictionary<Type, bool>();
        }

        public bool CanDispatch(object message, object handler)
        {
            return _messageTypes.GetOrAdd(message.GetType(), CheckAttributes);
        }

        private static bool CheckAttributes(Type messageType)
        {
            return messageType.GetCustomAttributes(typeof (InvokeViaExecutorAttribute), false).Any();
        }
    }
}