using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    [BuildStep(Name = "BuildAssetBundlesByInfo", Describe = "根据自定义AssetBundleBuild信息打包资源")]
    public class StepBuildAssetBundlesByInfo : IBuildStep
    {
        public void Execute(AssetPackage pkg)
        {
            foreach (var item in pkg.groups)
            {
                AssetBundleInfoAnalyser.AddGroup(item);
            }

            var buildMap = new Dictionary<string, AssetBundleBuild>();
            var infoMap = AssetBundleInfoAnalyser.Analyse();
            var setProgress = new ProgressHandler();
            setProgress.SetInfo(infoMap.Count, "Analyse AssetBundleBuild");
            foreach (var item in infoMap)
            {
                setProgress.Tick();
                if (buildMap.TryGetValue(item.Value, out var build))
                {
                    int srcLength = build.assetNames.Length;
                    string[] assetNames = new string[srcLength + 1];
                    Array.Copy(build.assetNames, assetNames, srcLength);
                    assetNames[srcLength] = item.Key;
                    build.assetNames = assetNames;
                }
                else
                {
                    build = new AssetBundleBuild();
                    build.assetBundleName = item.Value;
                    build.assetNames = new string[] {item.Key};
                }

                buildMap[item.Value] = build;
            }

            AssetBundleBuild bundlesNameManifest =
                CreateAddressManifestBuild(infoMap, pkg.name, out string manifestPath);
            buildMap[bundlesNameManifest.assetBundleName] = bundlesNameManifest;

            AssetBundleBuild[] builds = new List<AssetBundleBuild>(buildMap.Values).ToArray();
            string outPath = Path.Combine(AssetBundleUtils.AssetPackagesOutPath, pkg.name);
            FileUtils.CreateDirectory(outPath);
            BuildAssetBundleOptions options = GetPackageBuildOption(pkg.name);
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildPipeline.BuildAssetBundles(outPath, builds, options, buildTarget);
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

        private AssetBundleBuild CreateAddressManifestBuild(Dictionary<string, string> infoMap, string pkgName,
            out string manifestPath)
        {
            AssetBundleUtils.CreateAddressManifest(infoMap, pkgName, out manifestPath, out string bundleName);
            var manifestBuild = new AssetBundleBuild();
            manifestBuild.assetBundleName = bundleName;
            manifestBuild.assetNames = new string[] {manifestPath};
            return manifestBuild;
        }
    }
}