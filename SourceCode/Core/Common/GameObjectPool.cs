using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GameObjectPool
    {
        //最大缓存数量
        private readonly int maxSize;

        private Dictionary<string, Queue<GameObject>> poolMap = new Dictionary<string, Queue<GameObject>>();

        private Dictionary<int, string> borrowMap = new Dictionary<int, string>();

        //GameObject的提供器
        private IGameObjectProvider provider;

        //GameObject是否可见的处理方法，
        private Action<GameObject, bool> activeHandler;


        private static GameObjectPool global;

        public static GameObjectPool Global
        {
            get
            {
                if (null == global)
                {
                    global = new GameObjectPool();
                }

                return global;
            }
        }


        public GameObjectPool(int maxSize, IGameObjectProvider provider, Action<GameObject, bool> activeHandler)
        {
            this.maxSize = maxSize;
            this.provider = provider;
            //默认使用SetActive,ugui相关的可设置scale=0，减少enable中的rebuild消耗
            if (null == activeHandler)
            {
                this.activeHandler = GameObjectUtils.SetActive;
            }
            else
            {
                this.activeHandler = activeHandler;
            }
        }

        public GameObjectPool(int maxSize, IGameObjectProvider provider) : this(maxSize, provider, null)
        {
        }

        public GameObjectPool(IGameObjectProvider provider, Action<GameObject, bool> activeHandler) : this(0, provider,
            activeHandler)
        {
        }

        public GameObjectPool(IGameObjectProvider provider) : this(provider, null)
        {
        }

        public GameObjectPool(int maxSize) : this(maxSize, ResourcesProvider.Instance)
        {
        }

        public GameObjectPool() : this(0)
        {
        }

        /// <summary>
        /// 设置GameObject的提供器
        /// </summary>
        /// <param name="provider"></param>
        public void SetProvider(IGameObjectProvider provider)
        {
            this.provider = provider;
        }

        public void SetActiveHandler(Action<GameObject, bool> activeHandler)
        {
            this.activeHandler = activeHandler;
        }

        /// <summary>
        /// 获取缓存池数量
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>数量</returns>
        public int GetPoolCount(string path)
        {
            if (!string.IsNullOrEmpty(path) && poolMap.TryGetValue(path, out var queue))
            {
                return queue.Count;
            }

            return 0;
        }

        private Queue<GameObject> GetQueue(string path)
        {
            if (!poolMap.TryGetValue(path, out var queue))
            {
                if (maxSize != 0)
                {
                    queue = new Queue<GameObject>(maxSize);
                }
                else
                {
                    queue = new Queue<GameObject>();
                }

                poolMap.Add(path, queue);
            }

            return queue;
        }

        /// <summary>
        /// 获取GameObject
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public GameObject Pop(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var queue = GetQueue(path);
            GameObject go = null;
            if (queue.Count > 0)
            {
                go = queue.Dequeue();
                activeHandler.Invoke(go, true);
            }
            else
            {
                if (null != provider)
                {
                    go = provider.Load(path);
                }
            }

            if (null != go)
            {
                borrowMap.Add(go.GetInstanceID(), path);
            }

            return go;
        }

        /// <summary>
        /// 获取GameObject组件
        /// </summary>
        /// <param name="path">路径</param>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns></returns>
        public T Pop<T>(string path) where T : Component
        {
            GameObject inst = Pop(path);
            if (null != inst)
            {
                return inst.GetComponent<T>();
            }

            return null;
        }

        public int PopAsync(string path, Action<string, GameObject> callback)
        {
            int id = 0;
            if (string.IsNullOrEmpty(path))
            {
                callback.Invoke(path, null);
                return 0;
            }

            var queue = GetQueue(path);
            if (queue.Count > 0)
            {
                GameObject go = queue.Dequeue();
                borrowMap.Add(go.GetInstanceID(), path);
                activeHandler.Invoke(go, true);
                callback?.Invoke(path, go);
            }
            else
            {
                if (null != provider)
                {
                    id = provider.LoadAsync(path, (rPath, go) =>
                    {
                        if (null != go)
                        {
                            borrowMap.Add(go.GetInstanceID(), rPath);
                        }

                        callback?.Invoke(rPath, go);
                    });
                }
                else
                {
                    callback?.Invoke(path, null);
                }
            }

            return id;
        }

        public void CancelAsync(int id)
        {
            if (null != provider)
            {
                provider.CancelAsync(id);
            }
        }

        private void DisposeObject(GameObject go)
        {
            if (null != go && null != provider)
            {
                provider.Destroy(go);
            }
        }

        /// <summary>
        /// 释放对象到缓存池
        /// </summary>
        /// <param name="go">释放对象</param>
        /// <param name="path">路径</param>
        public bool Push(GameObject go, string path)
        {
            if (null == go || string.IsNullOrEmpty(path))
            {
                return false;
            }

            borrowMap.Remove(go.GetInstanceID());
            var queue = GetQueue(path);
            if (maxSize == 0 || queue.Count < maxSize)
            {
                activeHandler.Invoke(go, false);
                queue.Enqueue(go);
                return true;
            }
            else //池子满了，直接销毁
            {
                DisposeObject(go);
                return false;
            }
        }

        /// <summary>
        /// 释放对象到缓存池
        /// </summary>
        /// <param name="go">释放对象</param>
        public void Push(GameObject go)
        {
            if (null != go && borrowMap.TryGetValue(go.GetInstanceID(), out var path))
            {
                Push(go, path);
            }
        }

        /// <summary>
        /// 向引用池中追加指定数量的go
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">数量</param>
        public void Add(string path, int count)
        {
            if (string.IsNullOrEmpty(path) || null == provider)
            {
                return;
            }

            var queue = GetQueue(path);
            if (maxSize > 0)
            {
                int maxCount = maxSize - queue.Count;
                count = count > maxCount ? maxCount : count;
            }

            for (int i = 0; i < count; i++)
            {
                GameObject go = provider.Load(path);
                if (null != go)
                {
                    activeHandler.Invoke(go, false);
                    queue.Enqueue(go);
                }
            }
        }

        public void AddAsync(string path, int count, Action<bool> callback)
        {
            if (string.IsNullOrEmpty(path) || null == provider)
            {
                callback?.Invoke(false);
            }

            var queue = GetQueue(path);
            if (maxSize > 0)
            {
                int maxCount = maxSize - queue.Count;
                count = count > maxCount ? maxCount : count;
            }

            int completedCount = 0;
            for (int i = 0; i < count; i++)
            {
                provider.LoadAsync(path, (rPath, go) =>
                {
                    bool res = go != null;
                    if (res)
                    {
                        activeHandler.Invoke(go, false);
                        queue.Enqueue(go);
                    }

                    if (++completedCount == count)
                    {
                        callback?.Invoke(res);
                    }
                });
            }
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">数量</param>
        public void Remove(string path, int count)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (poolMap.TryGetValue(path, out var queue))
            {
                count = count > queue.Count ? queue.Count : count;
                for (int i = 0; i < count; i++)
                {
                    GameObject go = queue.Dequeue();
                    borrowMap.Remove(go.GetInstanceID());
                    DisposeObject(go);
                }
            }
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <param name="path">路径</param>
        public void RemoveAll(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (poolMap.TryGetValue(path, out var queue))
            {
                int count = queue.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject go = queue.Dequeue();
                    borrowMap.Remove(go.GetInstanceID());
                    DisposeObject(go);
                }
            }
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <param name="path">路径</param>
        public void Clear()
        {
            foreach (var map in poolMap)
            {
                foreach (var item in map.Value)
                {
                    DisposeObject(item);
                }
            }

            poolMap.Clear();
            borrowMap.Clear();
        }

        /// <summary>
        /// 销毁所有缓存对象
        /// </summary>
        public void Dispose()
        {
            Clear();
            poolMap = null;
            borrowMap = null;
            if (null != provider)
            {
                provider.Dispose();
                provider = null;
            }

            activeHandler = null;
        }
    }
}