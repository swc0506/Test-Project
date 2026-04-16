using System.Collections.Generic;
using UnityEngine;

namespace CoreEditor.FS
{
    public static class AssetPackageBuilder
    {
        private static Dictionary<string, IBuildStep> CollectBuildStepMap()
        {
            List<IBuildStep> steps;
            List<BuildStepAttribute> attributes;
            AttributeUtils.GetInstancesAttribute<IBuildStep, BuildStepAttribute>(out steps, out attributes);
            var stepMap = new Dictionary<string, IBuildStep>();
            for (int i = 0; i < steps.Count; i++)
            {
                stepMap.Add(attributes[i].Name, steps[i]);
            }

            return stepMap;
        }

        public static void BuildPackage(AssetPackage pkg, List<CommandStep> steps)
        {
            StatisticsTime time = new StatisticsTime();
            time.Start(string.Format("Start Build {0} AssetPkg", pkg.name));
            var stepMap = CollectBuildStepMap();
            var stepProgress = new ProgressHandler();
            stepProgress.SetInfo(steps.Count, "Execute Build Step", "Build Step");
            foreach (var item in steps)
            {
                stepProgress.Tick();
                if (item.enable && stepMap.TryGetValue(item.cmd, out var step))
                {
                    Debug.LogFormat("Execute Build Step:{0}", step.GetType());
                    step.Execute(pkg);
                }
            }

            time.Stop(string.Format("{0} AssetPkg Build Finished", pkg.name));
        }

        private static AssetPackage FindPackage(FSSettingsObject settings, string name)
        {
            foreach (var item in settings.packages)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            return null;
        }

        public static void BuildAllPackages()
        {
            var config = FSSettingsObject.Get();
            foreach (var item in config.pipelines)
            {
                var pkg = FindPackage(config, item.pkgName);
                BuildPackage(pkg, item.steps);
            }
        }
    }
}