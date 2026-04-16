using System.Collections.Generic;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "DependenceRefCount", Describe = "把该组的每个资源打包成ab,以及其依赖引用计数>=2的打包到不同的ab")]
    public class RuleDependenceRefCount : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();

            HashSet<string> mainSet = new HashSet<string>();
            Dictionary<string, int> depCountMap = new Dictionary<string, int>();
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                mainSet.Add(path);
                buildMap[path] = AssetPath.GetBundleName(path);

                string[] dependencies = AssetBundleUtils.GetBuildDependencies(path);
                foreach (var dep in dependencies)
                {
                    if (!depCountMap.TryGetValue(dep, out var value))
                    {
                        depCountMap.Add(dep, value);
                    }

                    depCountMap[dep] = ++value;
                }
            }

            foreach (var dep in depCountMap)
            {
                if (mainSet.Contains(dep.Key))
                {
                    continue;
                }

                if (dep.Value >= 2)
                {
                    buildMap[dep.Key] = AssetPath.GetBundleName(dep.Key);
                }
            }

            return buildMap;
        }
    }
}