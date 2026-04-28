using UnityEngine;
using ZM.AssetFrameWork;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        FrameBase.Instance.InitFrameWork();
    }

    void Start()
    {
        //HotUpdateManager.Instance.CheckAssetsVersion(BundleModuleEnum.Game);
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, this);
    }
    
    public void StartGame()
    {
        AssetsFrame.Instantiate("Assets/Demo/Hall/Prefab/LoginWindow", null, Vector3.zero, Vector3.one,
            Quaternion.identity);
        AssetsFrame.HotAssets(BundleModuleEnum.GameItem, null, null, null);
    }
}
