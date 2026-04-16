#if VIRTUAL

using System.Collections.Generic;
using System.IO;

namespace Core.Net
{
    public class VirtualServerManager : Singleton<VirtualServerManager>
    {
        private List<VirtualServer> servers = new List<VirtualServer>();

        public long Timestamp { get; private set; }

        public void SetTimestamp(long timestamp)
        {
            Timestamp = timestamp;
        }

        public VirtualServer StartupServer(string url)
        {
            VirtualServer server = GetVirtualServer(url);
            if (null == server)
            {
                server = new VirtualServer(url);
                servers.Add(server);
            }

            return server;
        }

        public void ShutdownServer(string url)
        {
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i].Url == url)
                {
                    servers[i].Dispose();
                    servers.RemoveAt(i);
                    break;
                }
            }
        }

        public VirtualServer GetVirtualServer(string url)
        {
            foreach (var item in servers)
            {
                if (item.Url == url)
                {
                    return item;
                }
            }

            return null;
        }

        public void Update(float delta)
        {
            foreach (var item in servers)
            {
                item.Update(delta);
            }
        }
    }
}

#endif