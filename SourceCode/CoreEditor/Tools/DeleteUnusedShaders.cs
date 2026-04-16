using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public class DeleteUnusedShaders
    {
        private GUILayoutOption width;
        private GUILayoutOption height;


        [MenuItem("Tools/Assets Optimize/Delete Unused Shaders", false, 5013)]
        private static void DeleteShaders()
        {
            List<Object> shaders = AssetUtils.GetAssets(Application.dataPath, ".shader");
            List<Object> mats = AssetUtils.GetAssets(Application.dataPath, ".mat");

            HashSet<Shader> usedShaders = new HashSet<Shader>();
            foreach (var item in mats)
            {
                usedShaders.Add(((Material)item).shader);
            }

            HashSet<Shader> unusedShaders = new HashSet<Shader>();
            foreach (var item in shaders)
            {
                Shader shader = (Shader)item;
                if (!usedShaders.Contains(shader))
                {
                    unusedShaders.Add(shader);
                }
            }

            foreach (var item in unusedShaders)
            {
                string path = AssetDatabase.GetAssetPath(item);
                if (path.StartsWith("Assets/Shaders/"))
                {
                    Debug.LogFormat("Delete:{0}", path);
                    AssetDatabase.DeleteAsset(path);
                }
                else
                {
                    Debug.Log(path);
                }
            }
        }
    }
}