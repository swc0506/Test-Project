using System;

namespace Core.FS
{
    /// <summary>
    /// 异步加载资源回调
    /// </summary>
    /// <param name="asset">资源对象</param>
    public delegate void LoadAssetAction(AssetObject asset, string path);

    /// <summary>
    /// 资源加载器
    /// </summary>
    public interface IAssetLoader : IUpdateable,IClearable
    {
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">类型</param>
        /// <returns>资源对象</returns>
        AssetObject Load(string path, Type type);

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">类型</param>
        /// <param name="priority">优先级</param>
        /// <returns>资源异步操作对象</returns>
        AssetAsyncHandler LoadAsync(string path, Type type, int priority);

        /// <summary>
        /// 设置最大异步请求数量 防止同一帧加载过多导致卡顿
        /// </summary>
        /// <param name="value">个数</param>
        void SetMaxAsyncCount(int value);

        /// <summary>
        /// 停止所有异步任务
        /// </summary>
        void StopAsync();

        /// <summary>
        /// 加载任务数量 包含未完成的
        /// </summary>
        int TaskCount { get; }
    }
}