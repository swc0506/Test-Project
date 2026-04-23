using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ZM.AssetFrameWork
{
    public class CacheObject
    {
        public uint crc;
        public string path;
        public int insId;
        public GameObject obj;

        public void Release()
        {
            crc = 0;
            insId = 0;
            path = string.Empty;
            obj = null;
        }
    }

    /// <summary>
    /// 加载对象回调
    /// </summary>
    public class LoadObjectCallBack
    {
        public string path;
        public uint crc;
        public object param1;
        public object param2;
        public Action<GameObject, object, object> callback;
    }

    public class ResourceManager : IResourceInterface
    {
        // 已经加载的资源
        private Dictionary<uint, BundleItem> mAlreadyLoadAssetsDic = new Dictionary<uint, BundleItem>();

        // 对象池字典
        private Dictionary<uint, List<CacheObject>> mObjectPoolDic = new Dictionary<uint, List<CacheObject>>();

        // 所有对象字典
        private Dictionary<int, CacheObject> mAllObjectDic = new Dictionary<int, CacheObject>();

        // 对象池
        private ClassObjectPool<CacheObject> mCacheObjectPool = new ClassObjectPool<CacheObject>(200);

        // 异步加载任务列表
        private List<long> mAsyncLoadingTaskList = new List<long>();

        // 加载对象的回调
        private Dictionary<long, LoadObjectCallBack>
            mLoadObjectCallBackDic = new Dictionary<long, LoadObjectCallBack>(); 
        
        // 等待资源加载的列表
        private List<HotFileInfo> mWaitLoadAssetsList = new List<HotFileInfo>();

        //异步加载任务唯一id
        private long asyncGuid;

        private long MAsyncTaskGuId => asyncGuid++;

        public void Initialize()
        {
            HotAssetsManager.downLoadBundleFinish += AssetsDownLoadFinish;
        }
        
        #region 对象加载
        
        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        public void PreLoadObj(string path, int count = 1)
        {
            List<GameObject> preLoadList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                preLoadList.Add(Instantiate(path, null, Vector3.zero, Vector3.one, Quaternion.identity));
            }
            foreach (var obj in preLoadList)
            {
                Release(obj, false);
            }
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public void PreLoadResource<T>(string path) where T : UnityEngine.Object
        {
            LoadResource<T>(path);
        }
        
        /// <summary>
        /// AssetBundle下载完成的回调
        /// </summary>
        /// <param name="info"></param>
        private void AssetsDownLoadFinish(HotFileInfo info)
        {
            Debug.Log("ResourceManager AB Download Finish: " + info.abName);
            //处理比AB配置文件先下载下来的AB加载
            if (info.abName.ToLower().Contains("bundleconfig"))
            {
                Debug.Log("Handler waitLoadList Count: " + mWaitLoadAssetsList.Count);
                HotFileInfo[] hotFileInfos = mWaitLoadAssetsList.ToArray();
                mWaitLoadAssetsList.Clear();
                foreach (var hotFileInfo in hotFileInfos)
                {
                    AssetsDownLoadFinish(hotFileInfo);
                }
                return;
            }
            
            if (mLoadObjectCallBackDic.Count > 0)
            {
                //根据对象路径查找所在AB包，以及这个AB下的所有资源
                List<BundleItem> assetsItems = AssetBundleManager.Instance.GetBundleItemByABName(info.abName);
                if (assetsItems.Count > 0)
                {
                    List<long> removeList = new List<long>();
                    foreach (var item in mLoadObjectCallBackDic)
                    {
                        if (ListContainsAsset(assetsItems, item.Value.crc))
                        {
                            Debug.Log("ResourceManager AssetsDownLoadFinish Load Obj Path: " + item.Value.path);
                            item.Value.callback?.Invoke(
                                Instantiate(item.Value.path, null, Vector3.zero, Vector3.one, Quaternion.identity),
                                item.Value.param1, item.Value.param2);
                            removeList.Add(item.Key);
                        }
                    }
                    
                    foreach (var item in removeList)
                    {
                        mLoadObjectCallBackDic.Remove(item);
                    }
                }
                else
                {
                    foreach (var t in mWaitLoadAssetsList)
                    {
                        if (string.Equals(t.abName, info.abName))
                        {
                            return;
                        }
                    }
                    mWaitLoadAssetsList.Add(info);
                }
            }
        }

        private bool ListContainsAsset(List<BundleItem> assetsItemList, uint crc)
        {
            foreach (var item in assetsItemList)
            {
                if (item.crc == crc)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 同步克隆物体
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="localPosition"></param>
        /// <param name="localScale"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public GameObject Instantiate(string path, Transform parent, Vector3 localPosition, Vector3 localScale,
            Quaternion quaternion)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            uint crc = Crc32.GetCrc32(path);
            GameObject obj = GetCacheObjFromPool(crc);
            if (obj == null)
            {
                //加载对象
                obj = LoadResource<GameObject>(path);
                if (obj != null)
                {
                    obj = Instantiate(path, obj, parent);
                }
            }

            if (obj != null)
            {
                obj.transform.localPosition = localPosition;
                obj.transform.localScale = localScale;
                obj.transform.localRotation = quaternion;
            }
            else
            {
                Debug.LogError("对象加载失败");
            }

            return obj;
        }

        private GameObject GetCacheObjFromPool(uint crc)
        {
            if (mObjectPoolDic.TryGetValue(crc, out List<CacheObject> objList) && objList.Count > 0)
            {
                CacheObject obj = objList[0];
                objList.RemoveAt(0);
                return obj.obj;
            }

            return null;
        }

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private GameObject Instantiate(string path, GameObject obj, Transform parent)
        {
            obj = Object.Instantiate(obj, parent, false);
            CacheObject cacheObj = mCacheObjectPool.Spawn();
            cacheObj.crc = Crc32.GetCrc32(path);
            cacheObj.path = path;
            cacheObj.obj = obj;
            cacheObj.insId = obj.GetInstanceID();
            mAllObjectDic[cacheObj.insId] = cacheObj;
            return obj;
        }

        /// <summary>
        /// 异步克隆一个对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void InstantiateAsync(string path, Action<GameObject, object, object> callback, object param1 = null,
            object param2 = null)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            uint crc = Crc32.GetCrc32(path);
            GameObject obj = GetCacheObjFromPool(crc);
            if (obj != null)
            {
                callback?.Invoke(obj, param1, param2);
            }
            else
            {
                long guId = MAsyncTaskGuId;
                mAsyncLoadingTaskList.Add(guId);
                //异步加载
                LoadResourceAsync<GameObject>(path, (go) =>
                {
                    if (obj != null)
                    {
                        if (mAsyncLoadingTaskList.Contains(guId))
                        {
                            mAsyncLoadingTaskList.Remove(guId);
                            GameObject nObj = Instantiate(path, (GameObject)go, null);
                            callback?.Invoke(nObj, param1, param2);
                        }
                    }
                    else
                    {
                        mAsyncLoadingTaskList.Remove(guId);
                        Debug.LogError("对象加载失败: " + path);
                    }
                });
            }
        }

        /// <summary>
        /// 移除对象加载回调
        /// </summary>
        /// <param name="loadId"></param>
        public void RemoveObjectLoadCallBack(long loadId)
        {
            if (loadId == -1)
                return;
            
            mLoadObjectCallBackDic.Remove(loadId);
        }

        /// <summary>
        /// 释放对象占用内存
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroy"></param>
        public void Release(GameObject obj, bool destroy = false)
        {
            int insId = obj.GetInstanceID();
            if (mAllObjectDic.TryGetValue(insId, out CacheObject cacheObj))
            {
                if (cacheObj.obj == null)
                {
                    Debug.LogError("Release error: obj is GameObject.Instantiate...");
                    return;
                }

                if (destroy)
                {
                    Object.Destroy(obj);
                    mAllObjectDic.Remove(insId);

                    if (mObjectPoolDic.TryGetValue(cacheObj.crc, out List<CacheObject> objPoolList))
                    {
                        objPoolList.Remove(cacheObj);
                        cacheObj.Release();
                        mCacheObjectPool.Despawn(cacheObj);
                    }

                    //卸载该对象的AB
                    if (objPoolList == null || objPoolList.Count == 0)
                    {
                        if (mAlreadyLoadAssetsDic.TryGetValue(cacheObj.crc, out BundleItem item))
                        {
                            AssetBundleManager.Instance.ReleaseAssets(item, true);
                        }
                        else
                        {
                            Debug.LogError("Release error: obj is not found in mAlreadyLoadAssetsDic..." + cacheObj.path);
                        }
                    }
                }
                else
                {
                    //回收到对象池
                    if (!mObjectPoolDic.TryGetValue(cacheObj.crc, out List<CacheObject> objPoolList))
                    {
                        //如果没有该crc的对象池，则新建一个对象池
                        objPoolList = new List<CacheObject>();
                        mObjectPoolDic.Add(cacheObj.crc, objPoolList);
                    }

                    objPoolList.Add(cacheObj);

                    if (cacheObj.obj != null)
                    {
                        cacheObj.obj.transform.SetParent(AssetsFrame.RecycleObjRoot);
                    }
                    else
                    {
                        Debug.LogError("cacheObj.obj is null Release Failed");
                    }
                }
            }
        }

        /// <summary>
        /// 释放纹理占用内存
        /// </summary>
        /// <param name="texture"></param>
        public void Release(Texture texture)
        {
            Resources.UnloadAsset(texture);
        }
        
        /// <summary>
        /// 清理所有异步加载任务
        /// </summary>
        public void ClearAllAsyncLoadTask()
        {
            mAsyncLoadingTaskList.Clear();
        }

        /// <summary>
        /// 清理加载的资源
        /// </summary>
        /// <param name="absoluteClear">深度清理</param>
        public void ClearResourcesAssets(bool absoluteClear)
        {
            if (absoluteClear)
            {
                foreach (var item in mAllObjectDic)
                {
                    if (item.Value.obj != null)
                    {
                        Object.Destroy(item.Value.obj);
                        item.Value.Release();
                        mCacheObjectPool.Despawn(item.Value);
                    }
                }
                mAllObjectDic.Clear();
                mObjectPoolDic.Clear(); 
                ClearAllAsyncLoadTask();
            }
            else
            {
                foreach (var objList in mObjectPoolDic.Values)
                {
                    if (objList != null)
                    {
                        foreach (var cacheObject in objList)
                        {
                            if (cacheObject.obj != null)
                            {
                                Object.Destroy(cacheObject.obj);
                                cacheObject.Release();
                                mCacheObjectPool.Despawn(cacheObject);
                            }
                        }
                    }
                }
                
                mObjectPoolDic.Clear();
            }

            // 释放AB 以及里面资源所占内存
            foreach (var item in mAlreadyLoadAssetsDic)
            {
                AssetBundleManager.Instance.ReleaseAssets(item.Value, absoluteClear);
            }
            mLoadObjectCallBackDic.Clear();
            mAlreadyLoadAssetsDic.Clear();
            //释放未使用的资源
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 克隆并且等待资源下载完成克隆
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="loading"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public long InstantiateAndLoad(string path, Action<GameObject, object, object> callback, Action loading,
            object param1 = null, object param2 = null)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            uint crc = Crc32.GetCrc32(path);
            GameObject cacheObj = GetCacheObjFromPool(crc);
            long loadId = -1;
            if (cacheObj != null)
            {
                callback?.Invoke(cacheObj, param1, param2);
                return loadId;
            }

            GameObject obj = Instantiate(path, null, Vector3.zero, Vector3.one, Quaternion.identity);
            if (obj != null)
            {
                callback?.Invoke(obj, param1, param2);
            }
            else
            {
                //资源没有下载完成 本地没有资源
                loadId = MAsyncTaskGuId;
                loading?.Invoke();
                mLoadObjectCallBackDic[loadId] = new LoadObjectCallBack
                {
                    crc = Crc32.GetCrc32(path),
                    callback = callback,
                    param1 = param1,
                    param2 = param2,
                };
            }

            return loadId;
        }
        
        #endregion

        #region 加载资源

        /// <summary>
        /// 加载图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string path)
        {
            if (!path.EndsWith(".png"))
                path += ".png";
            
            return LoadResource<Sprite>(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Texture LoadTexture(string path)
        {
            if (!path.EndsWith(".jpg"))
                path += ".jpg";
            
            return LoadResource<Texture>(path);
        }

        /// <summary>
        /// 加载音频资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AudioClip LoadAudio(string path)
        {
            return LoadResource<AudioClip>(path);
        }

        /// <summary>
        /// 加载文本资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TextAsset LoadTextAsset(string path)
        {
            return LoadResource<TextAsset>(path);
        }

        public Sprite LoadAtlasSprite(string atlasPath, string path)
        {
            if (!path.EndsWith(".spriteatlas"))
                path += ".spriteatlas";
            return LoadSpriteFromAtlas(LoadResource<SpriteAtlas>(atlasPath), path);
        }
        
        /// <summary>
        /// 从图集中加载精灵
        /// </summary>
        /// <param name="atlas"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Sprite LoadSpriteFromAtlas(SpriteAtlas atlas, string name)
        {
            if (atlas == null)
            {
                Debug.LogError("Not Find SpriteAtlas: " + atlas.name);
                return null;
            }

            Sprite sprite = atlas.GetSprite(name);
            if (sprite == null)
            {
                Debug.LogError("Not Find Sprite: " + name);
            }
            return sprite;
        }

        /// <summary>
        /// 异步加载图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadAsync"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public long LoadTextureAsync(string path, Action<Texture, object> loadAsync, object param = null)
        {
            if (!path.EndsWith(".jpg"))
                path += ".jpg";

            long guId = MAsyncTaskGuId;
            mAsyncLoadingTaskList.Add(guId);
            LoadResourceAsync<Texture>(path, (obj) =>
            {
                if (obj != null)
                {
                    if (mAsyncLoadingTaskList.Contains(guId))
                    {
                        mAsyncLoadingTaskList.Remove(guId);
                        loadAsync?.Invoke((Texture)obj, param);
                    }
                }
                else
                {
                    mAsyncLoadingTaskList.Remove(guId);
                    Debug.LogError("LoadTextureAsync error: " + path);
                }
            });
            
            return guId;
        }

        /// <summary>
        /// 异步加载精灵
        /// </summary>
        /// <param name="path"></param>
        /// <param name="image"></param>
        /// <param name="setNativeSize"></param>
        /// <param name="loadAsync"></param>
        /// <returns></returns>
        public long LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> loadAsync = null)
        {
            if (!path.EndsWith(".png"))
                path += ".png";

            long guId = MAsyncTaskGuId;
            mAsyncLoadingTaskList.Add(guId);
            LoadResourceAsync<Texture>(path, (obj) =>
            {
                if (obj != null)
                {
                    if (mAsyncLoadingTaskList.Contains(guId))
                    {
                        Sprite sprite = obj as Sprite;
                        if (image != null)
                        {
                            image.sprite = sprite;
                            if (setNativeSize)
                            {
                                image.SetNativeSize();
                            }
                        }
                        mAsyncLoadingTaskList.Remove(guId);
                        loadAsync?.Invoke(sprite);
                    }
                }
                else
                {
                    mAsyncLoadingTaskList.Remove(guId);
                    Debug.LogError("LoadSpriteAsync error: " + path);
                }
            });
            
            return guId;
        }
        
        /// <summary>
        /// 同步加载资源，外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("资源路径为空");
                return null;
            }

            uint crc = Crc32.GetCrc32(path);
            BundleItem item = GetCacheBundleItemFromDic(crc);

            if (item.obj != null)
            {
                return item.obj as T;
            }

            //声明新对象
            T obj = null;

