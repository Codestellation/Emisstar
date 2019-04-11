namespace Codestellation.Emisstar.Tests.Windsor.Facility
{
    //TODO Duplicates another multihandler
    public class MultiHandler : IHandler<int>, IHandler<string>
    {
        public void Handle(int message)
        {
        }

        public void Handle(string message)
        {
        }
    }
}