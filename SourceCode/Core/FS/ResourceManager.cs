using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.FS
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<string, AssetPackage> pkgMap = new Dictionary<string, AssetPackage>();
        private bool pause;
        private bool disableUpdateBundle;

        protected override void OnInitial()
        {
            base.OnInitial();
            pkgMap = new Dictionary<string, AssetPackage>();
            pause = false;
            disableUpdateBundle = false;
        }

        public AssetPackage Load(string pkgName)
        {
            if (!pkgMap.TryGetValue(pkgName, out AssetPackage pkg))
            {
                pkg = new AssetPackage(pkgName);
                pkgMap.Add(pkgName, pkg);
            }

            return pkg;
        }

        public void Loads(IEnumerable<string> pkgNames)
        {
            foreach (var item in pkgNames)
            {
                Load(item);
            }
        }

        public void LoadAsync(string pkgName, Action<AssetPackage, bool> completed)
        {
            if (!pkgMap.TryGetValue(pkgName, out AssetPackage pkg))
            {
                pkg = new AssetPackage(pkgName, completed);
                pkgMap.Add(pkgName, pkg);
            }
            else
            {
                if (!pkg.Bundles.IsLoaded)
                {
                    pkg.Bundles.SetCompletedAction(completed);
                    pkg.Bundles.LoadManifestAsync();
                }
                else
                {
                    completed?.Invoke(pkg, true);
                }
            }
        }

        public void LoadAsyncs(IEnumerable<string> pkgNames, Action<bool> completed)
        {
            int totalCount = 0;
            foreach (var item in pkgNames)
            {
                totalCount++;
            }

            int completedCount = 0;
            bool result = true;
            foreach (var item in pkgNames)
            {
                LoadAsync(item, (pkgName, res) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }
        }

        public AssetPackage Get(string pkgName)
        {
            if (pkgMap.TryGetValue(pkgName, out AssetPackage pkg))
            {
                return pkg;
            }

            return null;
        }

        public void Remove(string pkgName)
        {
            pkgMap.Remove(pkgName);
        }

        public void Remove(string[] pkgNames)
        {
            foreach (var item in pkgNames)
            {
                Remove(item);
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var item in pkgMap)
            {
                item.Value.Update(deltaTime);
            }
        }

        public int TaskCount
        {
            get
            {
                int count = 0;
                foreach (var item in pkgMap)
                {
                    count += item.Value.TaskCount;
                }

                return count;
            }
        }

        public bool Pause
        {
            get { return pause; }
            set
            {
                if (pause != value)
                {
                    pause = value;
                    foreach (var item in pkgMap)
                    {
                        item.Value.Pause = value;
                    }
                }
            }
        }

        public bool DisableUpdateBundle
        {
            get { return disableUpdateBundle; }
            set
            {
                if (disableUpdateBundle != value)
                {
                    disableUpdateBundle = value;
                    foreach (var item in pkgMap)
                    {
                        item.Value.Bundles.DisableUpdateBundle = value;
                    }
                }
            }
        }

        public void Reload()
        {
            foreach (var item in pkgMap)
            {
                item.Value.Reload();
            }
        }

        public void ReloadAsync(Action<bool> completed)
        {
            int totalCount = pkgMap.Count;
            int completedCount = 0;
            bool result = true;
            foreach (var item in pkgMap)
            {
                item.Value.ReloadAsync((pkgName, res) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var item in pkgMap)
            {
                item.Value.Clear();
            }

            pkgMap = null;
        }
    }
}