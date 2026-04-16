#if ZIP
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

namespace Core
{
    public static class ZipUtils
    {
        static ZipUtils()
        {
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
        }

        private class BaseParams
        {
            public string Password { get; private set; }
            private Action<float> progress;
            private Action<bool> complete;

            public long TotalLength { get; private set; }
            private long currLength;

            protected BaseParams(string password, Action<float> progress,
                Action<bool> complete)
            {
                this.Password = password;
                this.progress = progress;
                this.complete = complete;
            }

            public long CurrLength
            {
                get { return currLength; }
                set
                {
                    if (currLength != value)
                    {
                        currLength = value;
                        float val = (float)Math.Round((currLength / (double)TotalLength) * 100);
                        if (mainThreadCall)
                        {
                            MainThreadCaller.Call(CallProgressAction, val);
                        }
                        else
                        {
                            CallProgressAction(val);
                        }
                    }
                }
            }

            public void SetTotalLength(long totalLength)
            {
                this.TotalLength = totalLength;
                if (mainThreadCall)
                {
                    MainThreadCaller.Call(CallProgressAction, 0.0f);
                }
                else
                {
                    CallProgressAction(0);
                }
            }

            public void OnComplete(bool success)
            {
                if (mainThreadCall)
                {
                    MainThreadCaller.Call(CallCompletedAction, success);
                }
                else
                {
                    CallCompletedAction(success);
                }
            }

            private void CallCompletedAction(bool success)
            {
                complete?.Invoke(success);
            }

            private void CallProgressAction(float val)
            {
                progress?.Invoke(val);
            }
        }

        private class ZipParams : BaseParams
        {
            public string ZipPath { get; private set; }
            public string OutPath { get; private set; }
            public string RelativePath { get; private set; }

            public ZipParams(string zipPath, string outPath, string relativePath, string password,
                Action<float> progress, Action<bool> complete) : base(password, progress, complete)
            {
                this.ZipPath = zipPath;
                this.OutPath = outPath;
                this.RelativePath = relativePath;
            }
        }

        private class UnzipParams : BaseParams
        {
            public Stream InputStream { get; private set; }
            public string OutDir { get; private set; }

            public UnzipParams(Stream inputStream, string outDir, string password, Action<float> progress,
                Action<bool> complete) : base(password, progress, complete)
            {
                this.InputStream = inputStream;
                this.OutDir = outDir;
            }
        }

        public static bool workOnThread = true;
        public static bool mainThreadCall = false;

        /// <summary>
        /// 压缩文件或者文件夹
        /// </summary>
        /// <param name="zipPath">文件或者文件夹路径</param>
        /// <param name="outZipPath">输入的zip文件路径</param>
        /// <param name="relativePath">相对路径</param>
        /// <param name="password">密码</param>
        /// <param name="progress">进度回调</param>
        /// <param name="complete">完成回调</param>
        public static void Zip(string zipPath, string outZipPath, string relativePath, string password,
            Action<float> progress, Action<bool> complete)
        {
            if (string.IsNullOrEmpty(zipPath) || string.IsNullOrEmpty(outZipPath) ||
                (!Directory.Exists(zipPath) && !File.Exists(zipPath)))
            {
                return;
            }

            ZipParams zipParams = new ZipParams(zipPath, outZipPath, relativePath, password, progress, complete);
            try
            {
                if (workOnThread)
                {
                    ThreadPool.QueueUserWorkItem(DoZipFiles, zipParams);
                }
                else
                {
                    DoZipFiles(zipParams);
                }
            }
            catch (Exception e)
            {
                zipParams.OnComplete(false);
                Logger.ErrorFormat("Zip Exception,OutDirectory:{0},Exception:{1}", outZipPath, e.Message);
            }
        }

        public static void Zip(string zipPath, string outZipPath, string relativePath)
        {
            Zip(zipPath, outZipPath, relativePath, null, null, null);
        }

        private static void ZipSingleFile(string filePath, string relativePath, ZipOutputStream zipOutputStream,
            Action<long> zipSize)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                string entryPath = filePath.Substring(relativePath.Length + 1);
                ZipEntry entry = new ZipEntry(entryPath);
                FileInfo fInfo = new FileInfo(filePath);
                entry.DateTime = DateTime.Now;
                entry.Size = fInfo.Length;
                zipOutputStream.PutNextEntry(entry);

                byte[] bytes = new byte[1024 * 128];
                int readSize = 0;
                while ((readSize = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    zipOutputStream.Write(bytes, 0, readSize);
                    zipSize?.Invoke(readSize);
                }
            }
        }

