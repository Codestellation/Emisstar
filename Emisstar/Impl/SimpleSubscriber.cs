using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    /// <summary>
    /// Simple thread-safe implementation of <see cref="ISubscriber"/>. It serves as <see cref="IHandlerSource"/> for a <see cref="Publisher"/>.
    /// </summary>
    public class SimpleSubscriber : ISubscriber, IHandlerSource
    {
        private readonly ReaderWriterLockSlim _latch;
        private readonly Dictionary<Type, ISet<object>> _storeHandlers;
        private readonly HashSet<object> _emptySet;

        public SimpleSubscriber()
        {
            _storeHandlers = new Dictionary<Type, ISet<object>>();
            _emptySet = new HashSet<object>();
            _latch = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
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

            if (interfaces.Length == 0) return;

            _latch.EnterReadLock();

            foreach (var @interface in interfaces)
            {
                ISet<object> handlers;
                if (_storeHandlers.TryGetValue(@interface, out handlers))
                {
                    handlers.Add(handler);
                }
                else
                {
                    _storeHandlers[@interface] = new HashSet<object> { handler };
                }
            }
            _latch.ExitReadLock();
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

            _latch.EnterWriteLock();

            foreach (var @interface in interfaces)
            {
                ISet<object> handlers;
                if (_storeHandlers.TryGetValue(@interface, out handlers))
                {
                    handlers.Remove(handler);
                }
            }

            _latch.ExitWriteLock();
        }

        public virtual IEnumerable<IHandler<TMessage>> ResolveHandlersFor<TMessage>()
        {
            ISet<object> handlers;

            _latch.EnterReadLock();

            _storeHandlers.TryGetValue(typeof(IHandler<TMessage>), out handlers);
            handlers = new HashSet<object>(handlers ?? _emptySet);

            _latch.ExitReadLock();

            return handlers.Cast<IHandler<TMessage>>();
        }

        private static Type[] GetImplementedHandlers(object handler)
        {
            var interfaces = handler.GetType().FindInterfaces(
                (@interface, noMatter) => @interface.IsGenericType &&
                                          @interface.GetGenericTypeDefinition() == typeof(IHandler<>), null);
            return interfaces;
        }
    }
}