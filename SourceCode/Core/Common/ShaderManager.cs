using System;
using System.Collections.Generic;
using Core.FS;
using UnityEngine;

namespace Core
{
    public class ShaderVariantData
    {
        private string path;
        private AssetObject asset;
        private ShaderVariantCollection shaderVariant;

        public ShaderVariantData(string path, AssetObject asset)
        {
            this.path = path;
            this.asset = asset;
            if (null != asset)
            {
                asset.SetDontUnload(true);
            }
        }

        public void WramUp()
        {
            if (null != asset && null == shaderVariant)
            {
                shaderVariant = asset.Get<ShaderVariantCollection>();
                shaderVariant.WarmUp();
            }
        }

        public void Dispose()
        {
            if (null != asset)
            {
                asset.SetDontUnload(false);
                if (null != shaderVariant)
                {
                    asset.Release();
                    shaderVariant = null;
                }

                asset = null;
            }
        }
    }

    public class ShaderManager : Singleton<ShaderManager>
    {
        private Dictionary<string, ShaderVariantData> loadedMap = new Dictionary<string, ShaderVariantData>();

        public void LoadShaderVariants(string pkgName, string path, Action<bool> completed)
        {
            string key = pkgName + path;
            if (loadedMap.TryGetValue(key, out var shaderData))
            {
                completed?.Invoke(true);
                return;
            }

            var pkg = ResourceManager.Instance.Get(pkgName);
            var asyncHandler = pkg.LoadAsync(path);
            asyncHandler.CompletedEvent += (asset, resPath) =>
            {
                if (null != asset)
                {
                    ShaderVariantData data = null;
                    if (!loadedMap.TryGetValue(key, out data))
                    {
                        data = new ShaderVariantData(path, asset);
                        loadedMap.Add(key, data);
                        data.WramUp();
                    }

                    completed?.Invoke(true);
                }
                else
                {
                    completed?.Invoke(false);
                }
            };
        }

        public void LoadShaderVariants(string pkgName, string path)
        {
            LoadShaderVariants(pkgName, path,  null);
        }

        public ShaderVariantData GetShaderVariantData(string pkgName, string path)
        {
            string key = pkgName + path;
            if (loadedMap.TryGetValue(key, out ShaderVariantData data))
            {
                return data;
            }

            return null;
        }

        public void Clear()
        {
            foreach (var item in loadedMap)
            {
                item.Value.Dispose();
            }

            loadedMap.Clear();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Clear();
            loadedMap = null;
        }
    }
}