using System.IO;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    [BuildStep(Name = "DeleteExistingAssetBundles", Describe = "删除已经打包过的所有AssetBundles")]
    public class StepDeleteExistingAssetBundles : IBuildStep
    {
        public void Execute(AssetPackage pkg)
        {
            string outPath = Path.Combine(AssetBundleUtils.AssetPackagesOutPath, pkg.name);
            FileUtils.DeleteDirectory(outPath);
        }
    }
}