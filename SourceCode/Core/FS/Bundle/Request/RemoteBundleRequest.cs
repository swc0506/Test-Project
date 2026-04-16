using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.FS
{
    class CacheDownloadHandlerAssetBundle : DownloadHandlerScript
    {
        private readonly string savePath;
        private Stream fileStream;

        public CacheDownloadHandlerAssetBundle(string savePath)
            : base(new byte[512])
        {
            this.savePath = savePath;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (fileStream == null)
            {
                FileUtils.CreateDirectory(Path.GetDirectoryName(savePath));
                fileStream = new FileStream(savePath, FileMode.Create);
            }

            fileStream.Write(data, 0, dataLength);
            return true;
        }

        protected override void CompleteContent()
        {
            fileStream?.Close();
        }
    }

    public class RemoteBundleRequest : AbstractBundleRequest
    {
        private UnityWebRequest GetAssetBundleRequest(string url, string savePath)
        {
            UnityWebRequest webRequest =
                new UnityWebRequest(url, "GET", new CacheDownloadHandlerAssetBundle(savePath), null);
            webRequest.timeout = 6;
            return webRequest;
        }

        public override void Request()
        {
            string updatePath = string.Format("{0}/{1}", context.UpdateBundlePath, Path);
            UnityWebRequest webRequest = GetAssetBundleRequest(loadPath, updatePath);
            UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();
            request.completed += OnRequestCompleted;
        }

        private void OnRequestCompleted(AsyncOperation operation)
        {
            AssetBundle bundle = null;
            UnityWebRequest webRequest = ((UnityWebRequestAsyncOperation) operation).webRequest;
            if (!webRequest.isNetworkError && !webRequest.isHttpError)
            {
                string updatePath = string.Format("{0}/{1}", context.UpdateBundlePath, Path);
                ulong offset = AssetBundlePackage.GetOffset(Path);
                bundle = AssetBundle.LoadFromFile(updatePath, 0, offset);
            }

            webRequest.Dispose();

            OnLoadCompleted(bundle);
        }
    }
}