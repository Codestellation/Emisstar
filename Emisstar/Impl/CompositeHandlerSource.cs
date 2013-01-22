using System;
using System.Collections.Generic;
using System.Linq;

namespace Codestellation.Emisstar.Impl
{
    public class CompositeHandlerSource : IHandlerSource
    {
        private volatile IHandlerSource[] _sources;
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

        public virtual IEnumerable<IHandler<TMessage>> ResolveHandlersFor<TMessage>()
        {
            return ResolveHandlersFromSources<TMessage>(_sources);
        }

        private static IEnumerable<IHandler<TMessage>> ResolveHandlersFromSources<TMessage>(IEnumerable<IHandlerSource> sources)
        {
            var result = new HashSet<IHandler<TMessage>>();
            foreach (var handlerSource in sources)
            {
                result.UnionWith(handlerSource.ResolveHandlersFor<TMessage>());
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
                _sources = newSources;
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
                
                _sources = newSources;
            }
        }
    }
}