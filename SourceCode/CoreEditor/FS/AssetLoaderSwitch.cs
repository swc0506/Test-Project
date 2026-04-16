using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    public static class AssetLoaderSwitch
    {
        private const string ASSETBUNDLE_LOADER_MENU = "Tools/AssetLoader Switch/AssetBundle";
        private const string EDITORASSET_LOADER_MENU = "Tools/AssetLoader Switch/EditorAsset";

        private static string ASSETBUNDLE_LOADER_PREFS = "Core.FS_ASSETBUNDLE_LOADER_PREFS" + Application.productName;
        private static string EDITORASSET_LOADER_PREFS = "Core.FS_EDITORASSET_LOADER_PREFS" + Application.productName;

        public static bool EnableAssetBundle
        {
            get { return EditorPrefs.GetBool(ASSETBUNDLE_LOADER_PREFS, false); }
        }

        public static bool EnableEditorResources
        {
            get { return EditorPrefs.GetBool(EDITORASSET_LOADER_PREFS, true); }
        }


        [MenuItem(ASSETBUNDLE_LOADER_MENU, false, 2002)]
        private static void ToggleEnableAssetBundle()
        {
            EditorPrefs.SetBool(ASSETBUNDLE_LOADER_PREFS, !EnableAssetBundle);
        }


        [MenuItem(ASSETBUNDLE_LOADER_MENU, true, 2002)]
        private static bool ToggleEnableAssetBundleValidate()
        {
            Menu.SetChecked(ASSETBUNDLE_LOADER_MENU, EnableAssetBundle);
            return true;
        }

        [MenuItem(EDITORASSET_LOADER_MENU, false, 2002)]
        private static void ToggleEnableEditorResources()
        {
            EditorPrefs.SetBool(EDITORASSET_LOADER_PREFS, !EnableEditorResources);
        }


        [MenuItem(EDITORASSET_LOADER_MENU, true, 2002)]
        private static bool ToggleEnableEditorResourcesValidate()
        {
            Menu.SetChecked(EDITORASSET_LOADER_MENU, EnableEditorResources);
            return true;
        }
    }
}