using Core;
using Core.FS;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    [BuildStep(Name = "EncryptTextAssets", Describe = "加密文本资源")]
    public class StepEncryptTextAssets : IBuildStep
    {
        public const string ASSET_KEY = "Assets/";
        public const string TEMP_KEY = "TempTextAssets/";

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
                if (item.Value is TextAsset textAsset)
                {
                    bytes = textAsset.bytes;
                }
                else if (item.Value is BinaryAsset binaryAsset)
                {
                    bytes = binaryAsset.bytes;
                }
                else if (item.Value is LuaAsset luaAsset)
                {
                    bytes = luaAsset.bytes;
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