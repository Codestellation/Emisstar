namespace Codestellation.Emisstar.Tests.Impl
{
    public class MultiHandler : TestHandler, IHandler<AnotherMessage>
    {
        public AnotherMessage AnotherMessage;

        public void Handle(AnotherMessage message)
        {
            AnotherMessage = message;
        }
    }
}