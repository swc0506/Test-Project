using System.Collections.Generic;
using System.IO;
using UnityEditor;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    [BuildStep(Name = "BuildAssetBundlesByName", Describe = "根据已经设置的AssetBundle Name打包资源")]
    public class StepBuildAssetBundlesByName : IBuildStep
    {
        public void Execute(AssetPackage pkg)
        {
            string manifestPath = CreateBundlesNameManifest(pkg.name);

            string outPath = Path.Combine(AssetBundleUtils.AssetPackagesOutPath, pkg.name);
            FileUtils.CreateDirectory(outPath);
            BuildAssetBundleOptions options = GetPackageBuildOption(pkg.name);
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildPipeline.BuildAssetBundles(outPath, options, buildTarget);
            AssetDatabase.DeleteAsset(manifestPath);
        }

        private BuildAssetBundleOptions GetPackageBuildOption(string pkgName)
        {
            var config = FSSettingsObject.Get();
            foreach (var item in config.pipelines)
            {
                if (item.pkgName == pkgName)
                {
                    return item.bundleOptions;
                }
            }

            return FSSettingsObject.DefaultBundleOptions;
        }

        private string CreateBundlesNameManifest(string pkgName)
        {
            Dictionary<string, string> bundlesNameMap = new Dictionary<string, string>();
            string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundle in bundleNames)
            {
                string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
                foreach (var path in paths)
                {
                    bundlesNameMap[path] = bundle;
                }
            }

            AssetBundleUtils.CreateAddressManifest(bundlesNameMap, pkgName, out string manifestPath, out string bundleName);
            AssetBundleUtils.SetAssetBundleName(manifestPath, bundleName);
            return manifestPath;
        }
    }
}