using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public class ReplaceMaterialShaderWindow : EditorWindow
    {
        private GUILayoutOption width;
        private GUILayoutOption height;

        private Object findTarget;
        private List<Object> assets;
        private string fromShader = "Mobile/Diffuse";
        private string toShader = "Diffuse";
        private string fixShader = "Diffuse";
        private string errorShader = "Hidden/InternalErrorShader";

        [MenuItem("Tools/Assets Optimize/Replace Material Shader Window", false, 5012)]
        private static void ShowWindow()
        {
            var window = GetWindow<ReplaceMaterialShaderWindow>();
            window.titleContent = new GUIContent("Replace Material Shader Window");
            window.Show();
        }

        private void OnEnable()
        {
            width = GUILayout.ExpandWidth(true);
            height = GUILayout.Height(50);
        }

        private void OnGUI()
        {
            findTarget = EditorGUILayout.ObjectField("Find Target:", findTarget, typeof(Object), false);
            if (null == findTarget)
            {
                findTarget = AssetDatabase.LoadMainAssetAtPath("Assets");
            }

            GUILayout.Space(10);
            GUILayout.BeginVertical("Box");
            fromShader = EditorGUILayout.TextField("From Shader:", fromShader);
            toShader = EditorGUILayout.TextField("To Shader:", toShader);
            if (GUILayout.Button("Replace Materials Shader", width, height))
            {
                assets = AssetUtils.GetAssets(findTarget, ".mat");
                ProcessMaterialShader(assets, fromShader, toShader);
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical("Box");
            fixShader = EditorGUILayout.TextField("Fix Shader:", fixShader);
            if (GUILayout.Button("Fix Materials Error Shader", width, height))
            {
                assets = AssetUtils.GetAssets(findTarget, ".mat");
                ProcessMaterialShader(assets, errorShader, fixShader);
            }

            GUILayout.EndVertical();
        }

        private void ProcessMaterialShader(List<Object> assets, string fromShader, string toShader)
        {
            foreach (var item in assets)
            {
                if (item is Material mat)
                {
                    if (mat.shader.name == fromShader)
                    {
                        ReplaceMaterialShader(mat, toShader);
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }

        private void ReplaceMaterialShader(Material mat, string toShader)
        {
            Shader shader = Shader.Find(toShader);
            if (null != shader)
            {
                mat.shader = shader;
                EditorUtility.SetDirty(mat);
            }
        }
    }
}