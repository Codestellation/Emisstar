using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codestellation.Emisstar.Impl
{
    public class CompositeHandlerSource : IHandlerSource
    {
        private IHandlerSource[] _sources;
        private readonly object _latch;

        public CompositeHandlerSource() : this(Enumerable.Empty<IHandlerSource>())
        {
        }

        public CompositeHandlerSource(IEnumerable<IHandlerSource> handlerSources)
        {
            if (handlerSources == null)
            {
                throw new ArgumentNullException("handlerSources");
            }
            _sources = handlerSources.Distinct().ToArray();
            _latch = new object();
        }

        public virtual IEnumerable<object> ResolveHandlersFor(Type messageType)
        {
            var result = new HashSet<object>();
            foreach (var handlerSource in _sources)
            {
                result.UnionWith(handlerSource.ResolveHandlersFor(messageType));
            }
            return result;
        }

        public virtual void AddSource(IHandlerSource handlerSource)
        {
            lock (_latch)
            {
                if (_sources.Contains(handlerSource))
                {
                    return;
                }

                var newSources = new IHandlerSource[_sources.Length + 1];
                _sources.CopyTo(newSources,0);
                newSources[_sources.Length] = handlerSource;

                Interlocked.Exchange(ref _sources, newSources);
            }
        }

        public virtual void RemoveSource(IHandlerSource handlerSource)
        {
            lock (_latch)
            {
                if (!_sources.Contains(handlerSource))
                {
                    return;
                }

                var newSources = _sources.Except(new[] {handlerSource}).ToArray();

                Interlocked.Exchange(ref _sources, newSources);
            }
        }
    }
}