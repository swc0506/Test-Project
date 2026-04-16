using System;
using System.IO;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 断点续传下载器 
    /// </summary>
    public class ResumeDownloader : Downloader
    {
        public ResumeDownloader(string url, string saveDir, long length, string md5) : base()
        {
            SetTask(url, saveDir, length, md5);
        }

        public ResumeDownloader(string url, string saveDir, string md5) : this(url, saveDir, 0, md5)
        {
        }

        public ResumeDownloader(string url, string saveDir, long length) : this(url, saveDir, 0, string.Empty)
        {
        }

        public ResumeDownloader(string url, string saveDir) : this(url, saveDir, 0, string.Empty)
        {
        }

        internal ResumeDownloader() : base()
        {
        }

        private bool TryGetCacheFile(out FileStream fileStream, out long startPos)
        {
            if (FileUtils.ExistsFile(saveTempPath))
            {
                fileStream = File.Open(saveTempPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                //缓存文件异常，删除
                if (TotalLength == 0 && fileStream.Length > TotalLength)
                {
                    fileStream.Dispose();
                    FileUtils.DeleteFile(saveTempPath);
                }
                else
                {
                    startPos = fileStream.Length;
                    fileStream.Seek(startPos, SeekOrigin.Current);
                    return true;
                }
            }

            fileStream = null;
            startPos = 0;
            return false;
        }

        protected override void StartHttpDownload()
        {
            FileStream fileStream = null;
            Stream resStream = null;
            long downloadLength = 0;
            int resType = 0;
            try
            {
                if (FileUtils.ExistsFile(savePath))
                {
                    resType = 1;
                }
                else
                {
                    if (!TryGetCacheFile(out fileStream, out long startPos))
                    {
                        string saveDir = Path.GetDirectoryName(saveTempPath);
                        FileUtils.CreateDirectory(saveDir);
                        fileStream = new FileStream(saveTempPath, FileMode.Create, FileAccess.ReadWrite,
                            FileShare.ReadWrite);
                    }

                    if (TotalLength == 0 || startPos < TotalLength)
                    {
                        webRequest = CreateWebRequest(Url);
                        SetDownloadLength(0);
                        if (startPos > 0)
                        {
                            webRequest.AddRange(startPos); //设置range
                        }

                        resStream = webRequest.GetResponse().GetResponseStream();
                        byte[] bytes = new byte[flushSize];
                        int readSize = 0;
                        while ((readSize = resStream.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            fileStream.Write(bytes, 0, readSize);

                            long totalDownload = downloadedLength + readSize;
                            SetDownloadLength(totalDownload);
                            SetCompletedLength(fileStream.Position);
                            downloadLength = fileStream.Length;
                        }

                        fileStream.Flush(true);
                    }

                    fileStream.Seek(0, SeekOrigin.Begin);
                    if (!string.IsNullOrEmpty(md5))
                    {
                        if (md5 == FileUtils.GetMD5(fileStream))
                        {
                            resType = 2;
                        }
                        else
                        {
                            resType = 3;
                        }
                    }
                    else
                    {
                        if (TotalLength == 0 || downloadLength == TotalLength)
                        {
                            resType = 2;
                        }
                        else
                        {
                            resType = 3;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WarnFormat("文件下载发生异常,Url:{0},Exception:{1},StackTrace:{2}", Url, e, e.StackTrace);
                resType = 3;
            }
            finally
            {
                if (null != webRequest)
                {
                    webRequest.Abort();
                    webRequest = null;
                }

                if (null != resStream)
                {
                    resStream.Dispose();
                    resStream = null;
                }

                if (null != fileStream)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                if (resType == 1)
                {
                    OnCompleted(true);
                }
                else if (resType == 2)
                {
                    FileUtils.MoveFile(saveTempPath, savePath);
                    OnCompleted(true);
                }
                else
                {
                    if (downloadLength >= TotalLength)
                    {
                        FileUtils.DeleteFile(saveTempPath);
                    }
                    OnCompleted(false);
                }
            }
        }
    }
}