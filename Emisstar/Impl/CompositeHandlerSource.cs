using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public class CompositeHandlerSource : IHandlerSource
    {
        private readonly ISet<IHandlerSource> _sources;
        private readonly ManualResetEventSlim _writeLock;
        private readonly ManualResetEventSlim _readerLock;

        public CompositeHandlerSource() : this(Enumerable.Empty<IHandlerSource>())
        {
            
        }

        public CompositeHandlerSource(IEnumerable<IHandlerSource> handlerSources)
        {
            if (handlerSources == null)
            {
                throw new ArgumentNullException("handlerSources");
            }

            _sources = new HashSet<IHandlerSource>(handlerSources);
            _writeLock = new ManualResetEventSlim(true);
            _readerLock = new ManualResetEventSlim(true);
        }

        public virtual IEnumerable<IHandler<TMessage>> ResolveHandlersFor<TMessage>()
        {
            var copyOfSources = GetCopyOfSources();
            return ResolveHandlersFromSources<TMessage>(copyOfSources);
        }

        private static IEnumerable<IHandler<TMessage>> ResolveHandlersFromSources<TMessage>(IEnumerable<IHandlerSource> sources)
        {
            //returns a copy of handlers for the sake of thread safety.
            var result = new HashSet<IHandler<TMessage>>();
            foreach (var handlerSource in sources)
            {
                result.UnionWith(handlerSource.ResolveHandlersFor<TMessage>());
            }
            return result;
        }

        private IEnumerable<IHandlerSource> GetCopyOfSources()
        {
            //returns a copy of contained sources for the sake of thread safety.
            _writeLock.Wait();
            _readerLock.Set();

            var copyOfSources = new IHandlerSource[_sources.Count];
            _sources.CopyTo(copyOfSources, 0);
            
            _readerLock.Reset();

            return copyOfSources;
        }

        public virtual void AddSource(IHandlerSource handlerSource)
        {
            _writeLock.Reset();
            _readerLock.Wait();

            _sources.Add(handlerSource);

            _writeLock.Set();
        }

        public virtual void RemoveSource(IHandlerSource handlerSource)
        {
            _writeLock.Reset();
            _readerLock.Wait();

            _sources.Remove(handlerSource);

            _writeLock.Set();
        }
    }
}