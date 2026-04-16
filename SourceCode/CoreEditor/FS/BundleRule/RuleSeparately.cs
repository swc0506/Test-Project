using System.Collections.Generic;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BundleRule(Name = "Separately", Describe = "把该组的每个资源打包到不同的ab")]
    public class RuleSeparately : IBundleRule
    {
        public Dictionary<string, string> Anasyle(IEnumerable<Object> assets, string groupName)
        {
            Dictionary<string, string> buildMap = new Dictionary<string, string>();
            
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                buildMap[path] = AssetPath.GetBundleName(path);
            }

            return buildMap;
        }
    }
}