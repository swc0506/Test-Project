using System.Collections.Generic;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "PackTogether", Describe = "把该组的每个资源以及其每个依赖都打包到一个ab")]
    public class RulePackTogether : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();
            
            string bundleName = AssetPath.GetBundleName(string.Format("PackTogether:{0}", groupName));
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                buildMap[path] = bundleName;

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