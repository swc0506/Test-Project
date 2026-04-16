using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace CoreEditor.Tools
{
    public abstract class PostTextAsset
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);

        /// <summary>
        /// 对textAsset导入的后处理
        /// </summary>
        /// <param name="text"></param>
        public virtual void OnPostprocessTextAsset(string path, TextAsset text)
        {
        }
    }
}