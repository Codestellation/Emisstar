namespace Codestellation.Emisstar.Impl
{
    //this is struct intentionally, to decrease pressure on GC.
    public struct MessageHandlerTuple
    {
        public readonly object Message;
        public readonly object Handler;

        public MessageHandlerTuple(object message, object handler)
        {
            Message = message;
            Handler = handler;
        }

        public bool Equals(MessageHandlerTuple other)
        {
            return ReferenceEquals(Message, other.Message) && ReferenceEquals(Handler, other.Handler);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MessageHandlerTuple && Equals((MessageHandlerTuple) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Message.GetHashCode() ^ Handler.GetHashCode();
            }
        }
    }
}