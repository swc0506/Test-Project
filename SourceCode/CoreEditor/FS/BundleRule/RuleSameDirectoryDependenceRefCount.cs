using System.Collections.Generic;
using System.IO;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "SameDirectoryDependenceRefCount", Describe = "把该组的每个资源根据同目录打包到同一个ab,以及其依赖资源引用计数>=2的打包到不同的ab")]
    public class RuleSameDirectoryDependenceRefCount : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();

            HashSet<string> mainSet = new HashSet<string>();
            //<目录,所有资源>
            Dictionary<string, List<string>> dirMap = new Dictionary<string, List<string>>();
            Dictionary<string, int> depCountMap = new Dictionary<string, int>();
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                mainSet.Add(path);

                string dir = Path.GetDirectoryName(path).Replace("\\", "/");
                if (!dirMap.TryGetValue(dir, out var list))
                {
                    list = new List<string>();
                    dirMap.Add(dir, list);
                }

                list.Add(path);

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

            foreach (var item in dirMap)
            {
                foreach (var path in item.Value)
                {
                    buildMap[path] = AssetPath.GetBundleName(item.Key);
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