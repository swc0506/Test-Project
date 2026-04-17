using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    /// <summary>
    /// 资源下载线程
    /// </summary>
    public class DownLoadThread
    {
        private Action<DownLoadThread, HotFileInfo> onDownLoadSuccess;
        public Action<DownLoadThread, HotFileInfo> onDownLoadFailed;
        
        private HotAssetsModule mAssetsModule;
        private HotFileInfo mHotFileInfo;
        private string mDownLoadUrl;
        private string mHotAssetsSavePath;
        private float mDownLoadSize;
        //当前下载次数
        private int curDownLoadCount;
        private const int MAX_TRY_DOWNLOAD_COUNT = 3;
        
        /// <summary>
        /// 资源下载线程
        /// </summary>
        /// <param name ="assetsModule">资源模块</param>
        /// <param name="hotFileInfo">需要下载的热更资源</param>
        /// <param name="downLoadUrl">资源下载地址</param>
        /// <param name="hotAssetsSavePath">文件储存路径</param>
        public DownLoadThread(HotAssetsModule assetsModule, HotFileInfo hotFileInfo, string downLoadUrl, string hotAssetsSavePath)
        {
            mAssetsModule = assetsModule;
            mHotFileInfo = hotFileInfo;
            mDownLoadUrl = downLoadUrl + "/" + hotFileInfo.abName;
            mHotAssetsSavePath = hotAssetsSavePath + "/" + hotFileInfo.abName;;
        }
        
        /// <summary>
        /// 开始通过子线程下载资源
        /// </summary>
        /// <param name="downLoadSuccess"></param>
        /// <param name="downLoadFailed"></param>
        public void StartDownLoad(Action<DownLoadThread, HotFileInfo> downLoadSuccess, Action<DownLoadThread, HotFileInfo> downLoadFailed)
        {
            curDownLoadCount++;
            onDownLoadSuccess = downLoadSuccess;
            onDownLoadFailed = downLoadFailed;

            Task.Run(() =>
            {
                //这里的代码在子线程中执行 子线程中不要使用Unity的API
                try
                {
                    Debug.Log("开始下载资源：" + mHotFileInfo.abName + " url:" + mDownLoadUrl);
                    HttpWebRequest request = WebRequest.Create(mDownLoadUrl) as HttpWebRequest;
                    request.Method = "GET";
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    
                    //创建本地文件流
                    FileStream fileStream = File.Create(mHotAssetsSavePath);
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream.Length == 0)
                        {
                            Debug.LogError("下载资源为空：" + mHotFileInfo.abName + " url:" + mDownLoadUrl);
                        }
                        byte[] buffer = new byte[512];
                        // 从字节流中 读取数据
                        int size = stream.Read(buffer, 0, buffer.Length);

                        while (size > 0)
                        {
                            fileStream.Write(buffer, 0, size);
                            size = stream.Read(buffer, 0, buffer.Length);
                            //1mb = 1024kb 1kb = 1024b
                            mDownLoadSize += size;
                            mAssetsModule.AssetsDownLoadSizeM += size / 1024f / 1024f;
                        }
                        fileStream.Dispose();
                        fileStream.Close();
                        Debug.Log("下载资源成功：" + mHotFileInfo.abName + " url:" + mDownLoadUrl + " SavePath:" + mHotAssetsSavePath + " Size:" + mDownLoadSize + "M");
                        onDownLoadSuccess?.Invoke(this, mHotFileInfo);
                    }
                }
                catch (Exception e)
                {
                    if (curDownLoadCount > MAX_TRY_DOWNLOAD_COUNT)
                    { 
                        Debug.LogError("下载资源失败：" + mHotFileInfo.abName + " url:" + mDownLoadUrl + " Exception:" + e);
                        onDownLoadFailed?.Invoke(this, mHotFileInfo);
                    }
                    else
                    {
                        Debug.LogError("下载资源失败：" + mHotFileInfo.abName + " url:" + mDownLoadUrl + " 重新下载");
                        StartDownLoad(onDownLoadSuccess, onDownLoadFailed);
                    }
                    throw;
                }
            });
        }
    }
}