using System;

namespace Codestellation.Emisstar.Tests.Windsor
{
    [Obsolete("Use TestHandler<TMessage> instead")]
    public class TestTransientHandler : IHandler<Message>
    {
        public Message Message;
        public static Message StaticMessage;

        public void Handle(Message message)
        {
            Message = message;
            StaticMessage = message;
        }
    }
}