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

        public bool CanDispatch(ref MessageHandlerTuple tuple)
        {
            return _messageTypes.GetOrAdd(tuple.Message.GetType(), CheckAttributes);
        }

        private static bool CheckAttributes(Type messageType)
        {
            return messageType.GetCustomAttributes(typeof (InvokeViaExecutorAttribute), false).Any();
        }
    }
}