using System.Collections.Generic;
using Core;
using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    public sealed partial class FSWindow : TabWindow
    {
        private FSSettingsObject settings;

        [MenuItem("Tools/Assets Window &A", false, 2000)]
        public static void ShowWindow()
        {
            OpenWindow<FSWindow>("Assets Window");
        }

        protected override string SelectTabIndexKey
        {
            get { return "FS.Select_Tab_Index_Key"; }
        }

        protected override void OnInitial()
        {
            windows = new BaseWindow[]
            {
                new ConfigWindow(),
                new BuildWindow(),
                new VersionWindow(),
            };
        }

        protected override void OnEnable()
        {
            settings = FSSettingsObject.Get();
            base.OnEnable();
        }

        protected override void OnDrawGUI()
        {
            GUI.skin.button.richText = true;
            EditorStyles.label.richText = true;
            base.OnDrawGUI();
        }

        private AssetPackage FindPackage(string name)
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

        private string[] CollectBuildTargets()
        {
            List<string> targets = new List<string>();
            List<BuildTarget> enums = ReflectionUtils.CollectEnum<BuildTarget>();
            foreach (var item in enums)
            {
                if (item > 0)
                {
                    targets.Add(item.ToString());
                }
            }

            return targets.ToArray();
        }

        private string[] CollectBuildPlatforms()
        {
            List<string> targets = new List<string>();
            List<BuildTarget> enums = ReflectionUtils.CollectEnum<BuildTarget>();
            foreach (var item in enums)
            {
                if (item > 0)
                {
                    string name = BuildPlatformPath.GetBuildPlatform(item);
                    targets.Add(name);
                }
            }

            return targets.ToArray();
        }
    }
}