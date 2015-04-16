using System.Linq;
using Codestellation.Emisstar.LightInject;
using Codestellation.Emisstar.Testing;
using Codestellation.Emisstar.Tests.Windsor;
using LightInject;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.LightInject
{
    [TestFixture]
    public class LightInjectorHandlerSourceTests
    {
        [Test]
        public void Resolve_returns_instances_from_kernel()
        {
            var container = new ServiceContainer();
            container.Register<IHandler<Message>, TestHandler<Message>>(new PerContainerLifetime());
            container.Register<IHandler<Message>, TestHandler<Message>>("Other");

            var source = new LightInjectSource(container);

            var handlers = source.ResolveHandlersFor(typeof(Message));

            Assert.That(handlers.Count(), Is.EqualTo(2));
        } 
    }
}