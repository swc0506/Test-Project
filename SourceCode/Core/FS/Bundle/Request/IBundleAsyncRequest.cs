using System;

namespace Core.FS
{
    public interface IBundleAsyncRequest : IComparable<IBundleAsyncRequest>, IEquatable<IBundleAsyncRequest>, IClearable
    {
        /// <summary>
        /// 请求路径
        /// </summary>
        string Path { get; }

        AssetBundleAsyncOperation AsyncOperation { get; }

        /// <summary>
        /// 请求加载资源
        /// </summary>
        void Request();

        /// <summary>get
        /// 载依赖是否已经都准备好了 
        /// </summary>
        bool IsReady { get; }

        string[] Dependencies { get; }

        /// <summary>
        /// 是否是子依赖循环请求
        /// </summary>
        bool IsCycleRequest { get; }

        /// <summary>
        /// 增加子依赖
        /// </summary>
        /// <param name="dependent"></param>
        void AddDependent(IBundleAsyncRequest dependent, string[] dependencies);

        bool ContainsDependent(string path);
        
        /// <summary>
        /// 记录循请求 让父级加载成功后处理依赖计数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dependencies"></param>
        void MarkCycleRequest(string path, string[] dependencies);

        /// <summary>
        /// 记录循环加载的依赖引用计数
        /// </summary>
        void RetainCycleRequest();
    }
}