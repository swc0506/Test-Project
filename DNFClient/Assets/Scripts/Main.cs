using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZMGC.Battle;
using ZMGC.Hall;
using ZM.AssetFrameWork;
public class Main : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Debuger.InitLog();
        //初始化资源管理框架
        ZMAsset.Instance.InitFrameWork();
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Game,this);
        //不允许销毁当前节点
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 资源解压完成之后会调用，
    /// </summary>
    public async void StartGame()
    {
        //初始化UI框架
        UIModule.Instance.Initialize();
        WorldManager.CreateWorld<HallWorld>();
        await NetWorkManager.Instance.Initlization();
        Debuger.LogYellow("框架初始化完成...");
    }
 

    // Update is called once per frame
    void Update()
    {
        WorldManager.OnUpdate();
    }
}
