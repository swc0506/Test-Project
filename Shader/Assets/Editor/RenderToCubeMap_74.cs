using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class RenderToCubeMap_74 : EditorWindow
    {
        private GameObject obj;
        private Cubemap cubemap;
        
        [MenuItem("立方体纹理动态生成/打开生成窗口")]
        static void Open()
        {
            var window = GetWindow<RenderToCubeMap_74>("立方体纹理动态生成");
            window.Show();
        }
        
        private void OnGUI()
        {
            GUILayout.Label("关联对应位置对象");
            obj = EditorGUILayout.ObjectField(obj, typeof(GameObject), true) as GameObject;
            GUILayout.Label("关联立方体纹理");
            cubemap = EditorGUILayout.ObjectField(cubemap, typeof(Cubemap), false) as Cubemap;

            if (GUILayout.Button("生成立方体纹理"))
            {
                RenderCubeMap();
            }
        }
        
        private void RenderCubeMap()
        {
            if (!obj || !cubemap)
            {
                EditorUtility.DisplayDialog("错误", "请关联对应位置对象和立方体纹理", "确定");
                return;
            }
            
            //动态生成临时对象，渲染到立方体纹理
            GameObject tmpObj = new GameObject("临时对象");
            tmpObj.transform.position = obj.transform.position;
            Camera tmpCamera = tmpObj.AddComponent<Camera>();
            tmpCamera.RenderToCubemap(cubemap);
            DestroyImmediate(tmpObj);
        }
    }
}
