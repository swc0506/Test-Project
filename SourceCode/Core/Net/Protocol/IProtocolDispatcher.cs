namespace Core.Net
{
    public interface IProtocolDispatcher : IUpdateable, IClearable
    {
        void Dispatch(IProtocol protocol);
    }
}