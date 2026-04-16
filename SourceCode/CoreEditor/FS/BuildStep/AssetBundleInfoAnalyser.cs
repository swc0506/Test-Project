using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    internal static class AssetBundleInfoAnalyser
    {
        private static Queue<BuildGroup> buildGroups;
        private static Dictionary<string, IBundleRule> ruleMap;

        private static ProgressHandler progressHandler;

        static AssetBundleInfoAnalyser()
        {
            buildGroups = new Queue<BuildGroup>();
            ruleMap = new Dictionary<string, IBundleRule>();

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

        private static void CollectBundleRules()
        {
            List<IBundleRule> rules;
            List<BundleRuleAttribute> attributes;
            AttributeUtils.GetInstancesAttribute<IBundleRule, BundleRuleAttribute>(out rules, out attributes);
            ruleMap.Clear();
            for (int i = 0; i < rules.Count; i++)
            {
                ruleMap.Add(attributes[i].Name, rules[i]);
            }
        }

        public static Dictionary<string, string> Analyse()
        {
            CollectBundleRules();
            var infoMap = new Dictionary<string, string>();
            progressHandler.SetInfo(buildGroups.Count, "Analyse Build Asset Groups", "Asset Groups");
            foreach (var group in buildGroups)
            {
                progressHandler.Tick();
                if (!ruleMap.TryGetValue(group.rule, out var rule))
                {
                    continue;
                }

                HashSet<Object> all = new HashSet<Object>();
                foreach (var item in group.assetsGUID)
                {
                    string path = AssetDatabase.GUIDToAssetPath(item);
                    Object obj = AssetDatabase.LoadMainAssetAtPath(path);
                    var assets = AssetUtils.GetAssets(obj);
                    all.UnionWith(assets);
                }

                var pathNameMap = rule.Anasyle(all, group.name);
                if (null != pathNameMap)
                {
                    foreach (var item in pathNameMap)
                    {
                        infoMap[item.Key] = item.Value;
                    }
                }
            }

            buildGroups.Clear();
            Caching.ClearCache();

            return infoMap;
        }
    }
}