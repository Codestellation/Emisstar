namespace Codestellation.Emisstar.Impl
{
    public interface IDispatchRule
    {
        bool CanDispatch(ref MessageHandlerTuple tuple);
    }
}