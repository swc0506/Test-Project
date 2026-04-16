using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public abstract class PostAudio
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);


        /// <summary>
        /// 对声音导入的前处理
        /// </summary>
        /// <param name="importer"></param>
        public virtual void OnPreprocessAudio(AudioImporter importer)
        {
        }

        /// <summary>
        /// 对声音导入的后处理
        /// </summary>
        /// <param name="audioClip"></param>
        public virtual void OnPostprocessAudio(AudioClip audioClip, AudioImporter importer)
        {
        }
    }
}