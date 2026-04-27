using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.AssetFrameWork
{
    public partial class AssetsFrame
    {
        /// <summary>
        /// 开始热更
        /// </summary>
        /// <param name="bundleModule">热更模块</param>
        /// <param name="startHotCallBack">开始热更回调</param>
        /// <param name="hotFinish">热更完成回调</param>
        /// <param name="waitDownLoad">等待下载的回调</param>
        /// <param name="isCheck">是否检测资源版本</param>
        public static void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack,
            Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waitDownLoad, bool isCheck = true)
        {
            Instance.mHotAssets.HotAssets(bundleModule, startHotCallBack, hotFinish, waitDownLoad, isCheck);
        }

        /// <summary>
        /// 检测资源版本时候需要热更，获取需要热更资源的大小
        /// </summary>
        /// <param name="bundleModule">热更模块类型</param>
        /// <param name="callBack">回调</param>
        public static void CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack)
        {
            Instance.mHotAssets.CheckAssetsVersion(bundleModule, callBack);
        }

        /// <summary>
        /// 获取热更模块
        /// </summary>
        /// <param name="bundleModule">热更模块类型</param>
        /// <returns></returns>
        public static HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule)
        {
            return Instance.mHotAssets.GetHotAssetsModule(bundleModule);
        }
        
        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModuleEnum, Action callBack)
        {
            return Instance.mDecompressAssets.StartDeCompressBuiltinFile(bundleModuleEnum, callBack);
        }
        
        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        public static float GetDecompressProgress()
        {
            return Instance.mDecompressAssets.GetDecompressProgress();
        }
        
        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        public static void PreLoadObj(string path, int count = 1)
        {
            Instance.mResource.PreLoadObj(path, count);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public static void PreLoadResource<T>(string path) where T : UnityEngine.Object
        {
            Instance.mResource.PreLoadResource<T>(path);
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
        public static GameObject Instantiate(string path, Transform parent, Vector3 localPosition, Vector3 localScale,
            Quaternion quaternion)
        {
            return Instance.mResource.Instantiate(path, parent, localPosition, localScale, quaternion);
        }

        public static GameObject Instantiate(string path, Transform parent)
        {
            return Instantiate(path, parent, Vector3.zero, Vector3.one,
                Quaternion.identity);
        }

        /// <summary>
        /// 异步克隆一个对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public static void InstantiateAsync(string path, Action<GameObject, object, object> callback, object param1 = null,
            object param2 = null)
        {
            Instance.mResource.InstantiateAsync(path, callback, param1, param2);
        }

        /// <summary>
        /// 移除对象加载回调
        /// </summary>
        /// <param name="loadId"></param>
        public static void RemoveObjectLoadCallBack(long loadId)
        {
            Instance.mResource.RemoveObjectLoadCallBack(loadId);
        }

        /// <summary>
        /// 释放对象占用内存
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroy"></param>
        public static void Release(GameObject obj, bool destroy = false)
        {
            Instance.mResource.Release(obj, destroy);
        }

        /// <summary>
        /// 释放纹理占用内存
        /// </summary>
        /// <param name="texture"></param>
        public static void Release(Texture texture)
        {
            Instance.mResource.Release(texture);
        }
        
        /// <summary>
        /// 清理所有异步加载任务
        /// </summary>
        public static void ClearAllAsyncLoadTask()
        {
            Instance.mResource.ClearAllAsyncLoadTask();
        }

        /// <summary>
        /// 清理加载的资源
        /// </summary>
        /// <param name="absoluteClear">深度清理</param>
        public static void ClearResourcesAssets(bool absoluteClear)
        {
            Instance.mResource.ClearResourcesAssets(absoluteClear);
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
        public static long InstantiateAndLoad(string path, Action<GameObject, object, object> callback, Action loading,
            object param1 = null, object param2 = null)
        {
            return Instance.mResource.InstantiateAndLoad(path, callback, loading, param1, param2);
        }

        /// <summary>
        /// 加载图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sprite LoadSprite(string path)
        {
            return Instance.mResource.LoadSprite(path);
        }

        /// <summary>
        /// 加载纹理资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture LoadTexture(string path)
        {
            return Instance.mResource.LoadTexture(path);
        }

        /// <summary>
        /// 加载音频资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip LoadAudio(string path)
        {
            return Instance.mResource.LoadAudio(path);
        }

        /// <summary>
        /// 加载文本资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TextAsset LoadTextAsset(string path)
        {
            return Instance.mResource.LoadTextAsset(path);
        }

        public static Sprite LoadAtlasSprite(string atlasPath, string path)
        {
            return Instance.mResource.LoadAtlasSprite(atlasPath, path);
        }

        /// <summary>
        /// 异步加载图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadAsync"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long LoadTextureAsync(string path, Action<Texture, object> loadAsync, object param = null)
        {
            return Instance.mResource.LoadTextureAsync(path, loadAsync, param);
        }

        /// <summary>
        /// 异步加载精灵
        /// </summary>
        /// <param name="path"></param>
        /// <param name="image"></param>
        /// <param name="setNativeSize"></param>
        /// <param name="loadAsync"></param>
        /// <returns></returns>
        public static long LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> loadAsync = null)
        {
            return Instance.mResource.LoadSpriteAsync(path, image, setNativeSize, loadAsync);
        }
    }
}
