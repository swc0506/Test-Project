using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ReferencePool
    {
        private readonly int maxSize;
        private Dictionary<Type, Queue<object>> poolMap = new Dictionary<Type, Queue<object>>();

        private static ReferencePool global;

        public static ReferencePool Global
        {
            get
            {
                if (null == global)
                {
                    global = new ReferencePool();
                }

                return global;
            }
        }

        public ReferencePool(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public ReferencePool() : this(0)
        {
        }

        /// <summary>
        /// 获取缓存池数量
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <returns>数量</returns>
        public int GetPoolCount(Type type)
        {
            if (null != type && poolMap.TryGetValue(type, out var queue))
            {
                return queue.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取缓存池数量
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>数量</returns>
        public int GetPoolCount<T>() where T : class
        {
            return GetPoolCount(typeof(T));
        }


        private Queue<object> GetQueue(Type type)
        {
            if (!poolMap.TryGetValue(type, out var queue))
            {
                if (maxSize != 0)
                {
                    queue = new Queue<object>(maxSize);
                }
                else
                {
                    queue = new Queue<object>();
                }

                poolMap.Add(type, queue);
            }

            return queue;
        }

        private void CheckTargetType(Type type)
        {
            if (Application.isEditor)
            {
                if (null == type)
                {
                    throw new Exception("Acquire Type Is Null");
                }

                if (!type.IsClass || type.IsAbstract)
                {
                    throw new Exception("Acquire Type Is Not A non-abstract Class Type");
                }
            }
        }


        /// <summary>
        /// 获取对象实例
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <returns></returns>
        public object Pop(Type type)
        {
            CheckTargetType(type);
            var queue = GetQueue(type);
            object inst = null;
            if (queue.Count > 0)
            {
                inst = queue.Dequeue();
            }
            else
            {
                inst = Activator.CreateInstance(type);
            }

            return inst;
        }

        /// <summary>
        /// 获取对象实例
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns></returns>
        public T Pop<T>() where T : class
        {
            return Pop(typeof(T)) as T;
        }

        private void ClearObject(object inst)
        {
            if (null != inst)
            {
                if (inst is IClearable clearable)
                {
                    clearable.Clear();
                }
                else if (inst is IList list)
                {
                    list.Clear();
                }
                else if (inst is IDictionary dictionary)
                {
                    dictionary.Clear();
                }
            }
        }

        private void DisposeObject(object inst)
        {
            if (null != inst)
            {
                ClearObject(inst);
                if (inst is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// 释放对象到缓存池
        /// </summary>
        /// <param name="inst">释放对象</param>
        public bool Push(object inst)
        {
            if (null == inst)
            {
                return false;
            }

            var queue = GetQueue(inst.GetType());
            if (maxSize == 0 || queue.Count < maxSize)
            {
                queue.Enqueue(inst);
                ClearObject(inst);
                return true;
            }
            else //池子满了，直接销毁
            {
                DisposeObject(inst);
                return false;
            }
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <param name="count">数量</param>
        public void Add(Type type, int count)
        {
            CheckTargetType(type);
            var queue = GetQueue(type);
            if (maxSize > 0)
            {
                int maxCount = maxSize - queue.Count;
                count = count > maxCount ? maxCount : count;
            }

            for (int i = 0; i < count; i++)
            {
                object inst = Activator.CreateInstance(type);
                if (null != inst)
                {
                    queue.Enqueue(inst);
                }
            }
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <param name="count">数量</param>
        /// <typeparam name="T">引用类型</typeparam>
        public void Add<T>(int count) where T : class
        {
            Add(typeof(T), count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <param name="count">数量</param>
        public void Remove(Type type, int count)
        {
            CheckTargetType(type);
            if (poolMap.TryGetValue(type, out var queue))
            {
                count = count > queue.Count ? queue.Count : count;
                for (int i = 0; i < count; i++)
                {
                    object inst = queue.Dequeue();
                    DisposeObject(inst);
                }
            }
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="count">数量</param>
        /// <typeparam name="T">引用类型</typeparam>
        public void Remove<T>(int count) where T : class
        {
            Remove(typeof(T), count);
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <param name="type">引用类型</param>
        public void RemoveAll(Type type)
        {
            CheckTargetType(type);
            if (poolMap.TryGetValue(type, out var queue))
            {
                int count = queue.Count;
                for (int i = 0; i < count; i++)
                {
                    object inst = queue.Dequeue();
                    DisposeObject(inst);
                }
            }
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public void RemoveAll<T>() where T : class, IClearable
        {
            RemoveAll(typeof(T));
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
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
        }

        /// <summary>
        /// 销毁所有缓存对象
        /// </summary>
        public void Dispose()
        {
            Clear();
            poolMap = null;
        }
    }
}