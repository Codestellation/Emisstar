using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.Emisstar.CastleWindsor.Facility;
using Codestellation.Emisstar.Testing;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Windsor.Facility
{
    [TestFixture]
    public class RegistrationExtensionTests
    {


        [Test]
        public void Registration_helper_methods_should_properly_register_handlers()
        {
            var windsor = new WindsorContainer();
            windsor.Register(
                Classes
                    .FromThisAssembly()
                    .IncludeNonPublicTypes()
                    .Where(type => type.IsHandler())
                    .WithServiceAllHandlers());

            var handlers = windsor.ResolveAll<IHandler<Message>>();

            Assert.That(handlers.Count(), Is.EqualTo(2));
        }

    }
}