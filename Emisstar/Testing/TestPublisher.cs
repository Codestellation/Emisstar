using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.Testing
{
    public class TestPublisher : Publisher
    {
        private readonly IAssignee _assignee;

        public IAssignee GetAssignee()
        {
            return _assignee;
        }

        public TestPublisher() :this(new SimpleAssignee(), new SimpleDispatcher())
        {
            
        }

        private TestPublisher(IAssignee assignee, IDispatcher dispatcher) : base((IHandlerSource)assignee, new[] {dispatcher})
        {
            _assignee = assignee;
        }

        public TestHandler<TMessage> RegisterTestHandler<TMessage>()
        {
            var result = new TestHandler<TMessage>();
            GetAssignee().Subscribe(result);
            return result;
        }
    }
}