        private static void DoZipFiles(object obj)
        {
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.GetEncoding("GBK").CodePage;
                
            ZipParams zipParams = (ZipParams)obj;
            string dirName = Path.GetDirectoryName(zipParams.OutPath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipParams.OutPath)))
            {
                zipOutputStream.SetLevel(7);
                zipOutputStream.Password = zipParams.Password;
                string relativePath = string.IsNullOrEmpty(zipParams.RelativePath)
                    ? Path.GetDirectoryName(zipParams.ZipPath)
                    : zipParams.RelativePath;
                FileInfo[] fileInfos = null;
                if (Directory.Exists(zipParams.ZipPath))
                {
                    fileInfos = new DirectoryInfo(zipParams.ZipPath).GetFiles("*", SearchOption.AllDirectories);
                }
                else if (File.Exists(zipParams.ZipPath))
                {
                    fileInfos = new[] { new FileInfo(zipParams.ZipPath) };
                }

                Queue<string> filePaths = new Queue<string>();
                long totalLength = 0;
                foreach (var item in fileInfos)
                {
                    filePaths.Enqueue(item.FullName);
                    totalLength += item.Length;
                }

                zipParams.SetTotalLength(totalLength);
                foreach (var info in filePaths)
                {
                    ZipSingleFile(info, relativePath, zipOutputStream,
                        (size) => { zipParams.CurrLength += size; });
                }

                zipParams.OnComplete(true);
            }
        }

        /// <summary>
        /// 解压文件流
        /// </summary>
        /// <param name="inputStream">数据流</param>
        /// <param name="outDir">输出目录</param>
        /// <param name="password">密码</param>
        /// <param name="progress">进度回调</param>
        /// <param name="completed">完成回调</param>
        public static void Unzip(Stream inputStream, string outDir, string password, Action<float> progress,
            Action<bool> completed)
        {
            if (null == inputStream || string.IsNullOrEmpty(outDir))
            {
                completed?.Invoke(false);
                return;
            }

            UnzipParams unzipParams = new UnzipParams(inputStream, outDir, password, progress, completed);
            unzipParams.SetTotalLength(inputStream.Length);
            try
            {
                if (workOnThread)
                {
                    ThreadPool.QueueUserWorkItem(DoUnzipFiles, unzipParams);
                }
                else
                {
                    DoUnzipFiles(unzipParams);
                }
            }
            catch (Exception e)
            {
                unzipParams.OnComplete(false);
                Logger.ErrorFormat("Unzip Exception : {0}", e.Message);
            }
        }

        /// <summary>
        /// 解压文件流
        /// </summary>
        /// <param name="buff">数据字节码</param>
        /// <param name="outDir">输出目录</param>
        /// <param name="password">密码</param>
        /// <param name="progress">进度回调</param>
        /// <param name="completed">完成函数</param>
        public static void Unzip(byte[] buff, string outDir, string password, Action<float> progress,
            Action<bool> completed)
        {
            if (null == buff)
            {
                completed?.Invoke(false);
                return;
            }

            Unzip(new MemoryStream(buff), outDir, password, progress, completed);
        }

        public static void Unzip(string zipPath, string outDir, string password, Action<float> progress,
            Action<bool> completed)
        {
            if (string.IsNullOrEmpty(zipPath) || !File.Exists(zipPath))
            {
                completed?.Invoke(false);
                return;
            }

            Unzip(File.OpenRead(zipPath), outDir, password, progress, completed);
        }

        private static void DoUnzipFiles(object obj)
        {
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.GetEncoding("GBK").CodePage;
            UnzipParams unzipParams = (UnzipParams)obj;
            using (ZipInputStream zipInputStream = new ZipInputStream(unzipParams.InputStream))
            {
                zipInputStream.Password = unzipParams.Password;

                ZipEntry entry;
                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    if (entry.IsDirectory || string.IsNullOrEmpty(entry.Name))
                    {
                        continue;
                    }

                    string entryPath = Path.Combine(unzipParams.OutDir, entry.Name);
                    string dirName = Path.GetDirectoryName(entryPath);
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }

                    using (FileStream fileStream = File.Create(entryPath))
                    {
                        byte[] bytes = new byte[1024 * 128];
                        int readSize = 0;
                        while ((readSize = zipInputStream.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            fileStream.Write(bytes, 0, readSize);
                            unzipParams.CurrLength = zipInputStream.Position;
                        }
                    }
                }

                unzipParams.CurrLength = unzipParams.TotalLength;
                unzipParams.OnComplete(true);
            }
        }
    }
}

#endif