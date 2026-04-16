using Core;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BuildStep(Name = "RevertDllAssets", Describe = "还原加密的Dll资源")]
    public class StepRevertDllAssets : IBuildStep
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
                if (item.Value is TextAsset textAsset && item.Key.Contains(".dll"))
                {
                    bytes = textAsset.bytes;
                }

                if (null != bytes)
                {
                    string srcPath = item.Key;
                    string desPath = srcPath.Replace(StepEncryptDllAssets.ASSET_KEY,
                        StepEncryptDllAssets.TEMP_KEY);
                    FileUtils.MoveFile(desPath, srcPath);
                }
            }

            AssetDatabase.Refresh();
            FileUtils.DeleteDirectory(StepEncryptDllAssets.TEMP_KEY);
        }
    }
}