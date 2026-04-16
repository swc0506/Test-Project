#if LUA
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace CoreEditor.Lua
{
    internal enum LuaIDEType
    {
        JetBrains,
        VSCode
    }

    public class LuaIDESettingWindow : EditorWindow
    {
        [MenuItem("Tools/Lua/LuaIDE Setting Window", false, 3090)]
        public static void ShowWindow()
        {
            LuaIDESettingWindow editor = CreateInstance<LuaIDESettingWindow>();
            editor.titleContent = new GUIContent("Lua IDE Setting Window");
            editor.minSize = new Vector2(400, 300);
            editor.Show();
        }

        private static readonly string Prefs_Lua_IDE_Install_Path = "CoreEditor.Lua.Prefs_Lua_IDE_Install_Path";
        private static readonly string Prefs_Lua_Debugger_Plugins_Path = "CoreEditor.Lua.Prefs_Lua_Debugger_Plugins_Path";

        private Dictionary<string, string> openArgsMap;
        private string openArgsTip;

        private string idePath;
        private string debuggerPluginsPath;

        private LuaSettingsObject config;
        private bool hasModify;

        private SerializedObject so;
        private ReorderableList pathList;

        private void OnEnable()
        {
            openArgsMap = new Dictionary<string, string>()
            {
                {"Rider", "--line {line} {file}"},
                {"VS Code", "-g {file}:{line}"}
            };
            openArgsTip = string.Empty;
            foreach (var item in openArgsMap)
            {
                if (!string.IsNullOrEmpty(openArgsTip))
                {
                    openArgsTip += "\n";
                }

                openArgsTip += string.Format("{0}:{1}", item.Key, item.Value);
            }

            idePath = EditorPrefs.GetString(Prefs_Lua_IDE_Install_Path);
            debuggerPluginsPath = EditorPrefs.GetString(Prefs_Lua_Debugger_Plugins_Path);

            config = LuaSettingsObject.Get();
            hasModify = false;

            so = new SerializedObject(config);
            pathList = new ReorderableList(so, so.FindProperty("luaSearchDir"));
            pathList.drawHeaderCallback = DrawOrderHead;
            pathList.drawElementCallback = DrawOrderElement;
        }

        private void OnDisable()
        {
            if (hasModify)
            {
                config.Save();
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical("Box");

            GUILayout.Label("1.IDE Install Path:");
            GUILayout.TextField(idePath);
            if (GUILayout.Button("Browse", GUILayout.MinHeight(20)))
            {
                idePath = EditorUtility.OpenFilePanel("Select IDE Path", idePath, string.Empty);
                if (!string.IsNullOrEmpty(idePath))
                {
                    EditorPrefs.SetString(Prefs_Lua_IDE_Install_Path, idePath);

                    if (string.IsNullOrEmpty(config.ideOpenFileArgs))
                    {
                        config.ideOpenFileArgs = TryMatchOpenFileArgs(idePath);
                    }
                }
            }

            GUILayout.Label("2.IDE Open File Args:");
            EditorGUI.BeginChangeCheck();
            config.ideOpenFileArgs = EditorGUILayout.TextField(config.ideOpenFileArgs);
            if (EditorGUI.EndChangeCheck())
            {
                hasModify = true;
            }

            EditorGUILayout.HelpBox(openArgsTip, MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical("Box");
            EditorGUI.BeginChangeCheck();
            config.luaExtensionName = EditorGUILayout.TextField("Lua Extension Name", config.luaExtensionName);
            if (EditorGUI.EndChangeCheck())
            {
                hasModify = true;
            }

            so.Update();
            pathList.DoLayoutList();
            so.ApplyModifiedProperties();

            GUILayout.Space(20);
            GUILayout.Label("EmmyLua Debugger Plugins Path:");
            EditorGUI.BeginChangeCheck();
            debuggerPluginsPath = EditorGUILayout.TextArea(debuggerPluginsPath, GUILayout.MinHeight(60));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(Prefs_Lua_Debugger_Plugins_Path, debuggerPluginsPath);
            }

            GUILayout.EndVertical();
        }

        private string TryMatchOpenFileArgs(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                foreach (var item in openArgsMap)
                {
                    if (path.Contains(item.Key))
                    {
                        return item.Value;
                    }
                }
            }

            return string.Empty;
        }

        private void DrawOrderHead(Rect rect)
        {
            EditorGUI.LabelField(rect, "Lua Search Path");
        }

        private void DrawOrderElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            float btnWidth = 60;
            var element = pathList.serializedProperty.GetArrayElementAtIndex(index);
            string path = element.stringValue;

            rect.height = EditorGUIUtility.singleLineHeight;
            var textRect = rect;
            textRect.width -= btnWidth;

            if (!string.IsNullOrEmpty(path))
            {
                path = Application.dataPath + "/" + path;
            }

            GUI.TextField(textRect, path);

            var btnRect = rect;
            btnRect.width = btnWidth;
            btnRect.x += (textRect.width);

            if (GUI.Button(btnRect, "Browse"))
            {
                path = EditorUtility.OpenFolderPanel("Select Search Path", path, string.Empty);
                if (AssetUtils.CheckRelativeAssetsPath(path, out string relativePath))
                {
                    element.stringValue = relativePath;
                }
                else
                {
                    ShowNotification(new GUIContent("Lua Search Path Must In:" + relativePath));
                }
            }

            rect.y += 2;
        }
    }
}
#endif