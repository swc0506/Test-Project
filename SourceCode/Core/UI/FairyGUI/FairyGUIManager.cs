#if FAIRYGUI
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Core.FS;
using UnityEngine;
using FairyGUI;
using FairyGUI.Utils;

namespace Core.UI
{
    /// <summary>
    /// 异步加载UI包回调
    /// </summary>
    /// <param name="pkgName"></param>
    public delegate void LoadUIPackageAction(string pkgName, bool result);

    /// <summary>
    /// 异步创建UI对象回调
    /// </summary>
    /// <param name="comp"></param>
    public delegate void UICreateObjectAction(GObject comp, string compName);

    struct AsyncUIAsset : IEquatable<AsyncUIAsset>
    {
        public List<PackageItem> pkgItems;

        public AsyncUIAsset(List<PackageItem> pkgItems)
        {
            this.pkgItems = pkgItems;
        }

        public bool Equals(AsyncUIAsset other)
        {
            return Equals(pkgItems, other.pkgItems);
        }

        public override bool Equals(object obj)
        {
            return obj is AsyncUIAsset other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (pkgItems != null ? pkgItems.GetHashCode() : 0);
        }
    }

    struct AsyncUIPackage : IEquatable<AsyncUIPackage>
    {
        public readonly string pkgName;
        public readonly AssetAsyncHandler handler;
        public event LoadUIPackageAction completedEvent;

        public AsyncUIPackage(string pkgName, AssetAsyncHandler handler)
        {
            this.pkgName = pkgName;
            this.handler = handler;
            completedEvent = null;
        }

        public void Invoke(bool result)
        {
            if (!result)
            {
                Logger.WarnFormat("UIPackage load fail:{0}", pkgName);
            }

            completedEvent?.Invoke(pkgName, result);
        }

        public bool Equals(AsyncUIPackage other)
        {
            return pkgName == other.pkgName;
        }

        public override bool Equals(object obj)
        {
            return obj is AsyncUIPackage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (pkgName != null ? pkgName.GetHashCode() : 0);
        }
    }

    public class FairyGUIManager : Singleton<FairyGUIManager>
    {
        private enum LoadMode
        {
            Sync,
            Async
        }

        public AssetPackage AssetPkg { get; private set; }
        private string prefixPath;

        private Assembly binderAssembly;
        private HashSet<string> binderSet = new HashSet<string>();

        private HashSet<string> pkgSet = new HashSet<string>();
        private HashSet<string> dontUnloadSet = new HashSet<string>();

        private Dictionary<string, AsyncUIAsset> loadingAssetMap = new Dictionary<string, AsyncUIAsset>();
        private Dictionary<string, AsyncUIPackage> loadingPkgMap = new Dictionary<string, AsyncUIPackage>();
        private Dictionary<string, List<AssetObject>> assetMap = new Dictionary<string, List<AssetObject>>();

        protected override void OnInitial()
        {
            base.OnInitial();

            Console.SwitchEvent += SetTouchable;
            NTexture.CustomDestroyMethod += UnloadTexture;
            NAudioClip.CustomDestroyMethod = UnloadAudioClip;

            UIObjectFactory.SetLoaderExtension(typeof(FairyGUILoader));
        }

        private void SetTouchable(bool enable)
        {
            GRoot.inst.touchable = !enable;
        }

        public void SetAssetPackage(AssetPackage assetPkg, string prefixPath = null)
        {
            this.AssetPkg = assetPkg;
            this.prefixPath = prefixPath;
        }

        public void SetAssetPackageName(string pkgName, string prefixPath = null)
        {
            SetAssetPackage(ResourceManager.Instance.Get(pkgName), prefixPath);
        }

        public void SetBranch(string branch)
        {
            UIPackage.branch = branch;
        }

        public void SetDefaultFont(string fontName)
        {
            UIConfig.defaultFont = fontName;
        }

        private bool SetStringsSource(AssetObject asset)
        {
            if (null != asset)
            {
                TextAsset textAsset = asset.Get<TextAsset>();
                UIPackage.SetStringsSource(new XML(textAsset.text));
                asset.Release();
                return true;
            }

            return false;
        }

        public void SetStringsSource(string path)
        {
            AssetObject asset = AssetPkg.Load(path);
            SetStringsSource(asset);
        }

