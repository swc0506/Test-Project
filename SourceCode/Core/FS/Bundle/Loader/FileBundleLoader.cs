using System.IO;
using UnityEngine;

namespace Core.FS
{
    /// <summary>
    /// 文件加载方式
    /// </summary>
    public class FileBundleLoader : AbstractBundleLoader
    {
        public FileBundleLoader(AssetBundlePackage context) : base(context)
        {
        }

        protected override AssetBundle CreateLoadBundle(string path)
        {
            string loadPath = GetLoadPath(path);
            if (Application.isEditor && !File.Exists(loadPath))
            {
                return null;
            }

            ulong offset = AssetBundlePackage.GetOffset(path);
            AssetBundle result = AssetBundle.LoadFromFile(loadPath, 0, offset);
            return result;
        }

        protected override IBundleAsyncRequest CreateAsyncRequest(string path)
        {
            string loadPath = GetLoadPath(path);
            AbstractBundleRequest asyncRequest = null;
            if (Application.isEditor && !File.Exists(loadPath))
            {
                asyncRequest = AssetBundleObject.RefPool.Pop<NullBundleRequest>();
            }
            else
            {
                asyncRequest = AssetBundleObject.RefPool.Pop<FileBundleRequest>();
            }

            asyncRequest.Initial(path, loadPath, OnLoadAsyncCompleted, context);
            return asyncRequest;
        }
    }
}