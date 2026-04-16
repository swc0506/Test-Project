using System;
using System.Collections.Generic;
#if TCP||KCP
using System.Net;
using System.Net.Sockets;
#endif

namespace Core
{
    public class NetUtils
    {
#if TCP||KCP
        public static bool TryParseHost(string host, out IPAddress address)
        {
            if (IPAddress.TryParse(host, out address))
            {
                return true;
            }

            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                if (null != addresses && addresses.Length > 0)
                {
                    address = addresses[0];
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("TryParseHost error,host:{0},errorMsg:{1}", host, e);
            }

            return false;
        }

        /// <summary>
        /// 获取本地内网ip
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            string ip = null;
            string hostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            foreach (var address in ipEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = address.ToString();
                    break;
                }
            }

            return ip;
        }

#endif

        public static void GetIp(IEnumerable<string> urls, Action<bool, string> callback)
        {
            HttpUtils.ChinaGet(urls, (bool res, string data, string url) =>
            {
                if (res && null != data)
                {
                    string text = (string)data;
                    text = text.Replace("\r", string.Empty).Replace("\n", string.Empty);
                    callback?.Invoke(RegexUtils.IsIp(text), text);
                }
                else
                {
                    callback?.Invoke(false, null);
                }
            });
        }

        public static void GetIp(Action<bool, string> callback)
        {
            string[] ipUrls = new string[]
            {
                "https://checkip.amazonaws.com",
                "https://api.ipify.org",
                "https://ipinfo.io/ip",
            };
            GetIp(ipUrls, callback);
        }
    }
}