using System.Collections.Generic;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "DependenceTogether", Describe = "把该组的每个资源打包成ab,以及其每个依赖打包到同一个ab")]
    public class RuleDependenceTogether : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();

            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                buildMap[path] = AssetPath.GetBundleName(path);

                string bundleName = AssetPath.GetBundleName(string.Format("DependenceTogether:{0}", groupName));
                string[] dependencies = AssetBundleUtils.GetBuildDependencies(path);
                foreach (var dep in dependencies)
                {
                    buildMap[dep] = bundleName;
                }
            }

            return buildMap;
        }
    }
}