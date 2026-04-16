using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    [BuildStep(Name = "ClearAssetBundlesName", Describe = "清除所有的AssetBundle Name")]
    public class StepClearAssetBundlesName : IBuildStep
    {
        public void Execute(AssetPackage pkg)
        {
            string[] names = AssetDatabase.GetAllAssetBundleNames();
            var stepProgress = new ProgressHandler();
            stepProgress.SetInfo(names.Length, "Clear AssetBundles Name");
            foreach (var item in names)
            {
                stepProgress.Tick();
                //该方法unity有bug 在inspector显示empty但是meta内还有值
//                AssetDatabase.RemoveAssetBundleName(item, true);
                RemoveAssetBundleName(item);
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            Caching.ClearCache();
        }

        private void RemoveAssetBundleName(string bundleName)
        {
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            foreach (var item in paths)
            {
                AssetBundleUtils.SetAssetBundleName(item, null);
            }
        }
    }
}