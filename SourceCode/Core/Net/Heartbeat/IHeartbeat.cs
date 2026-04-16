namespace Core.Net
{
    public interface IHeartbeat
    {
        float Interval { get; }

        void Tick(Remote remote);
    }
}