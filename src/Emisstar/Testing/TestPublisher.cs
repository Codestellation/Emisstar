using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.Testing
{
    public class TestPublisher : Publisher
    {
        private readonly ISubscriber _subscriber;

        public ISubscriber GetAssignee()
        {
            return _subscriber;
        }

        public TestPublisher() :this(new SimpleSubscriber(), new SimpleDispatcher())
        {
            
        }

        private TestPublisher(ISubscriber subscriber, IDispatcher dispatcher) : base((IHandlerSource)subscriber, new[] {dispatcher})
        {
            _subscriber = subscriber;
        }

        public TestHandler<TMessage> RegisterTestHandler<TMessage>()
        {
            var result = new TestHandler<TMessage>();
            GetAssignee().Subscribe(result);
            return result;
        }
    }
}