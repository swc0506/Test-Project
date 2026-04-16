using System;
using UnityEngine;

namespace Core
{
    public class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get { return GetInstance(); }
        }

        public static T GetInstance()
        {
            if (null == instance)
            {
                new GameObject(typeof(T).Name).AddComponent<T>();
            }

            return instance;
        }

        private void Awake()
        {
            if (null == instance)
            {
                instance = gameObject.GetComponent<T>();
                instance.OnInitial();
            }

            GameObject.DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 单利被初始创建时调用
        /// </summary>
        protected virtual void OnInitial()
        {
        }

        private void OnDestroy()
        {
            OnDispose();
            instance = null;
        }

        /// <summary>
        /// 单利被销毁时调用
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        public void Dispose()
        {
            GameObject.Destroy(gameObject);
        }
    }
}