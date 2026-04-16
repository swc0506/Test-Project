using UnityEditor;
using System;
using System.Text.RegularExpressions;
using Core.Log;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CoreEditor.Tools
{
    static class OpenDebugScriptFile
    {
        private struct ScriptInfo
        {
            public string name;
            public int instanceID;
        }

        private static string debuggerName;
        private static ScriptInfo debugAppenderInfo;
        private static ScriptInfo unityLogInfo;

        [InitializeOnLoadMethod]
        static void InitialDebugScript()
        {
            if (string.IsNullOrEmpty(debuggerName))
            {
                debuggerName = typeof(Core.Logger).Name;
                FindScriptInfo(typeof(UnityConsoleAppender).Name, ref debugAppenderInfo);
                FindScriptInfo(typeof(UnityLogger).Name, ref unityLogInfo);
            }
        }

        private static void FindScriptInfo(string name, ref ScriptInfo info)
        {
            string[] scripts = AssetDatabase.FindAssets(name);
            foreach (var item in scripts)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (null != script)
                {
                    info.instanceID = script.GetInstanceID();
                    info.name = name + ".cs";
                }
            }
        }

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (instanceID != debugAppenderInfo.instanceID &&
                instanceID != unityLogInfo.instanceID)
            {
                return false;
            }

            string stackTrace = EditorGUIUtils.GetConsoleStackTrace();
            if (string.IsNullOrEmpty(stackTrace))
            {
                return false;
            }

            MatchCollection matches = Regex.Matches(stackTrace, @"\(at Assets/(.+)\)");
            for (int i = 0; i < matches.Count; i++)
            {
                int matchIndex = i;
                if (matches[i].Value.Contains(debugAppenderInfo.name)) //match UnityDebugAppender
                {
                    matchIndex = i + 2;
                }
                else if (matches[i].Value.Contains(unityLogInfo.name)) //match UnityLogger
                {
                    matchIndex = i + 1;
                }

                if (matches[matchIndex].Value.Contains(String.Format("/{0}.cs", debuggerName)))
                {
                    matchIndex++;
                }

                if (i == matches.Count - 3 && matches[i].Value.Contains(debugAppenderInfo.name))
                {
                    matchIndex = 0;
                }

                if (matchIndex != i)
                {
                    string atText = matches[matchIndex].Groups[1].Value;
                    OpenScriptFile(atText);
                    return true;
                }
            }

            return false;
        }


        private static void OpenScriptFile(string atText)
        {
            int splitIdx = atText.LastIndexOf(":");
            if (splitIdx >= 0 && Int32.TryParse(atText.Substring(splitIdx + 1), out int line))
            {
                string path = "Assets/" + atText.Substring(0, splitIdx);
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(path), line);
            }
        }
    }
}