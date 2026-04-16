using System.Collections.Generic;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "SeparatelyDependence", Describe = "把该组的每个资源以及其每个依赖都打包到不同的ab")]
    public class RuleSeparatelyDependence : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();
            
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                buildMap[path] = AssetPath.GetBundleName(path);

                string[] dependencies = AssetBundleUtils.GetBuildDependencies(path);
                foreach (var dep in dependencies)
                {
                    buildMap[dep] = AssetPath.GetBundleName(dep);
                }
            }

            return buildMap;
        }
    }
}