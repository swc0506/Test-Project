using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ResourcesProvider : IGameObjectProvider
    {
        private static ResourcesProvider instance;

        public static ResourcesProvider Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new ResourcesProvider();
                }

                return instance;
            }
        }

        private AtomicInt createId = new AtomicInt();
        private HashSet<int> actions = new HashSet<int>();

        public GameObject Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                GameObject prefab = Resources.Load<GameObject>(path);
                if (null != prefab)
                {
                    GameObject go = GameObject.Instantiate(prefab);
                    return go;
                }
            }

            return null;
        }

        public int LoadAsync(string path, Action<string, GameObject> callback)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(path))
            {
                id = createId.GetAndIncrement();
                actions.Add(id);

                ResourceRequest request = Resources.LoadAsync<GameObject>(path);
                request.completed += (AsyncOperation operation) =>
                {
                    if (!actions.Remove(id))
                    {
                        return;
                    }

                    GameObject prefab = (operation as ResourceRequest).asset as GameObject;
                    if (null != prefab)
                    {
                        GameObject go = GameObject.Instantiate(prefab);
                        callback?.Invoke(path, go);
                    }
                    else
                    {
                        callback?.Invoke(path, null);
                    }
                };
            }
            else
            {
                callback?.Invoke(path, null);
            }

            return id;
        }

        public void CancelAsync(int id)
        {
            actions.Remove(id);
        }

        public void Destroy(GameObject go)
        {
            if (null != go)
            {
                GameObject.Destroy(go);
            }
        }

        public void Dispose()
        {
        }
    }
}