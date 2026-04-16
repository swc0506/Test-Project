namespace Core.Net
{
    public class Heartbeat : IHeartbeat
    {
        private float interval;

        public float Interval
        {
            get { return interval; }
        }

        private byte[] heartbeatBuffs;

        public Heartbeat(float interval, IProtocolCoder protocolCoder)
        {
            this.interval = interval;

            IProtocol protocol = protocolCoder.CreateProtocol((int)ProtoType.HEARTBEAT, null);
            this.heartbeatBuffs = protocolCoder.Encode(protocol);
        }

        public void Tick(Remote remote)
        {
            remote.SendBuffs(heartbeatBuffs);
        }
    }
}