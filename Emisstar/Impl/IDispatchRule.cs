namespace Codestellation.Emisstar.Impl
{
    public interface IDispatchRule
    {
        bool CanDispatch(object message, object handler);
    }
}