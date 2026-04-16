using System;

namespace Core.FS
{
    /// <summary>
    /// 加载资源责任链处理者
    /// </summary>
    internal interface IChainLoadHandler
    {
        /// <summary>
        /// 下一个处理者
        /// </summary>
        /// <param name="handler"></param>
        void SetNext(IChainLoadHandler handler);

        /// <summary>
        /// 获取下一个处理者
        /// </summary>
        /// <returns></returns>
        IChainLoadHandler GetNext();

        /// <summary>
        /// 请求加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        AssetObject Request(string path, Type type);

        /// <summary>
        /// 异步请求加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">类型</param>
        /// <param name="priority">优先级</param>
        /// <returns></returns>
        AssetAsyncHandler RequestAsync(string path, Type type, int priority);
    }
}