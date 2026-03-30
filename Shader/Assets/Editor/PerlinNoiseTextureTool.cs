using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PerlinNoiseTextureTool : EditorWindow
    {
        private int textureWidth = 512;
        private int textureHeight = 512;
        private int scale = 20;
        private string textureName = "PerlinNoiseTexture";
        
        [MenuItem("柏林噪声纹理生成工具/打开")]
        public static void ShowWindow()
        {
            GetWindow<PerlinNoiseTextureTool>("柏林噪声纹理生成工具");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("柏林噪声纹理生成工具");
            textureWidth = EditorGUILayout.IntField("纹理宽度", textureWidth);
            textureHeight = EditorGUILayout.IntField("纹理高度", textureHeight);
            scale = EditorGUILayout.IntField("缩放", scale);
            textureName = EditorGUILayout.TextField("纹理名称", textureName);
            
            if (GUILayout.Button("生成纹理"))
            {
                GeneratePerlinNoiseTexture();
            }
        }
        
        private void GeneratePerlinNoiseTexture()
        {
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    float noiseValue = Mathf.PerlinNoise((float)x/textureWidth * scale, (float)y/textureHeight * scale);
                    texture.SetPixel(x, y, new Color(noiseValue, noiseValue, noiseValue));
                }
            }
            
            texture.Apply();
            
            File.WriteAllBytes("Assets/Art/Noise/" + textureName + ".png", texture.EncodeToPNG());
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("提示", "纹理已生成", "确定");
        }
    }
}