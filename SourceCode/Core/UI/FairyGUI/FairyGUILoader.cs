#if FAIRYGUI
using System.Collections.Generic;
using Core.FS;
using FairyGUI;
using UnityEngine;

namespace Core.UI
{
    public class FairyGUILoader : GLoader
    {
        private Dictionary<AssetObject, NTexture> assetMap;

        //默认异步加载
        public bool isAsync = false;
        private AssetAsyncHandler asyncHandler;

        private int tryCount;

        protected override void LoadExternal()
        {
            var assetPkg = FairyGUIManager.Instance.AssetPkg;
            if (isAsync)
            {
                ClearLoadEvent();
                asyncHandler = assetPkg.LoadAsync(url);
                asyncHandler.CompletedEvent += OnLoadTextureComplete;
            }
            else
            {
                var asset = assetPkg.Load(url);
                OnLoadTextureComplete(asset, url);
            }
        }

        private void ClearLoadEvent()
        {
            if (!asyncHandler.IsCompleted && !string.IsNullOrEmpty(asyncHandler.Path))
            {
                asyncHandler.CompletedEvent -= OnLoadTextureComplete;
            }
        }

        private void OnLoadTextureComplete(AssetObject asset, string path)
        {
            if (null != asset)
            {
                if (null == assetMap)
                {
                    assetMap = new Dictionary<AssetObject, NTexture>();
                }

                NTexture nTex = null;
                if (!assetMap.TryGetValue(asset, out nTex))
                {
                    Texture2D tex2D = asset.Get<Texture2D>();
                    nTex = new NTexture(tex2D);
                    assetMap.Add(asset, nTex);
                }

                onExternalLoadSuccess(nTex);
                return;
            }

            Logger.WarnFormat("Loader Texture Is NUll:{0}", path);
            onExternalLoadFailed();
        }

        protected override void OnContentItemError()
        {
            if (tryCount > 1)
            {
                return;
            }

            string pkgName = GetPackageName(url);
            if (string.IsNullOrEmpty(pkgName))
            {
                return;
            }

            tryCount++;
            FairyGUIManager.Instance.LoadPackageAsync(pkgName, OnLoadPackageCompleted);
        }

        private string GetPackageName(string url)
        {
            if (url == null)
                return null;

            int pos1 = url.IndexOf("//");
            if (pos1 == -1)
                return null;

            int pos2 = url.IndexOf('/', pos1 + 2);
            if (pos2 > 0)
            {
                string pkgName = url.Substring(pos1 + 2, pos2 - pos1 - 2);
                return pkgName;
            }

            return null;
        }

        private void OnLoadPackageCompleted(string pkgName, bool result)
        {
            if (!result || isDisposed)
            {
                return;
            }

            LoadFromPackage(url);
        }

        public override void Dispose()
        {
            ClearLoadEvent();
            if (null != assetMap)
            {
                foreach (var item in assetMap)
                {
                    item.Key.Release();
                }

                assetMap = null;
            }

            base.Dispose();
        }
    }
}
#endif