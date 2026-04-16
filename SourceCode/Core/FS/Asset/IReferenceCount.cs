namespace Core.FS
{
    /// <summary>
    /// 异步操作持有者
    /// </summary>
    public interface IReferenceCount
    {
        /// <summary>
        /// 增加引用计数
        /// </summary>
        void AddRef();

        /// <summary>
        /// 减少引用计数
        /// </summary>
        void DecRef();

        /// <summary>
        /// 减少多少引用计数
        /// </summary>
        /// <param name="count"></param>
        void DecRefCount(int count);
    }
}