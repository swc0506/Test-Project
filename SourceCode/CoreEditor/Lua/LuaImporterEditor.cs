#if LUA
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace CoreEditor.Lua
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LuaImporter))]
    public class LuaImporterEditor : ScriptedImporterEditor
    {
        private GUIStyle titleStyle;

        public override void OnInspectorGUI()
        {
            Rect lastRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 32);
            Rect rect = new Rect(lastRect.x, lastRect.y + 16f, 32f, 32f);
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(LuaAssetInspector.ICON_PATH);
            if (null != icon)
            {
                GUI.Label(rect, icon);
            }

            rect.x += 36;
            rect.width = EditorGUIUtility.currentViewWidth;
            if (null == titleStyle)
            {
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontSize = 16;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.normal.textColor = new Color32(0, 162, 248, 255);
            }

            GUI.Label(rect, "Lua Script", titleStyle);

            ApplyRevertGUI();
            
        }
    }
}
#endif