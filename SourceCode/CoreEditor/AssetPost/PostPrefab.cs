using UnityEngine;

namespace CoreEditor.Tools
{
    public abstract class PostPrefab
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);

        /// <summary>
        /// 对prefab导入的后处理
        /// </summary>
        /// <param name="go"></param>
        public virtual void OnPostprocessPrefab(string path, GameObject go)
        {
        }
    }
}