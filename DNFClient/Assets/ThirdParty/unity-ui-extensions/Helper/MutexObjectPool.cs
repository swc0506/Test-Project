using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePool
{
    public class MutexObjectPool<T> where T : new()
    {
        ObjectPool<T> pool = new ObjectPool<T>(null, null);
        //Mutex poolMutex = new Mutex();
        System.Object poolLockObj = new System.Object();
        // 最多缓存obj数量
        int maxCacheObjNum = 10;
        // 缓存的obj数量
        int cachedObjNum = 0;

        public void Init(int maxCacheObjNum_)
        {
            maxCacheObjNum = maxCacheObjNum_;
        }

        public int GetCachedNum()
        {
            return cachedObjNum;
        }

        public int GetMaxCacheObjNum()
        {
            return maxCacheObjNum;
        }
        
        // 获取一个对象
        public T Get()
        {
            //poolMutex.WaitOne();
            T obj;
            lock (poolLockObj)
            {
                obj = pool.Get();
            }

            if (cachedObjNum > 0)
            {
                cachedObjNum--;
            }

            //poolMutex.ReleaseMutex();
            return obj;
        }

        // 释放对象
        public void Release(T obj)
        {
            if (GetCachedNum() >= GetMaxCacheObjNum())
            {
                return;
            }

            //poolMutex.WaitOne();
            lock (poolLockObj)
            {
                pool.Release(obj);
            }
            cachedObjNum++;
            //poolMutex.ReleaseMutex();
        }
    }

}