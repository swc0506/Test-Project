namespace Core.FS
{
    public interface IAssetCachePool<T> where T : IReferenceCount, IClearable
    {
        /// <summary>
        /// 把资源放入缓存池
        /// </summary>
        /// <param name="asset"></param>
        void Push(T asset);

        /// <summary>
        /// 把资源移除缓存
        /// </summary>
        /// <param name="asset"></param>
        void Pop(T asset);

        /// <summary>
        /// 尝试获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool TryGet(string path, out T asset);

        /// <summary>
        /// 延迟减少引用计数
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="delayTime"></param>
        void DelayDecRef(T asset, float delayTime);
    }
}