using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Net
{
    public class HttpPostTask : HttpTask
    {
        private object postData;

        public HttpPostTask(string url) : base(url)
        {
        }

        public void Request(string text)
        {
            postData = text;
            var webRequest = new UnityWebRequest(url, "POST");
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(text));
            webRequest.uploadHandler.contentType = "application/json;charset=UTF-8";
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            Send(webRequest);
        }

        public void Request(Dictionary<string, string> map)
        {
            postData = map;
            var webRequest = UnityWebRequest.Post(url, map);
            Send(webRequest);
        }

        public void Request(WWWForm form)
        {
            postData = form;
            var webRequest = UnityWebRequest.Post(url, form);
            Send(webRequest);
        }

        protected override void Retry()
        {
            if (postData is string)
            {
                Request((string)postData);
            }
            else if (postData is Dictionary<string, string>)
            {
                Request((Dictionary<string, string>)postData);
            }
            else if (postData is WWWForm)
            {
                Request((WWWForm)postData);
            }
        }
    }
}