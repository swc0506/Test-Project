using System;
using System.Collections.Generic;
using Core.FS;
using UnityEngine;

namespace Core
{
    public class AssetPackageProvider : IGameObjectProvider
    {
        private Dictionary<int, AssetObject> assetMap = new Dictionary<int, AssetObject>();
        private AssetPackage assetPackage;

        private AtomicInt createId = new AtomicInt();
        private HashSet<int> actions = new HashSet<int>();

        public AssetPackageProvider(AssetPackage assetPackage)
        {
            this.assetPackage = assetPackage;
        }

        public AssetPackageProvider(string assetPackageName)
        {
            this.assetPackage = ResourceManager.Instance.Get(assetPackageName);
        }

        public GameObject Load(string path)
        {
            AssetObject asset = assetPackage.Load(path);
            if (null != asset)
            {
                GameObject go = asset.Get<GameObject>();
                assetMap.Add(go.GetInstanceID(), asset);
                return go;
            }

            return null;
        }

        public int LoadAsync(string path, Action<string, GameObject> callback)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(path))
            {
                id = createId.IncrementAndGet();
                actions.Add(id);

                AssetAsyncHandler asyncHandler = assetPackage.LoadAsync(path);
                asyncHandler.AddCompleted((asset, aPath) =>
                {
                    if (null == actions || !actions.Remove(id))
                    {
                        return;
                    }

                    if (null != asset)
                    {
                        GameObject go = asset.Get<GameObject>();
                        assetMap.Add(go.GetInstanceID(), asset);
                        callback?.Invoke(aPath, go);
                    }
                    else
                    {
                        callback?.Invoke(aPath, null);
                    }
                });
            }
            else
            {
                callback?.Invoke(path, null);
            }

            return id;
        }

        public void CancelAsync(int id)
        {
            actions.Remove(id);
        }

        public void Destroy(GameObject go)
        {
            if (null != go)
            {
                int instId = go.GetInstanceID();
                GameObject.Destroy(go);
                if (assetMap.TryGetValue(instId, out var asset))
                {
                    assetMap.Remove(instId);
                    asset.Release();
                }
            }
        }

        public void Dispose()
        {
            foreach (var item in assetMap)
            {
                item.Value.Release();
            }

            assetMap = null;
            createId = null;
            actions = null;
        }
    }
}