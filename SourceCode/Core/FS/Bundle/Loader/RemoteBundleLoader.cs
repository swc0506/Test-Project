using System.IO;
using UnityEngine;

namespace Core.FS
{
    /// <summary>
    /// 网络加载方式
    /// </summary>
    public class RemoteBundleLoader : AbstractBundleLoader
    {
        public RemoteBundleLoader(AssetBundlePackage context) : base(context)
        {
        }

        protected override AssetBundle CreateLoadBundle(string path)
        {
            string loadPath = GetLoadPath(path);
            if (Application.isEditor && !File.Exists(loadPath))
            {
                return null;
            }

            AssetBundle result = AssetBundle.LoadFromFile(loadPath);
            return result;
        }

        private string GetRemoteLoadPath(string path)
        {
            return string.Format("{0}/{1}/{2}", string.Empty, context.AssetPkg.Name, path);
        }

        protected override IBundleAsyncRequest CreateAsyncRequest(string path)
        {
            RemoteBundleRequest asyncRequest = AssetBundleObject.RefPool.Pop<RemoteBundleRequest>();
            string loadPath = GetRemoteLoadPath(path);
            asyncRequest.Initial(path, loadPath, OnLoadAsyncCompleted, context);
            return asyncRequest;
        }
    }
}