#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                obj = LoadAssetsFormEditor<T>(path);
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadAssetBundle(crc);
                if (item != null)
                {
                    if (item.assetBundle != null)
                    {
                        obj = item.obj != null ? item.obj as T : item.assetBundle.LoadAsset<T>(item.assetName);
                    }
                    else
                    {
                        Debug.LogError("资源加载失败");
                    }
                }
                else
                {
                    Debug.LogError("资源加载失败");
                    return null;
                }
            }

            item.obj = obj;
            item.bundlePath = path;
            mAlreadyLoadAssetsDic[crc] = item;
            return obj;
        }

        /// <summary>
        /// 异步加载资源，外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private void LoadResourceAsync<T>(string path, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("资源路径为空");
                callback?.Invoke(null);
            }

            uint crc = Crc32.GetCrc32(path);
            BundleItem item = GetCacheBundleItemFromDic(crc);

            if (item.obj != null)
            {
                callback?.Invoke(item.obj as T);
            }

            //声明新对象
            T obj = null;

#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                obj = LoadAssetsFormEditor<T>(path);
                callback?.Invoke(obj);
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadAssetBundle(crc);
                if (item != null)
                {
                    if (item.obj != null)
                    {
                        callback?.Invoke(item.obj);
                        item.bundlePath = path;
                        item.crc = crc;
                        mAlreadyLoadAssetsDic[crc] = item;
                    }
                    else
                    {
                        // 异步加载
                        AssetBundleRequest request = item.assetBundle.LoadAssetAsync<T>(item.assetName);
                        request.completed += (asyncOperation) =>
                        {
                            UnityEngine.Object loadObj = ((AssetBundleRequest)asyncOperation).asset;
                            item.obj = loadObj;
                            item.bundlePath = path;
                            item.crc = crc;
                            mAlreadyLoadAssetsDic[crc] = item;
                            callback?.Invoke(loadObj);
                        };
                    }
                }
                else
                {
                    Debug.LogError("资源加载失败");
                    callback?.Invoke(null);
                }
            }
            else
            {
                item.obj = obj;
                item.bundlePath = path;
                mAlreadyLoadAssetsDic[crc] = item;
            }
        }

        /// <summary>
        /// 通过资源路径获取缓存的BundleItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public BundleItem GetCacheBundleItemFromDic(uint crc)
        {
            if (mAlreadyLoadAssetsDic.TryGetValue(crc, out BundleItem item))
                return item;

            return new BundleItem
            {
                crc = crc,
            };
        }

#if UNITY_EDITOR
        private T LoadAssetsFormEditor<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif

        #endregion
    }
}