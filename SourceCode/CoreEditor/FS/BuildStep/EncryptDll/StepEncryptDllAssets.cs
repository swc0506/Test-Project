using Core;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BuildStep(Name = "EncryptDllAssets", Describe = "加密Dll资源")]
    public class StepEncryptDllAssets : IBuildStep
    {
        public const string ASSET_KEY = "Assets/";
        public const string TEMP_KEY = "TempDllAssets/";

        public void Execute(AssetPackage pkg)
        {
            foreach (var item in pkg.groups)
            {
                AssetScanner.AddGroup(item);
            }

            var infoMap = AssetScanner.Scan();
            var setProgress = new ProgressHandler();
            setProgress.SetInfo(infoMap.Count, "Encrypt Assets");
            foreach (var item in infoMap)
            {
                setProgress.Tick();

                byte[] bytes = null;
                if (item.Value is TextAsset textAsset && item.Key.Contains(".dll"))
                {
                    bytes = textAsset.bytes;
                }

                if (null != bytes)
                {
                    //backup file
                    string srcPath = item.Key;
                    string desPath = srcPath.Replace(ASSET_KEY, TEMP_KEY);
                    FileUtils.CopyFile(srcPath, desPath);

                    //encrypt file
                    bytes = EncryptUtils.Encrypt(FSConfig.Instance.EncryptKey, bytes);
                    FileUtils.CreateFile(item.Key, bytes);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}