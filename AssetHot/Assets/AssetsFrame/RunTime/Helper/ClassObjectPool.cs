using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class ClassObjectPool<T> where T : class, new()
    {
        /// <summary>
        /// 对象池 偏底层的东西尽量别用List
        /// </summary>
        protected Stack<T> mPool = new Stack<T>();
        
        /// <summary>
        /// 对象池数量
        /// </summary>
        public int Count => mPool.Count;

        /// <summary>
        /// 对象池最大数量
        /// </summary>
        protected int maxCount = 0;
        
        public ClassObjectPool(int maxCount)
        {
            this.maxCount = maxCount;
            for (int i = 0; i < maxCount; i++)
            {
                mPool.Push(new T());
            }
        }
        
        /// <summary>
        /// 取出
        /// </summary>
        /// <returns></returns>
        public T Spawn()
        {
            if (mPool.Count > 0)
            {
                return mPool.Pop();
            }
            return new T();
        }
        
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj"></param>
        public void Despawn(T obj)
        {
            if(obj != null) 
                mPool.Push(obj);
            else
                Debug.LogError("回收的对象为空");
        }
    }
}