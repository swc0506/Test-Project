using System;
using System.Collections.Generic;
using Core.FS;

namespace Core.UI
{
    public class UIFXManager : Singleton<UIFXManager>
    {
        class AssetPkgData
        {
            public AssetPackage assetPkg;
            public string prefixPath;

            public AssetPkgData(AssetPackage assetPkg, string prefixPath)
            {
                this.assetPkg = assetPkg;
                this.prefixPath = prefixPath;
            }
        }

        public float offsetDistance = 5f;

        private Dictionary<string, AssetPkgData> pkgMap = new Dictionary<string, AssetPkgData>();
        private AssetPkgData defPkgData;

        public string touchPkgName;
        public string touchCompName;

        private Dictionary<string, Queue<UIFX>> poolMap = new Dictionary<string, Queue<UIFX>>();
        private Dictionary<string, HashSet<UIFX>> activeMap = new Dictionary<string, HashSet<UIFX>>();

        private HashSet<UIFX> dontDestroySet = new HashSet<UIFX>();

        private List<UIFX> releaseTemps = new List<UIFX>();

        public void AddAssetPackage(AssetPackage assetPkg, string prefixPath = null)
        {
            if (!pkgMap.ContainsKey(assetPkg.Name))
            {
                AssetPkgData assetPkgData = new AssetPkgData(assetPkg, prefixPath);
                pkgMap.Add(assetPkg.Name, assetPkgData);
            }
        }

        public void AddAssetPackageName(string pkgName, string prefixPath = null)
        {
            var assetPkg = ResourceManager.Instance.Get(pkgName);
            if (null != assetPkg)
            {
                AddAssetPackage(assetPkg, prefixPath);
            }
        }

        public void SetDefaultPkg(string pkgName)
        {
            if (pkgMap.TryGetValue(pkgName, out var pkgData))
            {
                defPkgData = pkgData;
            }
        }

        private Queue<UIFX> GetQueue(string name)
        {
            if (!poolMap.TryGetValue(name, out var queue))
            {
                queue = new Queue<UIFX>();
                poolMap.Add(name, queue);
            }

            return queue;
        }

        private string GetFXPath(string path, string prefixPath)
        {
            if (!string.IsNullOrEmpty(prefixPath))
            {
                return string.Format("{0}/{1}", prefixPath, path);
            }
            else
            {
                return path;
            }
        }

        private UIFX CreateUIFX(string path, AssetPkgData pkgData)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            UIFX fx = null;
            string loadPath = GetFXPath(path, pkgData.prefixPath);
            var queue = GetQueue(loadPath);
            if (queue.Count > 0)
            {
                fx = queue.Dequeue();
            }
            else
            {
                if (UIManager.Instance.UIMode == UIMode.InternalGUI)
                {
                    return null;
                }
                else if (UIManager.Instance.UIMode == UIMode.FairyGUI)
                {
#if FAIRYGUI
                    fx = new FairyUIFX();
                    fx.Initial(loadPath, pkgData.assetPkg);
#endif
                }
            }

            if (!activeMap.TryGetValue(loadPath, out var set))
            {
                set = new HashSet<UIFX>();
                activeMap.Add(loadPath, set);
            }

            set.Add(fx);
            return fx;
        }

        public UIFX Create(string path, string pkgName)
        {
            if (pkgMap.TryGetValue(pkgName, out var pkgData))
            {
                return CreateUIFX(path, pkgData);
            }

            return null;
        }

        public UIFX Create(string path)
        {
            if (null != defPkgData)
            {
                return CreateUIFX(path, defPkgData);
            }

            return null;
        }

        private bool ReleaseUIFX(HashSet<UIFX> set, UIFX fx)
        {
            if (set.Remove(fx))
            {
                GetQueue(fx.Path).Enqueue(fx);
                fx.Clear();
            }

            return false;
        }

        public bool Release(UIFX fx)
        {
            if (null != fx && activeMap.TryGetValue(fx.Path, out var set))
            {
                return ReleaseUIFX(set, fx);
            }

            return false;
        }

        public bool Destroy(UIFX fx)
        {
            if (null != fx && activeMap.TryGetValue(fx.Path, out var set))
            {
                if (set.Remove(fx))
                {
                    fx.Dispose();
                    dontDestroySet.Remove(fx);
                    return true;
                }
            }

            return false;
        }

        public void DestroyAll()
        {
            HashSet<UIFX> removes = new HashSet<UIFX>();
            foreach (var item in activeMap)
            {
                removes.Clear();
                foreach (var fx in item.Value)
                {
                    if (!dontDestroySet.Contains(fx))
                    {
                        removes.Add(fx);
                    }
                }

                foreach (var fx in removes)
                {
                    fx.Dispose();
                    item.Value.Remove(fx);
                }
            }

            foreach (var item in poolMap)
            {
                foreach (var fx in item.Value)
                {
                    fx.Dispose();
                }
            }

            poolMap.Clear();
        }

        public void SetDontDestroy(UIFX uiFx, bool dontDestroy)
        {
            if (dontDestroy)
            {
                dontDestroySet.Add(uiFx);
            }
            else
            {
                dontDestroySet.Remove(uiFx);
            }
        }

        public void SetDontDestroys(IEnumerable<UIFX> uiFxs, bool dontDestroy)
        {
            foreach (var item in uiFxs)
            {
                SetDontDestroy(item, dontDestroy);
            }
        }

        public void ClearDontDestroys()
        {
            dontDestroySet.Clear();
        }

        public void Update(float deltaTime)
        {
            foreach (var item in activeMap)
            {
                foreach (var fx in item.Value)
                {
                    if (fx.IsFinished)
                    {
                        releaseTemps.Add(fx);
                    }
                }

                if (releaseTemps.Count > 0)
                {
                    foreach (var fx in releaseTemps)
                    {
                        ReleaseUIFX(item.Value, fx);
                    }

                    releaseTemps.Clear();
                }
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            dontDestroySet.Clear();
            DestroyAll();
        }
    }
}