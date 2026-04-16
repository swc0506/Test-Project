
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

namespace ZM.AssetFrameWork
{
    /// <summary>
    /// 下载事件
    /// </summary>
    public delegate void DownLoadEvent(HotFileInfo hotFileInfo);
    
    public class DownLoadEventHandler
    {
        public DownLoadEvent downLoadEvent; // 回调
        public HotFileInfo hotFileInfo;
    }
    
    /// <summary>
    /// 多线程资源下载器
    /// </summary>
    public class AssetsDownLoader
    {
        // 最大下载线程个数
        public int max_Download_Thread_Count = 3;
        // 下载地址
        private string mAssetsDownLoadUrl;
        // 热更文件储存路径
        private string mHotAssetsSavePath;
        // 资源下载队列
        private Queue<HotFileInfo> mDownLoadQueue;
        // 当前资源下载模块
        private HotAssetsModule mCurHotAssetsModule;
        // 所有下载线程
        private List<DownLoadThread> mAllDownLoadThreads = new List<DownLoadThread>();
        // 下载回调队列
        private Queue<DownLoadEventHandler> mDownLoadEventQueue = new Queue<DownLoadEventHandler>();

        
        private DownLoadEvent mDownLoadSuccess;
        private DownLoadEvent mDownLoadFailed;
        private DownLoadEvent mDownLoadFinis;
        
        /// <summary>
        /// 资源下载器
        /// </summary>
        /// <param name="assetsModule">资源下载模块</param>
        /// <param name="downLoadQueue">资源下载队列</param>
        /// <param name="downLoadUrl">资源下载地址</param>
        /// <param name="hotAssetsSavePath">热更文件储存路径</param>
        /// <param name="downLoadSuccess"></param>
        /// <param name="downLoadFailed"></param>
        /// <param name="downLoadFinis"></param>
        public AssetsDownLoader(HotAssetsModule assetsModule, Queue<HotFileInfo> downLoadQueue, string downLoadUrl,
            string hotAssetsSavePath, DownLoadEvent downLoadSuccess, DownLoadEvent downLoadFailed,
            DownLoadEvent downLoadFinis)
        {
            this.mCurHotAssetsModule = assetsModule;
            this.mDownLoadQueue = downLoadQueue;
            this.mAssetsDownLoadUrl = downLoadUrl;
            this.mHotAssetsSavePath = hotAssetsSavePath;
            this.mDownLoadSuccess = downLoadSuccess;
            this.mDownLoadFailed = downLoadFailed;
            this.mDownLoadFinis = downLoadFinis;
        }
        
        public void StartThreadDownLoadQueue()
        {
            //根据最大的线程下载个数，开启基本下载通道
            for (int i = 0; i < max_Download_Thread_Count; i++)
            {
                if (mDownLoadQueue.Count > 0)
                {
                    Debug.Log("开始下载资源 最大线程：" + max_Download_Thread_Count);
                    StartDownLoadNextBundle();
                }
            }
        }
        
        /// <summary>
        /// 开始下载一个AssetBundle
        /// </summary>
        public void StartDownLoadNextBundle()
        {
            HotFileInfo hotFileInfo = mDownLoadQueue.Dequeue();
            DownLoadThread downLoadThread =
                new DownLoadThread(mCurHotAssetsModule, hotFileInfo, mAssetsDownLoadUrl, mHotAssetsSavePath);
            downLoadThread.StartDownLoad(DownLoadSuccess, DownLoadFailed);
            mAllDownLoadThreads.Add(downLoadThread);
        }

        /// <summary>
        /// 开始加载下一个AssetBundle
        /// </summary>
        private void DownLoadNextBundle()
        {
            if (mAllDownLoadThreads.Count > max_Download_Thread_Count)
            {
                // 下载线程数量超过最大线程数量，等待下载线程完成后再继续下载
                Debug.Log("下载线程数量超过最大线程数量：" + max_Download_Thread_Count);
                return;
            }
            if (mDownLoadQueue.Count > 0)
            {
                StartDownLoadNextBundle();
                // 如果下载线程数量小于最大线程数量，则继续下载
                if (mAllDownLoadThreads.Count < max_Download_Thread_Count)
                {
                    int idleCount = max_Download_Thread_Count - mAllDownLoadThreads.Count;
                    for (int i = 0; i < idleCount; i++)
                    {
                        if (mDownLoadQueue.Count > 0)
                        {
                            StartDownLoadNextBundle();
                        }
                    }
                }
            }
            else
            {
                // 如果下载队列为空，则判断是否还有下载线程在运行，如果没有则表示下载完成
                if (mAllDownLoadThreads.Count == 0)
                {
                    TriggerCallBack(new DownLoadEventHandler
                        { downLoadEvent = mDownLoadFinis, hotFileInfo = null });
                }
            }
        }

        private void DownLoadSuccess(DownLoadThread downLoadThread, HotFileInfo hotFileInfo)
        {
            RemoveDownLoadThread(downLoadThread);
            //把回调放在主线程中调用
            TriggerCallBack(new DownLoadEventHandler
                { downLoadEvent = mDownLoadSuccess, hotFileInfo = hotFileInfo });
            DownLoadNextBundle();
        }
        
        private void DownLoadFailed(DownLoadThread downLoadThread, HotFileInfo hotFileInfo)
        {
            RemoveDownLoadThread(downLoadThread);
            TriggerCallBack(new DownLoadEventHandler
                { downLoadEvent = mDownLoadFailed, hotFileInfo = hotFileInfo });
            DownLoadNextBundle();
        }
        
        /// <summary>
        /// 在主线程触发回调
        /// </summary>
        /// <param name="handler"></param>
        private void TriggerCallBack(DownLoadEventHandler handler)
        {
            lock (mDownLoadEventQueue)
            {
                mDownLoadEventQueue.Enqueue(handler);
            }
        }
        
        /// <summary>
        /// 主线程更新接口
        /// </summary>
        public void OnMainThreadUpdate()
        {
            lock (mDownLoadEventQueue)
            {
                if (mDownLoadEventQueue.Count > 0)
                {
                    DownLoadEventHandler handler = mDownLoadEventQueue.Dequeue();
                    handler.downLoadEvent?.Invoke(handler.hotFileInfo);
                }
            }
        }

        public void RemoveDownLoadThread(DownLoadThread downLoadThread)
        {
            if (mAllDownLoadThreads.Contains(downLoadThread))
            {
                mAllDownLoadThreads.Remove(downLoadThread);
            }
        }
    }
}