using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ZM.AssetFrameWork
{
    public class AssetsDecompressManager : IDecompressAssets
    {
        // 资源内嵌路径
        private string mStreamAbPath;

        // 资源解压路径
        private string mDecompressPath;

        // 需要解压的文件列表
        private List<string> mNeedDecompressFileList = new List<string>();

        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModuleEnum, Action callBack)
        {
            if (ComputeDecompressFile(bundleModuleEnum))
            {
                IsStart = true;
                FrameBase.Instance.StartCoroutine(UnPackToPersistentDataPath(bundleModuleEnum, callBack));
            }
            else
            {
                Debug.Log("不需要解压");
                callBack?.Invoke();
            }

            return this;
        }

        /// <summary>
        /// 解压文件到持久化目录
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator UnPackToPersistentDataPath(BundleModuleEnum bundleModuleEnum, Action callBack)
        {
            foreach (var fileName in mNeedDecompressFileList)
            {
                string filePath = "";
#if UNITY_EDITOR_OSX || UNITY_IOS
                filePath = "file://" + mStreamAbPath + fileName;
#else
                filePath = mStreamAbPath + fileName;
#endif 
                Debug.Log("Unpack filePath: " + filePath + "\r\n UnpackPath:" + mDecompressPath);
                // 本地下载 不耗流量
                UnityWebRequest request = UnityWebRequest.Get(filePath);
                request.timeout = 30;
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Unpack filePath error: " + request.error);
                }
                else
                {
                    byte[] data = request.downloadHandler.data;
                    File.WriteAllBytes(mDecompressPath + fileName, data);
                    AlreadySize += data.Length / 1024f / 1024f;
                    Debug.Log("解压大小:" + AlreadySize + "M" + " 总大小:" + TotalSize + "M");
                    Debug.Log("UnPack Finish " + mDecompressPath + fileName);
                }
                
                request.Dispose();
            }
            
            callBack?.Invoke();
            IsStart = false;
        }

        public override float GetDecompressProgress()
        {
            return AlreadySize / TotalSize;
        }

        /// <summary>
        /// 计算解压文件
        /// </summary>
        /// <param name="bundleModuleEnum"></param>
        /// <returns></returns>
        private bool ComputeDecompressFile(BundleModuleEnum bundleModuleEnum)
        {
            mStreamAbPath = BundleSettings.Instance.GetAssetsBuiltinPath(bundleModuleEnum);
            mDecompressPath = BundleSettings.Instance.GetAssetsDecompressPath(bundleModuleEnum);
            mNeedDecompressFileList.Clear();

#if UNITY_ANDROID || UNITY_IOS

            if (!Directory.Exists(mDecompressPath))
            {
                Directory.CreateDirectory(mDecompressPath);
            }

            //计算需要解压的文件，以及大小
            TextAsset textAsset = Resources.Load<TextAsset>(bundleModuleEnum + "Info");
            if (textAsset != null)
            {
                List<BuiltinBundleInfo> bundleInfoList =
                    JsonConvert.DeserializeObject<List<BuiltinBundleInfo>>(textAsset.text);
                foreach (var bundleInfo in bundleInfoList)
                {
                    string localFilePath = mDecompressPath + bundleInfo.fileName;
                    if (localFilePath.EndsWith(".meta"))
                    {
                        continue;
                    }

                    //计算出来需要解压的文件
                    if (!File.Exists(localFilePath) || MD5.GetMd5FromFile(localFilePath) != bundleInfo.md5)
                    {
                        mNeedDecompressFileList.Add(bundleInfo.fileName);
                        TotalSize += bundleInfo.size / 1024f;
                    }
                }

                return mNeedDecompressFileList.Count > 0;
            }

            Debug.LogError("ComputeDecompressFile error: " + bundleModuleEnum + "Info not found");

#endif

            return false;
        }
    }
}