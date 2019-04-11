namespace Codestellation.Emisstar.Tests.Windsor
{
    //TODO Replace it with testhandler from main solution
    public class TestHandler : IHandler<Message>
    {
        public void Handle(Message message)
        {
            Message = message;
        }

        public Message Message;
    }
}