#if HYBRIDCLR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.HybridCLR
{
    public class CompileCodeProjects
    {
        private static readonly object lockObject = new object();

        private static bool isFocus;
        private static readonly Dictionary<string, string> projectMap = new Dictionary<string, string>();
        private static readonly HashSet<string> dirtyProjectSet = new HashSet<string>();

        private static HybridCLRSettingsObject settings;

        private static string[] newLineChars = new string[] { "\r\n", "\n", "\r" };


        [InitializeOnLoadMethod]
        private static void Initial()
        {
            settings = HybridCLRSettingsObject.Get();
            FindProjects();

            if (settings.autoCompileCodeProjects)
            {
                EditorApplication.update += OnUpdate;
                EditorInterfaceUtils.AddFocusEvent(OnChangeFocus);
                WatchProjects();
            }
        }

        private static void OnChangeFocus(bool focus)
        {
            isFocus = focus;
        }

        private static void OnUpdate()
        {
            if (settings.autoCompileCodeProjects)
            {
                if (isFocus)
                {
                    CheckDirtyProjectSet();
                }
            }
        }

        private static void FindProjects()
        {
            foreach (var item in settings.projects)
            {
                string proName = Path.GetFileNameWithoutExtension(item);
                string dirKey = Path.GetDirectoryName(item).Replace(".", "");
                projectMap[proName] = dirKey;
            }
        }

        private static void WatchProjects()
        {
            foreach (var item in settings.projects)
            {
                string dirName = Path.GetDirectoryName(item);
                string csDir = Path.Combine(Directory.GetCurrentDirectory(), dirName);
                FileWatcherUtils.AddWatcher(csDir, "*.cs", OnRefreshProject);
            }
        }

        private static void OnRefreshProject(string path, FileSystemEventArgs args)
        {
            lock (lockObject)
            {
                foreach (var item in projectMap)
                {
                    if (args.FullPath.Contains(item.Value))
                    {
                        dirtyProjectSet.Add(item.Key);
                    }
                }
            }
        }

        private static void CheckDirtyProjectSet()
        {
            if (dirtyProjectSet.Count > 0)
            {
                lock (lockObject)
                {
                    foreach (var item in dirtyProjectSet)
                    {
                        ExecuteCompileCmd(item, false);
                    }
                }

                dirtyProjectSet.Clear();
            }
        }

        private static void ExecuteCompileCmd(string projectName, bool isRelease)
        {
            string path = null;
            foreach (var item in settings.projects)
            {
                string name = Path.GetFileNameWithoutExtension(item);
                if (name == projectName)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(item));
                    break;
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Compile C# Scripts",
                string.Format("Started compile project:{0}", projectName),
                0.6f);
            StringBuilder strBuilder = new StringBuilder();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                strBuilder.Append(path.Substring(0, 2));
                strBuilder.Append("&&");
                strBuilder.AppendFormat("cd {0}", path);
                strBuilder.Append("&&");
                if (isRelease)
                {
                    strBuilder.Append("dotnet build -c Release");
                }
                else
                {
                    strBuilder.Append("dotnet build -c Debug");
                }
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                strBuilder.AppendFormat("cd {0}", path);
                strBuilder.Append("&&");
                if (isRelease)
                {
                    strBuilder.Append("dotnet build -c Release");
                }
                else
                {
                    strBuilder.Append("dotnet build -c Debug");
                }
            }

            string cmd = strBuilder.ToString();
            ExecuteUtils.ExecuteCmd(cmd, out var output);
            if (!string.IsNullOrEmpty(output))
            {
                if (output.Contains("error"))
                {
                    string log = string.Format("Compile project fail:{0}\n{1}", projectName, FormatOutput(output));
                    CompileInfos.Push(log, LogType.Error);
                }
                else if (output.Contains("warning"))
                {
                    string log = string.Format("Compile project warning:{0}\n{1}", projectName, FormatOutput(output));
                    CompileInfos.Push(log, LogType.Warning);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private static string FormatOutput(string output)
        {
            string[] lines = output.Split(newLineChars, StringSplitOptions.None);
            if (null != lines && lines.Length > 0)
            {
                StringBuilder str = new StringBuilder();
                int count = lines.Length;
                for (int i = 0; i < count; i++)
                {
                    if (!lines[i].StartsWith(" "))
                    {
                        str.AppendLine(lines[i]);
                    }

                    if (string.IsNullOrWhiteSpace(lines[i]))
                    {
                        break;
                    }
                }

                return str.ToString();
            }

            return output;
        }

        [MenuItem("Tools/HybridCLR/Compile Code Projects(Debug) _F5", false, 30001)]
        public static void CompileDebugCodeProjects()
        {
            CompileProjects(false);
        }

        [MenuItem("Tools/HybridCLR/Compile Code Projects(Release) _F6", false, 30002)]
        public static void CompileReleaseCodeProjects()
        {
            CompileProjects(true);
        }

        public static void CompileProjects(bool isRelease)
        {
            FindProjects();
            CompileProjects(projectMap.Keys, isRelease);
        }

        public static void CompileProjects(IEnumerable<string> projects, bool isRelease)
        {
            if (null != projects)
            {
                foreach (var item in projects)
                {
                    ExecuteCompileCmd(item, isRelease);
                }
            }
        }
    }
}
#endif