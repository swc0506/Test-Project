using System.IO;
using Core;
using UnityEditor;
using UnityEngine;

namespace CoreEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BinaryAsset))]
    public class BinaryAssetInspector : Editor
    {
        public const string ICON_PATH = "Assets/Gizmos/Core/BinaryAsset Icon.png";
        
        private GUIStyle textStyle;
        
        private string targetTitle
        {
            get
            {
                if (targets.Length == 1)
                {
                    string path = AssetDatabase.GetAssetPath(target);
                    return Path.GetFileName(path);
                }
                else
                {
                    return targets.Length + " Binary Asset";
                }
            }
        }
        
        protected override void OnHeaderGUI()
        {
            Rect lastRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 32);
            Rect rect = new Rect(lastRect.x, lastRect.y + 16f, 32f, 32f);
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH);
            if (null != icon)
            {
                GUI.Label(rect, icon);
            }
            
            rect.x += 36;
            rect.width = EditorGUIUtility.currentViewWidth;
            GUI.Label(rect, targetTitle, EditorStyles.largeLabel);
            
            GUILayout.Space(30f);
        }
        
        public override void OnInspectorGUI()
        {
            if (null == textStyle)
            {
                textStyle = (GUIStyle) "ScriptText";
            }

            bool enabled = GUI.enabled;
            GUI.enabled = true;
            string typeName = "Binary Asset";
            BinaryAsset asset = target as BinaryAsset;
            DrawContent(asset.bytes, typeName);
            GUI.enabled = enabled;
        }
        
        private void DrawContent(byte[] bytes, string typeName)
        {
            string str;
            if (targets.Length > 1)
            {
                str = string.Format("{0} {1}s", targets.Length, typeName);
            }
            else
            {
                str = string.Format("{0} bytes",bytes.Length);
            }

            Rect rect = GUILayoutUtility.GetRect(new GUIContent(str), textStyle);
            rect.x = 0.0f;
            rect.y -= 3f;
            rect.width = EditorGUIUtility.currentViewWidth + 1f;
            GUI.Box(rect, str, textStyle);
        }
      
    }

}