using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using ZM.AssetFrameWork;

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
        private static BundleModuleEnum mBundleModuleEnum;
        private static List<string> mAllBundlePathList = new List<string>();
        private static Dictionary<string, List<string>> mAllFolderPathDic = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> mAllPrefabPathDic = new Dictionary<string, List<string>>();
        private static string BundleOutputPath =>
            Application.dataPath + "/../AssetBundle/" + mBundleModuleEnum + "/" +
            EditorUserBuildSettings.activeBuildTarget.ToString() + "/";

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
            mBundleModuleEnum = (BundleModuleEnum)Enum.Parse(typeof(BundleModuleEnum), mBundleModuleData.moduleName);
            FileHelper.DeleteFile(BundleOutputPath);
            Directory.CreateDirectory(BundleOutputPath);
        }

        /// <summary>
        /// 打包所有文件夹
        /// </summary>
        public static void BuildAllFolder()
        {
            if (mBundleModuleData.signPathArr == null || mBundleModuleData.signPathArr.Length <= 0)
                return;
            for (int i = 0; i < mBundleModuleData.signPathArr.Length; i++)
            {
                string path = mBundleModuleData.signPathArr[i].bundlePath.Replace(@"\", "/");
                if (!IsRepeat(path))
                {
                    mAllBundlePathList.Add(path);
                    string abName = GenerateBundleName(mBundleModuleData.signPathArr[i].abName);
                    if (mAllFolderPathDic.ContainsKey(abName))
                    {
                        mAllFolderPathDic.Add(abName, new List<string>(){path});
                    }
                    else
                    {
                        mAllFolderPathDic[abName].Add(path);
                    }
                }
                else
                {
                    Debug.LogError("打包文件夹路径重复:" + path);
                }
            }
        }
        
        /// <summary>
        /// 打包根目录下的所有文件夹
        /// </summary>
        public static void BuildRootSubFolder()
        {
            if (mBundleModuleData.rootFolderPathArr == null || mBundleModuleData.rootFolderPathArr.Length <= 0)
                return;

            for (int i = 0; i < mBundleModuleData.rootFolderPathArr.Length; i++)
            {
                string path = mBundleModuleData.rootFolderPathArr[i] + "/";
                string[] folderArr = Directory.GetDirectories(path);
                foreach (var item in folderArr)
                {
                    path = item.Replace(@"\", "/");
                    int nameIndex = path.LastIndexOf("/") + 1;
                    //获取文件夹同名的AB名称
                    string abName = GenerateBundleName(path.Substring(nameIndex, path.Length - nameIndex));
                    if (!IsRepeat(path))
                    {
                        mAllBundlePathList.Add(path);
                        if (!mAllFolderPathDic.ContainsKey(abName))
                        {
                            mAllFolderPathDic.Add(abName, new List<string>(){path});
                        }
                        else
                        {
                            mAllFolderPathDic[abName].Add(path);
                        }
                    }
                    else
                    {
                        Debug.LogError("打包文件夹路径重复:" + path);
                        continue;
                    }
                    
                    //处理子文件夹资源的代码
                    string[] fileArr = Directory.GetFiles(path, "*");
                    foreach (var filePath in fileArr)
                    {
                        if (!filePath.EndsWith(".meta"))
                        {
                            string abFilePath = filePath.Replace(@"\", "/");
                            if (!IsRepeat(abFilePath))
                            {
                                mAllBundlePathList.Add(abFilePath);
                                if (!mAllFolderPathDic.ContainsKey(abName))
                                {
                                    mAllFolderPathDic.Add(abName, new List<string>(){abFilePath});
                                }
                                else
                                {
                                    mAllFolderPathDic[abName].Add(abFilePath);
                                }
                            }
                            else
                            {
                                Debug.LogError("打包文件夹路径重复:" + abFilePath);
                                continue;
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 打包指定文件下的所有预制体
        /// </summary>
        public static void BuildAllPrefabs()
        {
            if (mBundleModuleData.prefabPathArr == null || mBundleModuleData.prefabPathArr.Length <= 0)
                return;
            
            //获取所有的预制体的GUID
            string[] guidArr = AssetDatabase.FindAssets("t:Prefab", mBundleModuleData.prefabPathArr);

            for (int i = 0; i < guidArr.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guidArr[i]);
                string abName = GenerateBundleName(Path.GetFileNameWithoutExtension(filePath));
                if (!mAllBundlePathList.Contains(filePath))
                {
                    //获取预制体的依赖资源
                    string[] dependencies = AssetDatabase.GetDependencies(filePath);
                    List<string> dependList = new List<string>();
                    for (int k = 0; k < dependencies.Length; k++)
                    {
                        string path = dependencies[k];
                        if (!IsRepeat(path))
                        {
                            mAllBundlePathList.Add(path);
                            dependList.Add(path);
                        }
                    }

                    if (!mAllPrefabPathDic.ContainsKey(abName))
                    {
                        mAllPrefabPathDic.Add(abName, dependList);
                    }
                    else
                    { 
                        Debug.LogError("打包预制体路径重复:" + filePath);
                    }
                }
            }
        }
        
        /// <summary>
        /// 打包所有的AssetBundle
        /// </summary>
        public static void BuildAllAssetBundle()
        {
            //修改所有要打包的文件的abName
            ModifyAllFileAbName();
            //生成一份AB配置
            GenerateABConfig();
            AssetDatabase.Refresh();
            //打包
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(BundleOutputPath,
                BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            if (manifest != null)
            {
                Debug.Log("打包完成");
            }
            else
            {
                EditorUtility.DisplayProgressBar("打包失败", "AssetBundle打包失败", 1.0f);
                Debug.LogError("打包失败");
            }
            ModifyAllFileAbName(true);
            EditorUtility.ClearProgressBar();
        }
        
        private static void ModifyAllFileAbName(bool clear = false)
        {
            int i = 0;
            foreach (var item in mAllFolderPathDic)
            {
                i++;
                EditorUtility.DisplayProgressBar("打包进度", "正在打包第" + i + "个AssetBundle", (float)i / (float)mAllFolderPathDic.Count);
                foreach (var path in item.Value)
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = clear ? string.Empty : item.Key + ".unity";
                    }
                }
            }
            
            //修改所有预制体的AssetBundleName
            i = 0;
            foreach (var item in mAllPrefabPathDic)
            {
                i++;
                List<string> bundleList = item.Value;
                EditorUtility.DisplayProgressBar("打包进度", "正在打包第" + i + "个预制体", (float)i / (float)mAllPrefabPathDic.Count);
                foreach (var path in bundleList)
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = clear ? string.Empty : item.Key + ".unity";
                    }
                }
            }

            if (clear)
            {
                string bundleConfigPath = Application.dataPath + "/" + mBundleModuleEnum.ToString().ToLower() + "assetBundleConfig.json";
                AssetImporter importer = AssetImporter.GetAtPath(bundleConfigPath.Replace(Application.dataPath, "Assets"));
                if (importer != null)
                {
                    importer.assetBundleName = string.Empty;
                }
                
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
        }
        
        private static void GenerateABConfig()
        {
            BundleConfig config = new BundleConfig();
            config.bundleInfoList = new List<BundleInfo>();
            
            Dictionary<string, string> allBundleFilePathDic = new Dictionary<string, string>();
            //获取到工程内所有的ABName
            string[] allBundleNameArr = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in allBundleNameArr)
            {
                //获取指定AB下的所有文件路径
                string[] bundleFileArr = AssetDatabase.GetAssetPathsFromAssetBundle(name);
                foreach (var filePath in bundleFileArr)
                {
                    if (!filePath.EndsWith(".cs"))
                    {
                        allBundleFilePathDic.Add(filePath, name);
                    }
                }
            }
            
            //计算AB数据， 生成配置文件
            foreach (var item in allBundleFilePathDic)
            {
                string filePath = item.Key;
                if (!filePath.EndsWith(".cs"))
                {
                    BundleInfo info = new BundleInfo();
                    info.bundleName = item.Value;
                    info.bundlePath = filePath;
                    info.assetName = Path.GetFileName(filePath);
                    info.crc = Crc32.GetCrc32(filePath);
                    info.bundleDependence = new List<string>();
                    string[] dependencies = AssetDatabase.GetDependencies(filePath);
                    foreach (var dependPath in dependencies)
                    {
                        if (!dependPath.Equals(filePath) && !dependPath.EndsWith(".cs"))
                        {
                            string assetBundleName = string.Empty;
                            if (allBundleFilePathDic.TryGetValue(dependPath, out assetBundleName))
                            {
                                //如果依赖项已经包含AssetBundle就不添加
                                if (!info.bundleDependence.Contains(assetBundleName))
                                {
                                    info.bundleDependence.Add(assetBundleName);
                                }
                            }
                        }
                    }
                    config.bundleInfoList.Add(info);
                }
            }
            // 保存配置文件
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            string bundleConfigPath = Application.dataPath + "/" + mBundleModuleEnum.ToString().ToLower() + "assetBundleConfig.json";
            StreamWriter writer = File.CreateText(bundleConfigPath);
            writer.Write(json);
            writer.Dispose();
            writer.Close();
            //修改AssetBundle配置文件的AssetBundleName
            AssetImporter importer = AssetImporter.GetAtPath(bundleConfigPath.Replace(Application.dataPath, "Assets"));
            if (importer != null)
            {
                importer.assetBundleName = mBundleModuleEnum.ToString().ToLower() + "bundleConfig.unity";
            }
        }

        private static bool IsRepeat(string path)
        {
            foreach (var item in mAllBundlePathList)
            {
                if (string.Equals(item, path) || item.Contains(path) || path.EndsWith(".cs"))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GenerateBundleName(string abName)
        {
            return mBundleModuleEnum.ToString() + "_" + abName;
        }
    }
}