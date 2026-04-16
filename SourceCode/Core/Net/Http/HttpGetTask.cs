using UnityEngine.Networking;

namespace Core.Net
{
    public class HttpGetTask : HttpTask
    {
        public HttpGetTask(string url) : base(url)
        {
        }

        public void Request()
        {
            var webRequest = UnityWebRequest.Get(url);
            Send(webRequest);
        }

        protected override void Retry()
        {
            Request();
        }
    }
}