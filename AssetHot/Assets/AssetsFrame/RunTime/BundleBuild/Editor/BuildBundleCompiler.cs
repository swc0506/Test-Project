using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.AssetsFrameWork
{
    public enum BundleType
    {
        AssetBundle,
        HotPatch,
    }

    public class BuildBundleCompiler
    {
        private static string mUpdateNotice;
        private static int mHotPatchVersion;
        private static BundleType mBundleType;
        private static BundleModuleData mBundleModuleData;
        private static List<string> mAllBundlePathList = new List<string>();
        private static Dictionary<string, List<string>> mAllFolderPathDic = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> mAllPrefabPathDic = new Dictionary<string, List<string>>();
        
        /// <summary>
        /// 打包AssetBundle
        /// </summary>
        /// <param name="data"> 模块数据 </param>
        /// <param name="type"> 打包类型 </param>
        /// <param name="hotPatchVersion"> 热更新补丁版本号 </param>
        /// <param name="updateNotice"> 热更新补丁更新提示 </param>
        public static void BuildAssetBundle(BundleModuleData data, BundleType type, int hotPatchVersion = 0,
            string updateNotice = null)
        {
            // 初始化打包数据
            Init(data, type, hotPatchVersion, updateNotice);
            // 打包所有文件夹
            BuildAllFolder();
            // 打包根目录下的所有文件夹
            BuildRootSubFolder();
            // 打包指定文件下的所有预制体
            BuildAllPrefabs();
            // 打包所有的AssetBundle
            BuildAllAssetBundle();
        }

        /// <summary>
        /// 初始化打包数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="hotPatchVersion"></param>
        /// <param name="updateNotice"></param>
        private static void Init(BundleModuleData data, BundleType type, int hotPatchVersion = 0, string updateNotice = null)
        {
            mAllBundlePathList.Clear();
            mAllFolderPathDic.Clear();
            mAllPrefabPathDic.Clear();
            
            mBundleModuleData = data;
            mBundleType = type;
            mHotPatchVersion = hotPatchVersion;
            mUpdateNotice = updateNotice;
            
        }

        /// <summary>
        /// 打包所有文件夹
        /// </summary>
        public static void BuildAllFolder()
        {
            
        }
        
        /// <summary>
        /// 打包根目录下的所有文件夹
        /// </summary>
        public static void BuildRootSubFolder()
        {
            
        }
        
        /// <summary>
        /// 打包指定文件下的所有预制体
        /// </summary>
        public static void BuildAllPrefabs()
        {
            
        }
        
        /// <summary>
        /// 打包所有的AssetBundle
        /// </summary>
        public static void BuildAllAssetBundle()
        {
            
        }
    }
}