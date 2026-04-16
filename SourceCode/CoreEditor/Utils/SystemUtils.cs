using System;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace CoreEditor
{
    public static class SystemUtils
    {
        public static void ExecuteProgram(string path, string args, EventHandler exitHandler)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = path;
                info.Arguments = args;
                info.UseShellExecute = false;
                info.WindowStyle = ProcessWindowStyle.Normal;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;

                Process process = Process.Start(info);
                if (null != process)
                {
                    if (null != exitHandler)
                    {
                        process.Exited += exitHandler;
                    }

                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void ExecuteCmd(string args, Action<string> exitHandler)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.UseShellExecute = false;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.CreateNoWindow = true;
                info.StandardOutputEncoding = Encoding.UTF8;
                
                Process process = Process.Start(info);
                if (null != process)
                {
                    string cmd = args + "&exit";
                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.AutoFlush = true;
                    string output = process.StandardOutput.ReadToEnd();
                    exitHandler?.Invoke(output);
                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}