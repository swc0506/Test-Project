using System.Collections.Generic;

namespace Core.Net
{
    public class ChainGetTask : HttpTask
    {
        private List<string> paths;
        private int pathIndex;

        public ChainGetTask(IEnumerable<string> paths)
        {
            if (null != paths)
            {
                this.paths = new List<string>(paths);
            }
        }

        public void Request()
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
                HttpGetTask requestTask = new HttpGetTask(currPath);
                requestTask.timeout = timeout;
                requestTask.maxRetryCount = 0;
                requestTask.SetRequestHeaders(headers);
                requestTask.CompletedEvent += OnRequestCompleted;
                requestTask.Request();
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