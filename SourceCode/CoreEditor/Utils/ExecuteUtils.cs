using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CoreEditor
{
    public class ExecuteUtils
    {
        class ExecuteArgs
        {
            public string cmd;
            public string args;
            public Action<string> callback;
        }

        public static void ExecuteScript(string path, string args, out string output)
        {
            output = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            StringBuilder strBuilder = new StringBuilder();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = path + ".bat";
                string dirName = Path.GetDirectoryName(path);
                strBuilder.Append(dirName.Substring(0, 2));
                strBuilder.Append("&&");
                strBuilder.AppendFormat("cd {0}", dirName);
                strBuilder.Append("&&");
                string batFile = Path.GetFileName(path);
                strBuilder.Append(batFile);
                if (!string.IsNullOrEmpty(args))
                {
                    strBuilder.Append(" ");
                    strBuilder.Append(args);
                }
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                path = path + ".sh";
                strBuilder.Append($"chmod +x {path}");
                strBuilder.Append("&&");
                strBuilder.Append(path);
                if (!string.IsNullOrEmpty(args))
                {
                    strBuilder.Append(" ");
                    strBuilder.Append(args);
                }
            }

            string cmd = strBuilder.ToString();
            ExecuteCmd(cmd, out output);
        }

        public static void ExecuteScript(string path, out string output)
        {
            ExecuteScript(path, string.Empty, out output);
        }

        public static void ExecuteCmd(string cmd, out string output)
        {
            output = string.Empty;
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            string fileName = "";
            string args = "";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                fileName = "cmd.exe";
                args = $"/c \"{cmd}\"";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                fileName = "/bin/bash";
                args = $"-c \"{cmd}\"";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                output = string.Empty;
                Debug.LogWarning(e.Message);
            }
        }


        public static void AsyncExecuteScript(string path, string args, Action<string> callback)
        {
            ExecuteArgs executeArgs = new ExecuteArgs()
            {
                cmd = path,
                args = args,
                callback = callback
            };
            ThreadPool.QueueUserWorkItem(DoAsyncExecuteBat, executeArgs);
        }

        private static void DoAsyncExecuteBat(object args)
        {
            ExecuteArgs executeArgs = (ExecuteArgs)args;
            ExecuteScript(executeArgs.cmd, executeArgs.args, out string output);
            executeArgs.callback?.Invoke(output);
        }

        public static void AsyncExecuteScript(string path, Action<string> callback)
        {
            AsyncExecuteScript(path, string.Empty, callback);
        }

        public static void AsyncExecuteCmd(string cmd, Action<string> callback)
        {
            ExecuteArgs executeArgs = new ExecuteArgs()
            {
                cmd = cmd,
                callback = callback
            };
            ThreadPool.QueueUserWorkItem(DoAsyncExecuteCmd, executeArgs);
        }

        private static void DoAsyncExecuteCmd(object args)
        {
            ExecuteArgs executeArgs = (ExecuteArgs)args;
            ExecuteCmd(executeArgs.cmd, out string output);
            executeArgs.callback?.Invoke(output);
        }
    }
}