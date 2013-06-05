using System;
using System.Collections;
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
        private readonly Dictionary<Type, ISet<object>> _handlersStore;
        private readonly object[] _emptySet;
        private readonly Dictionary<Type, Type[]> _typesCache;

        public SimpleSubscriber()
        {
            _handlersStore = new Dictionary<Type, ISet<object>>();
            _emptySet = new object[0];
            _latch = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _typesCache = new Dictionary<Type, Type[]>();
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

            _latch.EnterWriteLock();

            foreach (var @interface in interfaces)
            {
                ISet<object> handlers;
                if (_handlersStore.TryGetValue(@interface, out handlers))
                {
                    var newHandlerSet = new HashSet<object>(handlers) {handler};
                    //Completely replace handler to prevent multithreading issues;
                    _handlersStore[@interface] = newHandlerSet;
                }
                else
                {
                    _handlersStore[@interface] = new HashSet<object> { handler };
                }
            }
            _latch.ExitWriteLock();
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
                if (!_handlersStore.TryGetValue(@interface, out handlers)) continue;

                //If it not contains - do nothing
                if (!handlers.Contains(handler)) continue;

                //Completely replace handlers set to prevent multithreading issues;
                _handlersStore[@interface] = new HashSet<object>(handlers.Where(x => !x.Equals(handler)));
            }

            _latch.ExitWriteLock();
        }

        public virtual IEnumerable<object> ResolveHandlersFor(Type messageType)
        {
            ISet<object> handlers;

            var handlerType = typeof (IHandler<>).MakeGenericType(messageType);

            _latch.EnterReadLock();
            
            _handlersStore.TryGetValue(handlerType, out handlers);
            
            _latch.ExitReadLock();

            if (handlers == null)
            {
                return _emptySet;
            }

            //We never change handler set, so it safe to return it without deep copy. 
            return handlers;
        }

        private Type[] GetImplementedHandlers(object handler)
        {
            Type[] implementedHandlers;

            if (!_typesCache.TryGetValue(handler.GetType(), out implementedHandlers))
            {
                implementedHandlers = handler.GetType().FindInterfaces(
                    (@interface, noMatter) => @interface.IsGenericType &&
                                              @interface.GetGenericTypeDefinition() == typeof(IHandler<>), null);
            }

            return implementedHandlers;
        }
    }


}