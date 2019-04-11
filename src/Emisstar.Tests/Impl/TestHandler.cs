using System.Threading;

namespace Codestellation.Emisstar.Tests.Impl
{
    //TODO Replace it with main TestHandler
    public class TestHandler : IHandler<TestMessage>
    {
        public TestMessage TestMessage;
        public readonly ManualResetEvent Called;

        public TestHandler()
        {
            Called = new ManualResetEvent(false);
        }

        public void Handle(TestMessage message)
        {
            TestMessage = message;
            Called.Set();
        }
    }
}