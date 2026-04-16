using Core;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    [BuildStep(Name = "RevertTextAssets", Describe = "还原加密的文本资源")]
    public class StepRevertTextAssets : IBuildStep
    {
        public void Execute(AssetPackage pkg)
        {
            foreach (var item in pkg.groups)
            {
                AssetScanner.AddGroup(item);
            }

            var infoMap = AssetScanner.Scan();
            var setProgress = new ProgressHandler();
            setProgress.SetInfo(infoMap.Count, "Revert Encrypt Assets");
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
                    string srcPath = item.Key;
                    string desPath = srcPath.Replace(StepEncryptTextAssets.ASSET_KEY,
                        StepEncryptTextAssets.TEMP_KEY);
                    FileUtils.MoveFile(desPath, srcPath);
                }
            }

            AssetDatabase.Refresh();
            FileUtils.DeleteDirectory(StepEncryptTextAssets.TEMP_KEY);
        }
    }
}