﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public class OrderedRule : IDispatchRule
    {
        private readonly Dictionary<Type, bool> _messageTypes;

        public OrderedRule()
        {
            _messageTypes = new Dictionary<Type, bool>();
        }

        public bool CanDispatch(object message, object handler)
        {
            Debug.Assert(message != null);

            bool result;
            
            if (_messageTypes.TryGetValue(message.GetType(), out result))
            {
                return result;
            }
            
            Monitor.Enter(_messageTypes);

            if(!_messageTypes.TryGetValue(message.GetType(), out result))
            {
                result = message.GetType().GetCustomAttributes(typeof(OrderedExecutionAttribute), false).Any();
                _messageTypes[message.GetType()] = result;
            }

            Monitor.Exit(_messageTypes);

            return result;
        }
    }
}