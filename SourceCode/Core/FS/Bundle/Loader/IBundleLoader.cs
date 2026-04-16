namespace Core.FS
{
    /// <summary>
    /// assetbundle 资源加载器
    /// </summary>
    public interface IBundleLoader : IUpdateable
    {
        /// <summary>
        /// 同步加载assetbundle
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>资源Bundle对象</returns>
        AssetBundleObject Load(string path);

        /// <summary>
        /// 异步加载assetbundle
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="priority">优先级</param>
        /// <returns>返回返回异步加载bundle对象</returns>
        AssetBundleAsyncOperation LoadAsync(string path, int priority);

        /// <summary>
        /// 设置最大异步请求数量 防止同一帧加载过多导致卡顿
        /// </summary>
        /// <param name="value">个数</param>
        void SetMaxAsyncCount(int value);
        
        /// <summary>
        /// 通知所有异步任务
        /// </summary>
        void StopAllAsync();
    }
}