using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public abstract class PostAnimation
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);

        /// <summary>
        /// 对动模型画导入的前处理
        /// </summary>
        /// <param name="importer"></param>
        public virtual void OnPreprocessModelAnimation(ModelImporter importer)
        {
        }

        /// <summary>
        /// 对模型动画导入的后处理
        /// </summary>
        /// <param name="root"></param>
        /// <param name="animationClip"></param>
        /// <param name="importer"></param>
        public virtual void OnPostprocessModelAnimation(GameObject root, AnimationClip animationClip,
            ModelImporter importer)
        {
        }

        /// <summary>
        /// 对单个动画导出处理
        /// </summary>
        /// <param name="path"></param>
        /// <param name="importer"></param>
        public virtual void OnPostprocessAnimationClip(string path, AnimationClip animationClip)
        {
        }
    }
}