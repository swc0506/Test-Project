using System.Collections.Generic;
using System.IO;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "DependenceRefCountSameDirectory", Describe = "把该组的每个资源打包成ab,以及其依赖引用计数>=2的根据同目录打包到一个ab")]
    public class RuleDependenceRefCountSameDirectory : IBundleRule
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
                    string bundleName = null;
                    var textureType = AssetUtils.GetTextureType(dep.Key);
                    //不是贴图类型的,根据同目录打包
                    if (textureType == TextureType.Unknown)
                    {
                        string dir = Path.GetDirectoryName(dep.Key).Replace("\\", "/");
                        bundleName = AssetPath.GetBundleName(dir);
                    }
                    else
                    {
                        //单独打包
                        bundleName = AssetPath.GetBundleName(dep.Key);
                    }

                    buildMap[dep.Key] = bundleName;
                }
            }

            return buildMap;
        }
    }
}