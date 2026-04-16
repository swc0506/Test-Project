using System.Collections.Generic;
using System.IO;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "SameDirectory", Describe = "把该组的每个资源根据同目录打包到同一个ab")]
    public class RuleSameDirectory : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();

            //<目录,所有资源>
            Dictionary<string, List<string>> dirMap = new Dictionary<string, List<string>>();
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                string dir = Path.GetDirectoryName(path).Replace("\\", "/");
                if (!dirMap.TryGetValue(dir, out var list))
                {
                    list = new List<string>();
                    dirMap.Add(dir, list);
                }

                list.Add(path);
            }

            foreach (var item in dirMap)
            {
                string bundleName = AssetPath.GetBundleName(item.Key);
                foreach (var path in item.Value)
                {
                    buildMap[path] = bundleName;
                }
            }

            return buildMap;
        }
    }
}