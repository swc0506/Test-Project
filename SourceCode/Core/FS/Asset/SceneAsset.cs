using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.FS
{
    public delegate void LoadSceneAction(SceneAsset sceneAsset);

    public class SceneAsset : ScriptableObject, IDisposable
    {
        private static Func<string, SceneAsset> createFromBuildFunc;

        public static void SetCreateFromBuildFunc(Func<string, SceneAsset> func)
        {
            createFromBuildFunc = func;
        }


        public string ScenePath { get; private set; }
        public string SceneName { get; private set; }
        public bool autoActive = true;
        public bool updateEnv = true;
        
        private Transform nodeParent = null;
        private HashSet<string> ignoreParentSet;
        private bool nodeEnable = true;
        private HashSet<string> ignoreActiveSet;
        private Scene scene;
        private LoadSceneAction completeAction;
        private List<GameObject> rootGameObjects;

        public event LoadSceneAction CompletedEvent
        {
            add { completeAction += value; }
            remove { completeAction -= value; }
        }


        public static SceneAsset Create(string path)
        {
            SceneAsset sceneAsset = CreateInstance<SceneAsset>();
            sceneAsset.ScenePath = path;
            sceneAsset.SceneName = Path.GetFileNameWithoutExtension(path);
            return sceneAsset;
        }

        public static SceneAsset CreateFromLocalFile(string path)
        {
            SceneAsset sceneAsset = null;
            if (FileUtils.ExistsFile(path))
            {
                sceneAsset = Create(path);
            }

            return sceneAsset;
        }

        public static SceneAsset CreateFromBuildSettings(string path)
        {
            SceneAsset sceneAsset = null;
            if (Application.isEditor && null != createFromBuildFunc)
            {
                sceneAsset = createFromBuildFunc.Invoke(path);
            }
            else
            {
                sceneAsset = Create(path);
            }

            return sceneAsset;
        }

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            rootGameObjects = new List<GameObject>();
        }

        public void Load(bool single)
        {
            if (!scene.isLoaded)
            {
                LoadSceneMode mode = single ? LoadSceneMode.Single : LoadSceneMode.Additive;
                SceneManager.LoadScene(SceneName, mode);
            }
        }

        public AsyncOperation LoadAsync(bool single)
        {
            if (!scene.isLoaded)
            {
                LoadSceneMode mode = single ? LoadSceneMode.Single : LoadSceneMode.Additive;
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneName, mode);
                return asyncOperation;
            }

            return null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != SceneName)
            {
                return;
            }

            this.scene = scene;
            scene.GetRootGameObjects(rootGameObjects);
            if (autoActive)
            {
                SceneManager.SetActiveScene(scene);
            }

            if (null != nodeParent)
            {
                IntervalSetNodesParent(nodeParent);
            }

            if (!nodeEnable)
            {
                nodeEnable = true;
                IntervalSetNodesEnable(nodeEnable);
            }

            completeAction?.Invoke(this);

            if (updateEnv)
            {
                DynamicGI.UpdateEnvironment();
            }
        }

        public void Unload()
        {
            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(SceneName);
            }
        }

        public void Active()
        {
            if (scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
            }
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            completeAction = null;
            Unload();
            Destroy(this);
        }

        public void SetNodesParent(Transform parent)
        {
            if (nodeParent != parent)
            {
                nodeParent = parent;
                IntervalSetNodesParent(parent);
            }
        }

        private void IntervalSetNodesParent(Transform parent)
        {
            if (!scene.isLoaded)
            {
                return;
            }

            foreach (var item in rootGameObjects)
            {
                if (null != ignoreParentSet && ignoreParentSet.Contains(item.name))
                {
                    continue;
                }

                item.transform.SetParent(parent);
            }
        }

        public void AddIgnoreSetParentNode(string name)
        {
            if (null == ignoreParentSet)
            {
                ignoreParentSet = new HashSet<string>();
            }

            ignoreParentSet.Add(name);
        }


        public void SetNodesEnable(bool enable)
        {
            if (nodeEnable != enable)
            {
                nodeEnable = enable;
                IntervalSetNodesEnable(enable);
            }
        }

        private void IntervalSetNodesEnable(bool enable)
        {
            if (!scene.isLoaded)
            {
                return;
            }

            foreach (var item in rootGameObjects)
            {
                if (null != ignoreActiveSet && ignoreActiveSet.Contains(item.name))
                {
                    continue;
                }

                item.SetActive(enable);
            }
        }

        public void AddIgnoreActiveNode(string name)
        {
            if (null == ignoreActiveSet)
            {
                ignoreActiveSet = new HashSet<string>();
            }

            ignoreActiveSet.Add(name);
        }

        public GameObject GetChildNode(string name)
        {
            foreach (var item in rootGameObjects)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 增加加载完成回调
        /// </summary>
        /// <param name="callback">回调方法</param>
        public void AddCompleted(LoadSceneAction callback)
        {
            completeAction += callback;
        }

        /// <summary>
        /// 移除加载完成回调
        /// </summary>
        /// <param name="callback">回调方法</param>
        public void RemoveCompleted(LoadSceneAction callback)
        {
            completeAction -= callback;
        }
    }
}