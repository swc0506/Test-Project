using System.Collections.Generic;
using UnityEngine;

namespace Core.FS
{
    public class ChainReader : FileReader
    {
        private List<string> paths;
        private int pathIndex;

        public ChainReader(IEnumerable<string> paths, MimeType mimeType)
        {
            this.mimeType = mimeType;
            Initial(paths);
        }

        public ChainReader(IEnumerable<string> paths, AudioType audioType)
        {
            this.audioType = audioType;
            mimeType = MimeType.Audio;
            Initial(paths);
        }

        private void Initial(IEnumerable<string> paths)
        {
            if (null != paths)
            {
                this.paths = new List<string>(paths);
            }
        }

        public override void Read()
        {
            if (null == paths || paths.Count < 0)
            {
                return;
            }

            TryRead();
        }

        private void TryRead()
        {
            if (pathIndex < paths.Count)
            {
                pathIndex++;
                string currPath = paths[pathIndex - 1];
                FileReader fileReader = null;
                if (mimeType == MimeType.Audio)
                {
                    fileReader = new FileReader(currPath, audioType);
                }
                else
                {
                    fileReader = new FileReader(currPath, mimeType);
                }

                fileReader.userData = userData;
                fileReader.timeout = timeout;
                fileReader.maxRetryCount = 0;
                fileReader.CompletedEvent += OnLoadCompleted;
                fileReader.Read();
            }
            else
            {
                completedAction?.Invoke(null, paths[pathIndex - 1], userData);
            }
        }

        private void OnLoadCompleted(object data, string path, object userData)
        {
            if (null == data)
            {
                if (retryCount < maxRetryCount)
                {
                    if (pathIndex >= paths.Count)
                    {
                        retryCount++;
                        pathIndex = 0;
                    }

                    TryRead();
                }
                else
                {
                    completedAction?.Invoke(null, path, userData);
                }
            }
            else
            {
                completedAction?.Invoke(data, path, userData);
            }
        }
    }
}