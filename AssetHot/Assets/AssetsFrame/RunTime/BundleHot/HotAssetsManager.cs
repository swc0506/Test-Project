using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class WaitDownLoadModule
    {
        public BundleModuleEnum bundleModule;
        public Action<BundleModuleEnum> startHot;
        public Action<BundleModuleEnum> hotFinish;
        public Action<BundleModuleEnum, float> hotAssetsProgressCallBack;
    }
    
    public class HotAssetsManager : IHotAssets
    {
        // 最大线程数
        private int maxThreadCount = 3;
        // 所有热更模块
        private Dictionary<BundleModuleEnum, HotAssetsModule> mAllAssetsModuleDic = new Dictionary<BundleModuleEnum, HotAssetsModule>();
        // 正在下载热更模块
        private Dictionary<BundleModuleEnum, HotAssetsModule> mDownLoadAssetsModuleDic = new Dictionary<BundleModuleEnum, HotAssetsModule>();
        // 正在下载热更模块列表
        private List<HotAssetsModule> mDownLoadAssetsModuleList = new List<HotAssetsModule>();
        // 等待下载热更模块
        private Queue<WaitDownLoadModule> mWaitDownLoadQueue = new Queue<WaitDownLoadModule>();
        // 下载AB完成
        public static Action<HotFileInfo> downLoadBundleFinish;
        

        public void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack,
            Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waitDownLoad,
            bool isCheck = true)
        {
            if (BundleSettings.Instance.bundleHotType == BundleHotEnum.NotHot)
            {
                hotFinish?.Invoke(bundleModule);
                return;
            }
            
            maxThreadCount = BundleSettings.Instance.maxThreadCount;

            HotAssetsModule assetsModule = GetOrNewAssetsModule(bundleModule);
            // 是否有闲置
            if (mDownLoadAssetsModuleDic.Count < maxThreadCount)
            {
                if (!mDownLoadAssetsModuleDic.ContainsKey(bundleModule))
                {
                    mDownLoadAssetsModuleDic.Add(bundleModule, assetsModule);
                }

                if (!mDownLoadAssetsModuleList.Contains(assetsModule))
                {
                    mDownLoadAssetsModuleList.Add(assetsModule);
                }
                assetsModule.onDownLoadAllAssetsFinish += HotModuleAssetsFinish;
                // 开始热更
                assetsModule.StartHotAssets(() =>
                {
                    MultipleThreadBalancing();
                    startHotCallBack?.Invoke(bundleModule);
                }, hotFinish);
            }
            else
            {
                waitDownLoad?.Invoke(bundleModule);
                //添加到等待下载队列
                mWaitDownLoadQueue.Enqueue(new WaitDownLoadModule()
                {
                    bundleModule = bundleModule,
                    startHot = startHotCallBack,
                    hotFinish = hotFinish,
                });
            }
        }
        
        // 获取或新建热更模块
        public HotAssetsModule GetOrNewAssetsModule(BundleModuleEnum bundleModule)
        {
            if (mAllAssetsModuleDic.TryGetValue(bundleModule, out var module))
            {
                return module;
            }

            mAllAssetsModuleDic.Add(bundleModule, new HotAssetsModule(bundleModule, FrameBase.Instance));
            return mAllAssetsModuleDic[bundleModule];
        }

        /// <summary>
        /// 检测资源版本是否需要热更
        /// </summary>
        /// <param name="bundleModule"></param>
        /// <param name="callBack"></param>
        public void CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack)
        {
            HotAssetsModule assetsModule = GetOrNewAssetsModule(bundleModule);
            assetsModule.CheckAssetsVersion((isHot, size) =>
            {
                if (!isHot)
                    AssetBundleManager.Instance.LoadAssetBundleConfig(bundleModule);
                callBack?.Invoke(isHot, size);
            });
        }

        public HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule)
        {
            if (mAllAssetsModuleDic.TryGetValue(bundleModule, out var module))
            {
                return module;
            }
            return null;
        }

        /// <summary>
        /// 主线程更新接口
        /// </summary>
        public void OnMainThreadUpdate()
        {
            for (var index = 0; index < mDownLoadAssetsModuleList.Count; index++)
            {
                var t = mDownLoadAssetsModuleList[index];
                t.OnMainThreadUpdate();
            }
        }

        /// <summary>
        /// 热更模块资源完成
        /// </summary>
        /// <param name="bundleModule"></param>
        private void HotModuleAssetsFinish(BundleModuleEnum bundleModule)
        {
            //移除已完成
            if (mDownLoadAssetsModuleDic.ContainsKey(bundleModule))
            {
                HotAssetsModule assetsModule = mDownLoadAssetsModuleDic[bundleModule];
                if (mDownLoadAssetsModuleList.Contains(assetsModule))
                {
                    mDownLoadAssetsModuleList.Remove(assetsModule);
                }
                mDownLoadAssetsModuleDic.Remove(bundleModule);
            }
            
            //判断是否有线程空闲
            if (mWaitDownLoadQueue.Count > 0)
            {
                var downLoadModule = mWaitDownLoadQueue.Dequeue();
                HotAssets(downLoadModule.bundleModule, downLoadModule.startHot, downLoadModule.hotFinish, null);
            }
            else
            {
                //在没有等待热更模块的情况下，如果还有下载中的模块，则进行负载均衡
                if (mDownLoadAssetsModuleDic.Count > 0)
                {
                    MultipleThreadBalancing();
                }
            }
            
            AssetBundleManager.Instance.LoadAssetBundleConfig(bundleModule);
        }

        /// <summary>
        /// 多线程均衡
        /// </summary>
        public void MultipleThreadBalancing()
        {
            // 获取当前正在下载热更资源模块的个数 
            int count = mDownLoadAssetsModuleDic.Count;
            
            //计算并发下载个数 向上取整
            float threadCount = maxThreadCount * 1.0f / count;
            
            //主下载线程个数
            int mainThreadCount = 0;
            //通过(int) 进行强转  (int)强转：表示向下强转
            int threadBalancingCount = (int)threadCount;

            if ((int)threadCount< threadCount)
            {
                //向上取整
                mainThreadCount = Mathf.CeilToInt(threadCount);
                //向下取整
                threadBalancingCount = Mathf.FloorToInt(threadCount);
            }
            
            //多线程均衡
            int i = 0;
            foreach (var item in mDownLoadAssetsModuleDic.Values)
            {
                if (mainThreadCount != 0 && i == 0)
                {
                    item.SetDownLoadThreadCount(mainThreadCount);
                }
                else
                {
                    item.SetDownLoadThreadCount(threadBalancingCount);
                }
                i++;
            }
        }
    }
}