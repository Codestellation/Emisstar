using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.DarkFlow;
using Codestellation.DarkFlow.Execution;
using Codestellation.Emisstar.CastleWindsor.Facility;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Windsor.Facility
{
    //TODO Needs actualization
    [TestFixture]
    public class EmisstarFacilityTests
    {
        private WindsorContainer _windsor;

        [SetUp]
        public void SetUp()
        {
            _windsor = new WindsorContainer();
            _windsor.AddFacility<EmisstarFacility>();
        }

        [Test]
        public void Facility_configured_to_send_message_to_resolved_handlers()
        {
            var handler = new TestHandler();

            _windsor.Resolve<IAssignee>().Subscribe(handler);
            _windsor.Register(Component.For<IExecutor>().ImplementedBy<SynchronousExecutor>());
            _windsor.Register(Component.For<IHandler<Message>>().ImplementedBy<TestHandler>());

            var publisher = _windsor.Resolve<IPublisher>();
            var message = new Message();

            publisher.Publish(message);

            Assert.That(handler.Message, Is.SameAs(message));
        }
    }
}