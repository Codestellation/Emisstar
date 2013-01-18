using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codestellation.Emisstar.Testing
{
    public class TestHandler<TMessage> : IHandler<TMessage>
    {
        private readonly List<TMessage> _messages;
        
        private readonly ManualResetEventSlim _called;
        private int _calledTimes;

        public void WaitUntilCalled(int timeout = 10)
        {
            if (_called.Wait(timeout)) return;
            throw new InvalidOperationException(string.Format("TestHandler<{0}> was not called in a second.", typeof(TMessage)));
        }

        public TestHandler()
        {
            _messages = new List<TMessage>();
            _called = new ManualResetEventSlim(false);
        }

        public int CalledTimes
        {
            get { return _calledTimes; }
        }

        public bool WasCalledAtLeastOnce
        {
            get { return CalledTimes > 0; }
        }

        public TMessage LastMessage
        {
            get { return _messages.Last(); }
        }

        public IList<TMessage> Messages
        {
            get { return _messages.AsReadOnly(); }
        }

        public bool WasCalledOnce
        {
            get { return CalledTimes == 1; }
        }

        public void Handle(TMessage message)
        {
            Interlocked.Increment(ref _calledTimes);
            
            _messages.Add(message);
            _called.Set();
        }
    }
}