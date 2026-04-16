#if HYBRIDCLR
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Core;
using UnityEditor;

namespace CoreEditor.HybridCLR
{
    public class AddCodeProjectsToSolution
    {
        private const string SLN_PATTERN = @"Project\(""{([0-9A-F\-]+)}""\)";
        private const int INTERVAL = 10;

        private static int elapseFrame;
        private static bool isSlnDirty;
        private static bool isSelfSave;
        private static string slnPath;
        private static string slnGUID;

        private static HybridCLRSettingsObject settings;

        [InitializeOnLoadMethod]
        private static void Initial()
        {
            settings = HybridCLRSettingsObject.Get();
            if (!settings.addCodeProjectsToSolution)
            {
                return;
            }

            EditorApplication.update += OnUpdate;
            FindSolutionPath();
            WatchSolution();
        }

        private static void OnUpdate()
        {
            if (!settings.addCodeProjectsToSolution)
            {
                return;
            }

            if (++elapseFrame >= INTERVAL)
            {
                elapseFrame -= INTERVAL;
                if (isSlnDirty)
                {
                    AddProjectToSolution();
                }
            }
        }

        private static void FindSolutionPath()
        {
            string[] slns = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln");
            if (null != slns && slns.Length == 1)
            {
                slnPath = slns[0];
            }

            isSlnDirty = true;
        }

        private static void WatchSolution()
        {
            FileWatcherUtils.AddWatcher(Directory.GetCurrentDirectory(), "*.sln", OnRefreshSolution);
        }

        private static void OnRefreshSolution(string path, FileSystemEventArgs args)
        {
            if (isSelfSave)
            {
                isSelfSave = false;
            }
            else
            {
                isSlnDirty = true;
            }
        }

        private static string ComputeGuidHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash).ToString().ToUpper();
            }
        }

        private static string ToSlnString(string project)
        {
            string name = Path.GetFileNameWithoutExtension(project);
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Project(\"{{{0}}}\") = ", slnGUID);
            strBuilder.AppendFormat("\"{0}\"", name);
            strBuilder.AppendFormat(", \"{0}\"", project);
            strBuilder.AppendFormat(", \"{{{0}}}\"", ComputeGuidHash(project));
            strBuilder.AppendLine();
            strBuilder.Append("EndProject");
            return strBuilder.ToString();
        }

        private static string ToCompileString(string project)
        {
            string guid = ComputeGuidHash(project);
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("\t");
            strBuilder.Append("\t");
            strBuilder.AppendFormat("{{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU", guid);
            strBuilder.AppendLine();
            strBuilder.Append("\t");
            strBuilder.Append("\t");
            strBuilder.AppendFormat("{{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU", guid);
            strBuilder.AppendLine();
            strBuilder.Append("\t");
            strBuilder.Append("\t");
            strBuilder.AppendFormat("{{{0}}}.Debug|Any CPU.ActiveCfg = Release|Any CPU", guid);
            strBuilder.AppendLine();
            strBuilder.Append("\t");
            strBuilder.Append("\t");
            strBuilder.AppendFormat("{{{0}}}.Debug|Any CPU.Build.0 = Release|Any CPU", guid);
            return strBuilder.ToString();
        }

        private static string GetSlnString()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in settings.projects)
            {
                strBuilder.AppendLine();
                strBuilder.Append(ToSlnString(item));
            }

            return strBuilder.ToString();
        }

        private static string GetCompileString()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in settings.projects)
            {
                strBuilder.Append(ToCompileString(item));
                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }


        private static void AddProjectToSolution()
        {
            isSlnDirty = false;
            if (string.IsNullOrEmpty(slnPath))
            {
                return;
            }

            string context = FileUtils.ReadText(slnPath);
            var projects = settings.projects;
            if (projects.Count == 0 || context.Contains(projects[0]))
            {
                return;
            }

            if (string.IsNullOrEmpty(slnGUID))
            {
                MatchCollection matches = Regex.Matches(context, SLN_PATTERN);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        slnGUID = match.Groups[1].Value;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(slnGUID))
            {
                return;
            }

            int index = context.LastIndexOf("EndProject");
            string slnStr = GetSlnString();
            if (index > 0 && !context.Contains(slnStr))
            {
                context = context.Insert(index + 10, slnStr);

                index = context.LastIndexOf("GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                if (index > 0)
                {
                    index = context.IndexOf("\tEndGlobalSection", index);
                    if (index > 0)
                    {
                        context = context.Insert(index, GetCompileString());
                    }
                }
            }

            isSelfSave = true;
            FileUtils.CreateFile(slnPath, context);
        }
    }
}
#endif