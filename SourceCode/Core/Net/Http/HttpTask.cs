using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Net
{
    public delegate void HttpCompletedAction(bool res, string data, string url);

    public abstract class HttpTask
    {
        public const int TIMEOUT = 8;

        public int timeout = TIMEOUT;
        public int maxRetryCount = 2;

        protected readonly string url;
        protected HttpCompletedAction completedAction;

        protected int retryCount = 0;
        protected Dictionary<string, string> headers;

        public event HttpCompletedAction CompletedEvent
        {
            add { completedAction += value; }
            remove { completedAction -= value; }
        }

        public HttpTask()
        {
        }

        public HttpTask(string url)
        {
            this.url = url;
        }

        public void SetRequestHeader(string name, string value)
        {
            if (null == headers)
            {
                headers = new Dictionary<string, string>();
            }

            headers[name] = value;
        }

        public void SetRequestHeaders(Dictionary<string, string> headers)
        {
            if (null != headers)
            {
                this.headers = headers;
            }
        }

        protected void Send(UnityWebRequest webRequest)
        {
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeCertificateHandlerOnDispose = true;

            if (webRequest.timeout != timeout)
            {
                webRequest.timeout = timeout;
            }

            if (null != headers)
            {
                foreach (var item in headers)
                {
                    webRequest.SetRequestHeader(item.Key, item.Value);
                }
            }
            else
            {
                //默认json
                webRequest.SetRequestHeader("Content-Type", "application/json");
            }

            UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
            asyncOperation.completed += OnCompleted;
        }


        private void OnCompleted(AsyncOperation asyncOperation)
        {
            UnityWebRequest webRequest = ((UnityWebRequestAsyncOperation)asyncOperation).webRequest;
            string error = webRequest.error;
            string text = webRequest.downloadHandler.text;
            bool isError = webRequest.isHttpError || webRequest.isNetworkError;

            webRequest.Dispose();
            if (isError)
            {
                if (retryCount < maxRetryCount)
                {
                    retryCount++;
                    Retry();
                }
                else
                {
                    completedAction?.Invoke(false, error, url);
                }
            }
            else
            {
                completedAction?.Invoke(true, text, url);
            }
        }

        protected abstract void Retry();
    }
}