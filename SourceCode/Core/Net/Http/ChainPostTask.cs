using System.Collections.Generic;
using UnityEngine;

namespace Core.Net
{
    public class ChainPostTask : HttpTask
    {
        private List<string> paths;
        private int pathIndex;

        private object postData;

        public ChainPostTask(IEnumerable<string> paths)
        {
            if (null != paths)
            {
                this.paths = new List<string>(paths);
            }
        }

        public void Request(string text)
        {
            postData = text;
            Request();
        }

        public void Request(Dictionary<string, string> map)
        {
            postData = map;
            Request();
        }

        public void Request(WWWForm form)
        {
            postData = form;
            Request();
        }

        private void Request()
        {
            if (null == paths || paths.Count < 0)
            {
                return;
            }

            retryCount = 0;
            pathIndex = 0;
            TryRequest();
        }

        private void TryRequest()
        {
            if (pathIndex < paths.Count)
            {
                pathIndex++;
                string currPath = paths[pathIndex - 1];
                HttpPostTask requestTask = new HttpPostTask(currPath);
                requestTask.timeout = timeout;
                requestTask.maxRetryCount = 0;
                requestTask.SetRequestHeaders(headers);
                requestTask.CompletedEvent += OnRequestCompleted;
                if (postData is string)
                {
                    requestTask.Request((string)postData);
                }
                else if (postData is Dictionary<string, string>)
                {
                    requestTask.Request((Dictionary<string, string>)postData);
                }
                else if (postData is WWWForm)
                {
                    requestTask.Request((WWWForm)postData);
                }
            }
            else
            {
                completedAction?.Invoke(false, null, paths[pathIndex - 1]);
            }
        }

        private void OnRequestCompleted(bool res, string data, string url)
        {
            if (!res)
            {
                if (retryCount < maxRetryCount)
                {
                    if (pathIndex >= paths.Count)
                    {
                        retryCount++;
                        pathIndex = 0;
                    }

                    TryRequest();
                }
                else
                {
                    completedAction?.Invoke(false, null, url);
                }
            }
            else
            {
                completedAction?.Invoke(res, data, url);
            }
        }

        protected override void Retry()
        {
        }
    }
}