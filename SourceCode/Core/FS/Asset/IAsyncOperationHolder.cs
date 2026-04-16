namespace Core.FS
{
    /// <summary>
    /// 异步操作持有者
    /// </summary>
    internal interface IAsyncOperationHolder
    {
        /// <summary>
        /// 异步操作对象
        /// </summary>
        AssetAsyncOperation AsyncOperation { get; }
    }
}