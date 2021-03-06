using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.Emisstar.CastleWindsor.Facility;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Windsor.Facility
{
    [TestFixture]
    public class EmisstarFacilityTests
    {
        [Test]
        public void Facility_configured_to_send_message_to_resolved_handlers()
        {
            var windsor = new WindsorContainer();
            windsor.AddFacility<EmisstarFacility>();

            var handler = new TestHandler();

            windsor.Resolve<ISubscriber>().Subscribe(handler);
            windsor.Register(Component.For<IHandler<Message>>().ImplementedBy<TestHandler>());

            var publisher = windsor.Resolve<IPublisher>();
            var message = new Message();

            publisher.Publish(message);

            Assert.That(handler.Message, Is.SameAs(message));
        }
    }
}