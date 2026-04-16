using System.Collections.Generic;

namespace Core.Net
{
    public class NetChannelManager : Singleton<NetChannelManager>
    {
        private Dictionary<string, NetChannel> channelMap = new Dictionary<string, NetChannel>();

        public bool Add(NetChannel netChannel)
        {
            if (!channelMap.ContainsKey(netChannel.Name))
            {
                channelMap.Add(netChannel.Name, netChannel);
                return true;
            }

            return false;
        }

        public bool Contains(string name)
        {
            return channelMap.ContainsKey(name);
        }

        public NetChannel Get(string name)
        {
            if (channelMap.TryGetValue(name, out var remote))
            {
                return remote;
            }

            return null;
        }

        public T Get<T>(string name) where T : NetChannel
        {
            return (T)Get(name);
        }

        public void Delete(string name)
        {
            if (channelMap.TryGetValue(name, out var netChannel))
            {
                netChannel.Dispose();
                channelMap.Remove(name);
            }
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var item in channelMap)
            {
                item.Value.Dispose();
            }

            channelMap = null;
        }
        
    }
}