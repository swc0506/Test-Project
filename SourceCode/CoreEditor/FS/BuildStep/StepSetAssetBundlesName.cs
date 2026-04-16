using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BuildStep(Name = "SetAssetBundlesName", Describe = "设置需要打包资源的AssetBundle Name")]
    public class StepSetAssetBundlesName : IBuildStep, IStepDrawable
    {
        public void Execute(AssetPackage pkg)
        {
            foreach (var item in pkg.groups)
            {
                AssetBundleInfoAnalyser.AddGroup(item);
            }

            var infoMap = AssetBundleInfoAnalyser.Analyse();
            var setProgress = new ProgressHandler();
            setProgress.SetInfo(infoMap.Count, "Set AssetBundle Name");
            foreach (var item in infoMap)
            {
                setProgress.Tick();
                AssetBundleUtils.SetAssetBundleName(item.Key, item.Value);
            }

            AssetDatabase.Refresh();
        }

        public void Draw(AssetPackage pkg)
        {
            EditorGUILayout.LabelField("Select the group to set assetbundle name:");
            for (int i = 0; i < pkg.groups.Count; i++)
            {
                GUILayout.Space(5);
                pkg.groups[i].enable = EditorGUILayout.ToggleLeft(pkg.groups[i].name, pkg.groups[i].enable);
            }
        }
    }
}