        public void SetStringsSourcesAsync(string path, Action<bool> completed)
        {
            AssetAsyncHandler handler = AssetPkg.LoadAsync(path);
            handler.CompletedEvent += (AssetObject asset, string assetPath) =>
            {
                bool res = SetStringsSource(asset);
                completed?.Invoke(res);
            };
        }

        public void SetUpdateImageTexture(Action<Image> action)
        {
            Image.onUpdateTexture = action;
        }

        private void UnloadTexture(Texture texture)
        {
            ReleaseAsset(texture.GetInstanceID().ToString());
        }

        private void UnloadAudioClip(AudioClip audio)
        {
            ReleaseAsset(audio.GetInstanceID().ToString());
        }

        #region Font

        private bool OnRegisterFont(AssetObject asset, Func<AssetObject, BaseFont> createFontFunc)
        {
            if (null == asset)
            {
                return false;
            }

            string name = Path.GetFileNameWithoutExtension(asset.Path);
            var font = createFontFunc.Invoke(asset);
            font.name = name;
            FontManager.RegisterFont(font);
            asset.SetDontUnload(true);
            return true;
        }

        private DynamicFont CreateDynamicFont(AssetObject asset)
        {
            DynamicFont font = new DynamicFont();
            font.nativeFont = asset.Get<Font>();
            return font;
        }

        public void RegisterFont(string path, Func<AssetObject, BaseFont> createFontFunc)
        {
            AssetObject asset = AssetPkg.Load(path);
            OnRegisterFont(asset, createFontFunc);
        }

        public void RegisterFont(string path)
        {
            RegisterFont(path, CreateDynamicFont);
        }

        public void RegisterFonts(IEnumerable<string> paths, Func<AssetObject, BaseFont> createFontFunc)
        {
            foreach (var item in paths)
            {
                RegisterFont(item, createFontFunc);
            }
        }

        public void RegisterFonts(IEnumerable<string> paths)
        {
            RegisterFonts(paths, CreateDynamicFont);
        }

        public void RegisterFontAsync(string path, Func<AssetObject, BaseFont> createFontFunc, Action<bool> completed)
        {
            AssetAsyncHandler handler = AssetPkg.LoadAsync(path);
            handler.CompletedEvent += (AssetObject asset, string assetPath) =>
            {
                bool res = OnRegisterFont(asset, createFontFunc);
                completed?.Invoke(res);
            };
        }

        public void RegisterFontAsync(string path, Action<bool> completed)
        {
            RegisterFontAsync(path, CreateDynamicFont, completed);
        }

        public void RegisterFontsAsync(IEnumerable<string> paths, Func<AssetObject, BaseFont> createFontFunc,
            Action<bool> completed)
        {
            int totalCount = 0;
            int completedCount = 0;
            foreach (var item in paths)
            {
                totalCount++;
            }

            bool result = true;
            foreach (var item in paths)
            {
                RegisterFontAsync(item, (res =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                }));
            }
        }

        public void RegisterFontsAsync(IEnumerable<string> paths, Action<bool> completed)
        {
            RegisterFontsAsync(paths, CreateDynamicFont, completed);
        }

        #endregion

        public bool HasPackage(string pkgName)
        {
            return pkgSet.Contains(pkgName);
        }

        public UIPackage GetPackage(string pkgName)
        {
            if (HasPackage(pkgName))
            {
                return UIPackage.GetByName(pkgName);
            }

            return null;
        }

        #region Package

        public void SetBinderAssembly(Assembly assembly)
        {
            binderAssembly = assembly;
        }

        public void SetBinderAssembly()
        {
            SetBinderAssembly(Assembly.GetCallingAssembly());
        }

        public void RegisterBinder(string pkgName)
        {
            if (null != binderAssembly && !binderSet.Contains(pkgName))
            {
                string binderName = string.Format("{0}.{1}Binder", pkgName, pkgName);
                Type binderType = binderAssembly.GetType(binderName);
                RegisterBinder(binderType, pkgName);
            }
        }

