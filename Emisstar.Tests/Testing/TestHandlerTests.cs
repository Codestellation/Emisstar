using System;
using Codestellation.Emisstar.Testing;
using NUnit.Framework;
using SharpTestsEx;

namespace Codestellation.Emisstar.Tests.Testing
{
    [TestFixture]
    public class TestHandlerTests
    {
        [Test]
        public void Handle_called_once_has_right_correct_properties()
        {
            var handler = new TestHandler<int>();

            handler.Handle(10);

            handler.Satisfy(h => 
                h.CalledTimes == 1 &&
                h.WasCalledOnce &&
                h.WasCalledAtLeastOnce  &&
                h.LastMessage == 10);
        }
        
        [Test]
        public void Handle_called_twice_has_right_correct_properties()
        {
            var handler = new TestHandler<int>();

            handler.Handle(10);
            handler.Handle(15);

            handler.Satisfy(h =>
                h.CalledTimes == 2 &&
                h.WasCalledAtLeastOnce &&
                h.WasCalledOnce == false &&
                h.Messages[0] == 10 &&
                h.Messages[1] == 15 &&
                h.LastMessage == 15);
        }

        [Test]
        public void Throws_if_no_call_to_handle_was_perfomed()
        {
            var handler = new TestHandler<int>();

            Assert.Throws<InvalidOperationException>(() => handler.WaitUntilCalled());
        }

        [Test]
        public void Skip_waiting_when_call_performed()
        {
            var handler = new TestHandler<int>();

            handler.Handle(10);

            Assert.DoesNotThrow(() => handler.WaitUntilCalled());
        }
    }
}