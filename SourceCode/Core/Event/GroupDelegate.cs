using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Event
{
    public class GroupDelegate<T> where T : Delegate
    {
        private static ReferencePool pool = new ReferencePool();

        private List<T> callbacks = new List<T>();

        internal Type GetGenericType()
        {
            return typeof(T);
        }

        public void Add(T callback)
        {
            if (null == callback)
            {
                return;
            }

            callbacks.Add(callback);
        }

        public void Remove(T callback)
        {
            if (null == callback)
            {
                return;
            }

            callbacks.Remove(callback);
        }

        public bool Contains(T callback)
        {
            return callbacks.Contains(callback);
        }

        public void ClearByCaller(object caller)
        {
            for (int i = callbacks.Count - 1; i >= 0; i--)
            {
                if (callbacks[i].Target == caller)
                {
                    callbacks.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            callbacks.Clear();
        }

        #region DynamicInvoke

        private object[] argsArr1;
        private object[] argsArr2;
        private object[] argsArr3;
        private object[] argsArr4;

        private List<T> GetTempCallbacks()
        {
            var list = pool.Pop<List<T>>();
            list.Clear();
            list.AddRange(callbacks);
            return list;
        }

        public void DynamicInvoke()
        {
            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                item.Method.Invoke(item.Target, null);
            }
            pool.Push(list);
        }

        public void DynamicInvoke(object args1)
        {
            if (null == argsArr1)
            {
                argsArr1 = new object[1];
            }

            argsArr1[0] = args1;

            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                item.Method.Invoke(item.Target, argsArr1);
            }
            pool.Push(list);
        }

        public void DynamicInvoke(object args1, object args2)
        {
            if (null == argsArr2)
            {
                argsArr2 = new object[2];
            }

            argsArr2[0] = args1;
            argsArr2[1] = args2;

            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                item.Method.Invoke(item.Target, argsArr2);
            }
            pool.Push(list);
        }

        public void DynamicInvoke(object args1, object args2, object args3)
        {
            if (null == argsArr3)
            {
                argsArr3 = new object[3];
            }

            argsArr3[0] = args1;
            argsArr3[1] = args2;
            argsArr3[2] = args3;

            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                item.Method.Invoke(item.Target, argsArr3);
            }
            pool.Push(list);
        }

        public void DynamicInvoke(object args1, object args2, object args3, object args4)
        {
            if (null == argsArr4)
            {
                argsArr4 = new object[4];
            }

            argsArr4[0] = args1;
            argsArr4[1] = args2;
            argsArr4[2] = args3;
            argsArr4[3] = args4;

            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                item.Method.Invoke(item.Target, argsArr4);
            }
            pool.Push(list);
        }

        #endregion


        #region Invoke

        public void Invoke()
        {
            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                (item as Action).Invoke();
            }
            pool.Push(list);
        }

        public void Invoke<T1>(T1 args1)
        {
            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                (item as Action<T1>).Invoke(args1);
            }
            pool.Push(list);
        }

        public void Invoke<T1, T2>(T1 args1, T2 args2)
        {
            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                (item as Action<T1, T2>).Invoke(args1, args2);
            }
            pool.Push(list);
        }

        public void Invoke<T1, T2, T3>(T1 args1, T2 args2, T3 args3)
        {
            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                (item as Action<T1, T2, T3>).Invoke(args1, args2, args3);
            }
            pool.Push(list);
        }

        public void Invoke<T1, T2, T3, T4>(T1 args1, T2 args2, T3 args3, T4 args4)
        {
            var list = GetTempCallbacks();
            foreach (var item in list)
            {
                (item as Action<T1, T2, T3, T4>).Invoke(args1, args2, args3, args4);
            }
            pool.Push(list);
        }

        #endregion
    }
}