        public void RegisterBinder(Type binderType, string pkgName)
        {
            if (null == binderType)
            {
                return;
            }

            var method = binderType.GetMethod("BindAll",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (null != method)
            {
                method.Invoke(null, null);
            }

            binderSet.Add(pkgName);
        }

        public void RemoveBinder(string pkgName)
        {
            binderSet.Remove(pkgName);
        }

        private void RetainAsset(string flag, AssetObject asset)
        {
            if (string.IsNullOrEmpty(flag))
            {
                flag = asset.Result.GetInstanceID().ToString();
            }

            if (!assetMap.TryGetValue(flag, out var list))
            {
                list = new List<AssetObject>();
                assetMap.Add(flag, list);
            }

            list.Add(asset);
        }

        private void ReleaseAsset(string flag)
        {
            if (assetMap.TryGetValue(flag, out var list))
            {
                foreach (var item in list)
                {
                    item.Release();
                }

                assetMap.Remove(flag);
            }
        }

        private void AddPackage(string pkgName, AssetObject desAsset, LoadMode mode)
        {
            if (null != desAsset && pkgSet.Add(pkgName))
            {
                RegisterBinder(pkgName);
                RetainAsset(pkgName, desAsset);
                TextAsset text = desAsset.Get<TextAsset>();
                if (mode == LoadMode.Sync)
                {
                    UIPackage.AddPackage(text.bytes, pkgName, LoadAsset);
                }
                else if (mode == LoadMode.Async)
                {
                    UIPackage.AddPackage(text.bytes, pkgName, LoadAssetAsync);
                }
            }
        }

        private object LoadAsset(string name, string extension, Type type, out DestroyMethod destroyMethod)
        {
            destroyMethod = DestroyMethod.Custom;
            string path = string.Format("{0}/{1}{2}", prefixPath, name, extension);
            var asset = AssetPkg.Load(path, type);
            if (null != asset)
            {
                RetainAsset(null, asset);
                return asset.Get();
            }

            return null;
        }

        private void LoadAssetAsync(string name, string extension, Type type, PackageItem pkgItem)
        {
            string path = string.Format("{0}/{1}{2}", prefixPath, name, extension);
            if (!loadingAssetMap.TryGetValue(path, out var item))
            {
                AssetAsyncHandler handler = AssetPkg.LoadAsync(path, type);
                handler.CompletedEvent += OnLoadAssetCompleted;
                item = new AsyncUIAsset(new List<PackageItem>());
            }

            item.pkgItems.Add(pkgItem);
            loadingAssetMap[path] = item;
        }

        private void OnLoadAssetCompleted(AssetObject asset, string path)
        {
            if (loadingAssetMap.TryGetValue(path, out var item))
            {
                foreach (var pkgItem in item.pkgItems)
                {
                    if (null != asset && null != pkgItem.owner)
                    {
                        RetainAsset(null, asset);
                        pkgItem.owner.SetItemAsset(pkgItem, asset.Get(), DestroyMethod.Custom);
                    }
                }

                loadingAssetMap.Remove(path);
            }
        }

        private string GetPkgDesPath(string pkgName)
        {
            return string.Format("{0}/{1}_fui.bytes", prefixPath, pkgName);
        }

        public void LoadPackage(string pkgName)
        {
            if (!HasPackage(pkgName))
            {
                string desPath = GetPkgDesPath(pkgName);
                AssetObject asset = AssetPkg.Load(desPath);
                AddPackage(pkgName, asset, LoadMode.Sync);
            }
        }

        public void LoadPackages(IEnumerable<string> pkgNames)
        {
            foreach (var item in pkgNames)
            {
                LoadPackage(item);
            }
        }

        public void LoadPackageAsync(string pkgName, LoadUIPackageAction completed)
        {
            if (!HasPackage(pkgName))
            {
                string path = GetPkgDesPath(pkgName);
                if (!loadingPkgMap.TryGetValue(path, out var item))
                {
                    AssetAsyncHandler handler = AssetPkg.LoadAsync(path);
                    handler.CompletedEvent += OnLoadPackageCompleted;
                    item = new AsyncUIPackage(pkgName, handler);
                }

                item.completedEvent += completed;
                loadingPkgMap[path] = item;
            }
            else
            {
                completed?.Invoke(pkgName, true);
            }
        }

        private void OnLoadPackageCompleted(AssetObject asset, string path)
        {
            if (loadingPkgMap.TryGetValue(path, out var item))
            {
                if (null != asset)
                {
                    AddPackage(item.pkgName, asset, LoadMode.Async);
                    item.Invoke(true);
                }
                else
                {
                    item.Invoke(false);
                }

                loadingPkgMap.Remove(path);
            }
        }

        public void LoadPackagesAsync(IEnumerable<string> pkgNames, Action<bool> completed)
        {
            int totalCount = 0;
            int completedCount = 0;
            foreach (var item in pkgNames)
            {
                totalCount++;
            }

            bool result = true;
            foreach (var item in pkgNames)
            {
                LoadPackageAsync(item, (pkgName, res) =>
                {
                    result &= res;
                    if (++completedCount >= totalCount)
                    {
                        completed?.Invoke(result);
                    }
                });
            }
        }

        public void UnloadPackage(string pkgName)
        {
            dontUnloadSet.Remove(pkgName);
            if (pkgSet.Remove(pkgName))
            {
                UIPackage pkg = UIPackage.GetByName(pkgName);
                if (null != pkg)
                {
                    UIPackage.RemovePackage(pkgName);
                    ReleaseAsset(pkgName);
                }

                return;
            }

            string path = GetPkgDesPath(pkgName);
            if (loadingPkgMap.TryGetValue(path, out var item))
            {
                item.handler.CompletedEvent -= OnLoadPackageCompleted;
                loadingPkgMap.Remove(path);
            }
        }

        public void UnloadPackages(IEnumerable<string> pkgNames)
        {
            foreach (var item in pkgNames)
            {
                UnloadPackage(item);
            }
        }

        public void UnloadAllPackages()
        {
            HashSet<string> removeSet = new HashSet<string>();
            foreach (var item in pkgSet)
            {
                if (!dontUnloadSet.Contains(item))
                {
                    removeSet.Add(item);
                }
            }

            foreach (var item in loadingPkgMap)
            {
                if (!dontUnloadSet.Contains(item.Value.pkgName))
                {
                    removeSet.Add(item.Value.pkgName);
                }
            }

            foreach (var item in removeSet)
            {
                UnloadPackage(item);
            }
        }

        public void SetDontUnloadPackage(string pkgName, bool dontUnload)
        {
            if (dontUnload)
            {
                dontUnloadSet.Add(pkgName);
            }
            else
            {
                dontUnloadSet.Remove(pkgName);
            }
        }

        public void SetDontUnloadPackages(IEnumerable<string> pkgNames, bool dontUnload)
        {
            foreach (var item in pkgNames)
            {
                SetDontUnloadPackage(item, dontUnload);
            }
        }

        public void ClearDontUnloadPackages()
        {
            dontUnloadSet.Clear();
        }

        #endregion

        #region Object

        public GObject CreateObject(string pkgName, string compName, Type type)
        {
            LoadPackage(pkgName);
            return UIPackage.CreateObject(pkgName, compName, type);
        }

        public GObject CreateObject(string pkgName, string compName)
        {
            return CreateObject(pkgName, compName, null);
        }

        public T CreateObject<T>(string pkgName, string compName) where T : GObject
        {
            return (T)CreateObject(pkgName, compName, typeof(T));
        }

        public void CreateObjectAsync(string pkgName, string compName, Type type, UICreateObjectAction callback)
        {
            LoadPackageAsync(pkgName, (pName, res) =>
            {
                if (res)
                {
                    GObject comp = UIPackage.CreateObject(pkgName, compName, type);
                    callback?.Invoke(comp, compName);
                }
            });
        }

        public void CreateObjectAsync(string pkgName, string compName, UICreateObjectAction completed)
        {
            CreateObjectAsync(pkgName, compName, null, completed);
        }

        public void CreateObjectAsync<T>(string pkgName, string compName, UICreateObjectAction completed)
        {
            CreateObjectAsync(pkgName, compName, typeof(T), completed);
        }

        public GObject CreateObjectFromURL(string url, Type type)
        {
            PackageItem item = UIPackage.GetItemByURL(url);
            if (null == item)
            {
                Logger.WarnFormat("UIPackage is null,please load UIPackage first!:{0}", url);
                return null;
            }

            return UIPackage.CreateObjectFromURL(url, type);
        }

        public GObject CreateObjectFromURL(string url)
        {
            return CreateObjectFromURL(url, null);
        }

        public T CreateObjectFromURL<T>(string url) where T : GObject
        {
            return (T)CreateObjectFromURL(url, typeof(T));
        }

        #endregion

        protected override void OnDispose()
        {
            base.OnDispose();
            ClearDontUnloadPackages();
            UnloadAllPackages();
            Console.SwitchEvent -= SetTouchable;
            NTexture.CustomDestroyMethod -= UnloadTexture;
            NAudioClip.CustomDestroyMethod = null;
        }
    }
}
#endif