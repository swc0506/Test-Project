using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public abstract class PostModel 
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);


        /// <summary>
        /// 对模型导入的前处理
        /// </summary>
        public virtual void OnPreprocessModel(ModelImporter importer)
        {
        }

        /// <summary>
        /// 对模型导入的后处理
        /// </summary>
        /// <param name="go"></param>
        public virtual void OnPostprocessModel(GameObject go, ModelImporter importer)
        {
        }
    }
}