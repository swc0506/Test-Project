using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class BuildWindow : OdinMenuEditorWindow
{
    [SerializeField]
    public BuildBundleWindow bundleWindow = new BuildBundleWindow();
    
    [SerializeField]
    public BuildHotPatchWindow hotPatchWindow = new BuildHotPatchWindow();
    
    [MenuItem("Frame/Build AssetBundle")]
    public static void ShowAssetBundleWindow()
    {
        BuildWindow window = GetWindow<BuildWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(985, 612);
        window.ForceMenuTreeRebuild();
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        bundleWindow.Init();
        hotPatchWindow.Init();
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Build", null, EditorIcons.House },
            { "Build/AssetBundle", bundleWindow, EditorIcons.UnityLogo},
            { "Build/HotPatch", hotPatchWindow, EditorIcons.UnityLogo}
        };
        return menuTree;
    }
}
