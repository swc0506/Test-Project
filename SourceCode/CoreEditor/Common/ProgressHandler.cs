using System;
using UnityEditor;

namespace CoreEditor
{
    public class ProgressHandler : IDisposable
    {
        string content;
        private int totalCount;
        private string title;

        private int completeCount;
        private float progress;

        public void SetInfo(int totalCount, string content, string title = "Hold On")
        {
            this.totalCount = totalCount;
            this.title = title;
            this.content = content;

            completeCount = 0;
            progress = 0;
        }

        public void Tick(int interval = 1)
        {
            completeCount += interval;
            progress = completeCount / (float) (totalCount);

            string tipStr = string.Format("{0}  Progress:{1:P2} {2}/{3}", content, progress, completeCount,
                totalCount);
            EditorUtility.DisplayProgressBar(title, tipStr, progress);
            if (completeCount >= totalCount)
            {
                Close();
            }
        }

        private void Close()
        {
            EditorUtility.ClearProgressBar();
        }

        public void Dispose()
        {
            Close();
        }
    }
}