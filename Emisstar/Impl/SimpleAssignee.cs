using System;
using System.Collections.Generic;
using System.Linq;

namespace Codestellation.Emisstar.Impl
{
    /// <summary>
    /// Simple thread-safe implementation of <see cref="IAssignee"/>. It serves as <see cref="IHandlerSource"/> for a <see cref="Publisher"/>.
    /// </summary>
    public class SimpleAssignee : IAssignee, IHandlerSource
    {
        private readonly object _latch;
        private readonly Dictionary<Type, ISet<object>> _storeHandlers;
        private readonly HashSet<object> _emptySet;

        public SimpleAssignee()
        {
            _storeHandlers = new Dictionary<Type, ISet<object>>();
            _emptySet = new HashSet<object>();
            _latch = new object();
        }

        /// <summary>
        /// Subscribes handler to events.
        /// </summary>
        /// <param name="handler">Object to subscribe.
        /// <remarks>If handler already subscribed implement <see cref="IHandler{TMessage}"/> it will be silently ignored.</remarks>
        /// </param>
        public virtual void Subscribe(object handler)
        {
            var interfaces = GetImplementedHandlers(handler);

            if(interfaces.Length == 0) return;
            //TODO Consider faster implementation using slim events.
            lock(_latch)
            {
                foreach (var @interface in interfaces)
                {
                    ISet<object> handlers;
                    if(_storeHandlers.TryGetValue(@interface, out handlers))
                    {
                        handlers.Add(handler);
                    }
                    else
                    {
                        _storeHandlers[@interface] = new HashSet<object> { handler };
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes handler from events. 
        /// </summary>
        /// <param name="handler">Object to unsubscirbed
        /// <remarks>If handler already unsubscribed it will be silently ignored.</remarks>
        /// </param>
        public virtual void Unsubscribe(object handler)
        {
            var interfaces = GetImplementedHandlers(handler);
            if (interfaces.Length == 0) return;

            lock (_latch)
            {
                foreach (var @interface in interfaces)
                {
                    ISet<object> handlers;
                    if (_storeHandlers.TryGetValue(@interface, out handlers))
                    {
                        handlers.Remove(handler);
                    }
                }
            }
        }

        public virtual IEnumerable<IHandler<TMessage>> ResolveHandlersFor<TMessage>()
        {
            ISet<object> handlers;
            lock(_latch)
            {
                _storeHandlers.TryGetValue(typeof (IHandler<TMessage>), out handlers);
                handlers = new HashSet<object>(handlers ?? _emptySet);
            }
            return handlers.Cast<IHandler<TMessage>>();
        }

        private static Type[] GetImplementedHandlers(object handler)
        {
            var interfaces = handler.GetType().FindInterfaces(
                (@interface, noMatter) => @interface.IsGenericType &&
                                          @interface.GetGenericTypeDefinition() == typeof (IHandler<>), null);
            return interfaces;
        }
    }
}