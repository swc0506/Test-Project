using System;
using UnityEngine;

namespace Core
{
    public interface IGameObjectProvider : IDisposable
    {
        /// <summary>
        /// 加载方法
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        GameObject Load(string path);

        int LoadAsync(string path, Action<string, GameObject> callback);

        /// <summary>
        /// 关闭异步
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        void CancelAsync(int id);

        /// <summary>
        /// 卸载方法
        /// </summary>
        /// <param name="go">对象实例</param>
        void Destroy(GameObject go);
    }
}