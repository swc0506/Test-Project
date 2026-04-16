using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    internal static class AssetScanner
    {
        private static Queue<BuildGroup> buildGroups;
        private static ProgressHandler progressHandler;

        static AssetScanner()
        {
            buildGroups = new Queue<BuildGroup>();
            progressHandler = new ProgressHandler();
        }

        private static bool ContainsGroup(BuildGroup group)
        {
            foreach (var item in buildGroups)
            {
                if (item.name == group.name)
                {
                    return true;
                }
            }

            return false;
        }


        public static void AddGroup(BuildGroup group)
        {
            if (group.enable && !ContainsGroup(group))
            {
                buildGroups.Enqueue(group);
            }
        }

        public static Dictionary<string, Object> Scan()
        {
            var infoMap = new Dictionary<string, Object>();
            progressHandler.SetInfo(buildGroups.Count, "Scan Asset Groups", "Asset Groups");
            foreach (var group in buildGroups)
            {
                progressHandler.Tick();

                HashSet<Object> all = new HashSet<Object>();
                foreach (var item in group.assetsGUID)
                {
                    string path = AssetDatabase.GUIDToAssetPath(item);
                    Object obj = AssetDatabase.LoadMainAssetAtPath(path);
                    var assets = AssetUtils.GetAssets(obj);
                    foreach (var asset in assets)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(asset);
                        infoMap[assetPath] = asset;
                    }
                }
            }

            buildGroups.Clear();
            Caching.ClearCache();

            return infoMap;
        }
    }
}