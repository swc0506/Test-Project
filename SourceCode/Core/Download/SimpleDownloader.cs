using System;
using System.IO;

namespace Core
{
    public class SimpleDownloader : Downloader
    {
        public SimpleDownloader(string url, string saveDir, long length, string md5) : base()
        {
            SetTask(url, saveDir, length, md5);
        }

        public SimpleDownloader(string url, string saveDir, string md5) : this(url, saveDir, 0, md5)
        {
        }

        public SimpleDownloader(string url, string saveDir, long length) : this(url, saveDir, 0, string.Empty)
        {
        }

        public SimpleDownloader(string url, string saveDir) : this(url, saveDir, 0, string.Empty)
        {
        }

        internal SimpleDownloader() : base()
        {
        }

        protected override void StartHttpDownload()
        {
            FileStream fileStream = null;
            Stream resStream = null;
            int resType = 0;
            try
            {
                if (FileUtils.ExistsFile(savePath))
                {
                    resType = 1;
                }
                else
                {
                    if (!FileUtils.DeleteFile(saveTempPath))
                    {
                        string saveDir = Path.GetDirectoryName(saveTempPath);
                        FileUtils.CreateDirectory(saveDir);
                    }

                    fileStream = new FileStream(saveTempPath, FileMode.Create);

                    webRequest = CreateWebRequest(Url);
                    SetDownloadLength(0);
                    resStream = webRequest.GetResponse().GetResponseStream();
                    byte[] bytes = new byte[flushSize];
                    int readSize = 0;
                    while ((readSize = resStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        fileStream.Write(bytes, 0, readSize);

                        long totalDownload = downloadedLength + readSize;
                        SetDownloadLength(totalDownload);
                        SetCompletedLength(fileStream.Position);
                    }

                    fileStream.Flush(true);

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
                        if (TotalLength == 0 || fileStream.Length == TotalLength)
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
                    FileUtils.DeleteFile(saveTempPath);
                    OnCompleted(false);
                }
            }
        }
     
    }
}