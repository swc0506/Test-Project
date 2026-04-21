using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        // 已经加载的资源
        private Dictionary<uint, BundleItem> mAlreadyLoadAssetsDic = new Dictionary<uint, BundleItem>();

        #region 加载资源
        /// <summary>
        /// 同步加载资源，外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadResource<T>(string path) where T : UnityEngine.Object
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
        public void LoadResourceAsync<T>(string path, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
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