using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace CoreEditor
{
    public class EditorGUIUtils
    {
        private static MethodInfo setIconForObjectMethodInfo;

        public static bool DrawHeader(string text)
        {
            return DrawHeader(text, text);
        }

        /// <summary>
        /// 绘制折叠栏
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool DrawHeader(string text, string key)
        {
            GUILayout.Space(5);
            bool state = EditorPrefs.GetBool(key, true);
            if (!state)
            {
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            }

            GUILayout.BeginHorizontal();
            GUI.changed = false;
            text = "<b><size=12>" + text + "</size></b>";
            if (state)
            {
                text = "\u25BC " + text;
            }
            else
            {
                text = "\u25BA " + text;
            }

            GUIStyle style = "dragtab";
            Debug.Log(style.name);
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.Height(20), GUILayout.MinWidth(30)))
            {
                state = !state;
            }

            if (GUI.changed)
            {
                EditorPrefs.SetBool(key, state);
            }

            GUILayout.EndHorizontal();
            return state;
        }

        public static Texture GetAssetTypeIcon(Object asset)
        {
            GUIContent gc = null;
            if (null == asset)
            {
                gc = EditorGUIUtility.IconContent("Warning");
                return gc.image;
            }

            Type type = asset.GetType();
            if (type == typeof(Texture) || type == typeof(Texture2D) || type == typeof(Sprite))
            {
                Texture2D iconTex = AssetPreview.GetAssetPreview(asset);
                return iconTex;
            }

            string iconName = "Folder Icon";
            if (type == typeof(GameObject))
            {
                iconName = "Prefab Icon";
            }
            else if (type == typeof(Material))
            {
                iconName = "Material Icon";
            }
            else if (type == typeof(Shader))
            {
                iconName = "Shader Icon";
            }
            else if (type == typeof(TextAsset))
            {
                iconName = "TextAsset Icon";
            }
            else if (type == typeof(AudioClip))
            {
                iconName = "AudioClip Icon";
            }
            else if (type == typeof(VideoClip))
            {
                iconName = "VideoClip Icon";
            }
            else if (type == typeof(SceneAsset))
            {
                iconName = "SceneAsset Icon";
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                iconName = "ScriptableObject Icon";
            }

//            else if (type == typeof(Texture))
//            {
//                iconName = "Texture Icon";
//            }
//            else if (type == typeof(Texture2D))
//            {
//                iconName = "Texture2D Icon";
//            }
//            else if (type == typeof(Sprite))
//            {
//                iconName = "Sprite Icon";
//            }
            gc = EditorGUIUtility.IconContent(iconName);
            return gc.image;
        }

        public static string GetConsoleStackTrace()
        {
            System.Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            FieldInfo msFieldInfo =
                consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            EditorWindow instance = msFieldInfo.GetValue(null) as EditorWindow;
            if (null != instance && EditorWindow.focusedWindow == instance)
            {
                FieldInfo textFieldInfo =
                    consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                return textFieldInfo.GetValue(instance).ToString();
            }

            return string.Empty;
        }

        public static void SetObjectIcon(Object obj, Texture2D icon)
        {
            if (setIconForObjectMethodInfo == null)
            {
                Type type = typeof(EditorGUIUtility);
                setIconForObjectMethodInfo =
                    type.GetMethod("SetIconForObject",
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            setIconForObjectMethodInfo.Invoke(null, new object[] {obj, icon});
        }

        public static void SetObjectIcon(Object obj, string iconName)
        {
//            Texture2D icon = EditorGUIUtility.FindTexture(iconName);
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Images/lua.png");
            Debug.Log(icon + "**" + obj.name);
            SetObjectIcon(obj, icon);
        }
    }
}