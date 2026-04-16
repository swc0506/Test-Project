using System.Collections.Generic;
using Core.FS;
using UnityEditor;

namespace CoreEditor.FS
{
    internal class FSSettingsObject : SettingsObject
    {
        public List<AssetPackage> packages;
        public List<StepPipeline> pipelines;

        public static BuildAssetBundleOptions DefaultBundleOptions
        {
            get
            {
                return BuildAssetBundleOptions.DeterministicAssetBundle |
                       BuildAssetBundleOptions.ChunkBasedCompression;
            }
        }

        protected override void OnInitial()
        {
            packages = new List<AssetPackage>();
            pipelines = new List<StepPipeline>();
        }

        public static FSSettingsObject Get()
        {
            return Load<FSSettingsObject>("FS");
        }
    }

    [System.Serializable]
    public class AssetPackage
    {
        public string name;
        public List<BuildGroup> groups;

        public AssetPackage()
        {
            groups = new List<BuildGroup>();
        }
    }

    [System.Serializable]
    public class BuildGroup
    {
        public string name;
        public string rule;
        public bool enable;
        public List<string> assetsGUID;

        public BuildGroup()
        {
            enable = true;
            assetsGUID = new List<string>();
        }
    }


    [System.Serializable]
    public class StepPipeline
    {
        public string pkgName;
        public List<CommandStep> steps;
        public BuildAssetBundleOptions bundleOptions;

        public StepPipeline()
        {
            steps = new List<CommandStep>();
            bundleOptions = FSSettingsObject.DefaultBundleOptions;
        }
    }

    [System.Serializable]
    public class CommandStep
    {
        public string cmd;
        public bool enable;
    }
}