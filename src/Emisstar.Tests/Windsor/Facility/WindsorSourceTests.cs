using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.Emisstar.CastleWindsor.Facility;
using Codestellation.Emisstar.Testing;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Windsor.Facility
{
    [TestFixture]
    public class WindsorSourceTests
    {
        [Test]
        public void Resolve_returns_instances_from_kernel()
        {
            var windsor = new WindsorContainer();
            windsor.Register(
                Component
                    .For<IHandler<Message>>()
                    .ImplementedBy<TestHandler<Message>>()
                    .LifestyleSingleton(),
                Component
                    .For<IHandler<Message>>()
                    .ImplementedBy<TestHandler<Message>>()
                    .Named("transient")
                    .LifestyleTransient());

            var source = new WindsorSource(windsor.Kernel);

            var handlers = source.ResolveHandlersFor(typeof(Message));

            Assert.That(handlers.Count(), Is.EqualTo(2));
        } 
    }
}