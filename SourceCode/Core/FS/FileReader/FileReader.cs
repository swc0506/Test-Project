using UnityEngine;
using UnityEngine.Networking;

namespace Core.FS
{
    public delegate void ReadCompletedAction(object data, string path, object userData);

    public enum MimeType
    {
        Binary,
        Text,
        Texture,
        Audio
    }

    public class FileReader
    {
        public int timeout = 5;
        public int maxRetryCount = 1;

        private readonly string path;
        protected MimeType mimeType;
        protected ReadCompletedAction completedAction;
        protected AudioType audioType = AudioType.OGGVORBIS;

        protected int retryCount;
        internal object userData;

        public event ReadCompletedAction CompletedEvent
        {
            add { completedAction += value; }
            remove { completedAction -= value; }
        }

        public FileReader()
        {
        }

        public FileReader(string path, AudioType audioType)
        {
            this.path = path;
            this.audioType = audioType;
            mimeType = MimeType.Audio;
        }

        public FileReader(string path, MimeType mimeType)
        {
            this.path = path;
            this.mimeType = mimeType;
        }

        public virtual void Read()
        {
            UnityWebRequest webRequest = null;
            if (mimeType == MimeType.Binary || mimeType == MimeType.Text)
            {
                webRequest = UnityWebRequest.Get(path);
            }
            else if (mimeType == MimeType.Texture)
            {
                webRequest = UnityWebRequestTexture.GetTexture(path);
            }
            else if (mimeType == MimeType.Audio)
            {
                webRequest = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
            }
            else
            {
                Logger.ErrorFormat("MimeType dont realize:{0}", mimeType);
            }

            webRequest.timeout = timeout;
            UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
            asyncOperation.completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperation asyncOperation)
        {
            UnityWebRequest webRequest = ((UnityWebRequestAsyncOperation)asyncOperation).webRequest;
            bool isError = webRequest.isHttpError || webRequest.isNetworkError;
            object data = ReadData(webRequest);

            webRequest.Dispose();
            if (isError)
            {
                if (retryCount < maxRetryCount)
                {
                    retryCount++;
                    Read();
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

        private object ReadData(UnityWebRequest webRequest)
        {
            object data;
            if (mimeType == MimeType.Binary)
            {
                data = webRequest.downloadHandler.data;
            }
            else if (mimeType == MimeType.Text)
            {
                data = webRequest.downloadHandler.text;
            }
            else if (mimeType == MimeType.Texture)
            {
                data = DownloadHandlerTexture.GetContent(webRequest);
            }
            else if (mimeType == MimeType.Audio)
            {
                data = DownloadHandlerAudioClip.GetContent(webRequest);
            }
            else
            {
                data = null;
            }

            return data;
        }
    }
}