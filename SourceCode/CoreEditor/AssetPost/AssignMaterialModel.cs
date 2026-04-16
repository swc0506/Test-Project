using UnityEngine;

namespace CoreEditor.Tools
{
    public abstract class AssignMaterialModel
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool IsProcess(string path);

        /// <summary>
        /// 赋值模型材质
        /// </summary>
        /// <param name="previousMaterial"></param>
        /// <param name="renderer"></param>
        /// <returns></returns>
        public abstract Material OnAssignMaterialModel(Material previousMaterial, Renderer renderer);
    }
}