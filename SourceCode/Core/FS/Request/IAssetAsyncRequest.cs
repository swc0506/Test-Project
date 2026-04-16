using System;

namespace Core.FS
{
    /// <summary>
    /// 资源异步请求
    /// </summary>
    public interface IAssetAsyncRequest : IComparable<IAssetAsyncRequest>, IEquatable<IAssetAsyncRequest>, IClearable
    {
        /// <summary>
        /// 请求路径
        /// </summary>
        string Path { get; }

        /// <summary>
        /// 请求加载的类型
        /// </summary>
        Type LoadType { get; }

        string LoadPath { get; }

        /// <summary>
        /// 异步操作对象
        /// </summary>
        AssetAsyncOperation AsyncOperation { get; }

        /// <summary>
        /// 请求加载资源
        /// </summary>
        void Request();
    }
}