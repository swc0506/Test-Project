using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public abstract class PostTexture
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);

        /// <summary>
        /// 对纹理导入的前处理
        /// </summary>
        /// <param name="importer"></param>
        public virtual void OnPreprocessTexture(TextureImporter importer)
        {
        }

        /// <summary>
        /// 对纹理导入的后处理
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="importer"></param>
        public virtual void OnPostprocessTexture(Texture2D texture, TextureImporter importer)
        {
        }

        /// <summary>
        ///  对精灵导入的后处理
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="sprites"></param>
        /// <param name="importer"></param>
        public virtual void OnPostprocessSprites(Texture2D texture, Sprite[] sprites, TextureImporter importer)
        {
        }
    }
}