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
        //初始化资源框架
        ZMAssetsFrame.Instance.InitFrameWork();
        //初始化UI模块
        UIModule.Instance.Initialize();
        WorldManager.CreateWorld<HallWorld>();
        //跨场景不销毁当前节点
        DontDestroyOnLoad(gameObject);
        //随机测试1
        //LogicRandom random1 = new LogicRandom(10);
        //string randomResult = "logicRamdom:";
        //for (int i = 0; i < 11; i++)
        //{
        //    randomResult += random1.Range(1,360)+",";
        //}
        //Debug.Log(randomResult);

        ////随机测试2
        //LogicRandom random2 = new LogicRandom(10);
        //string randomResult2 = "logicRamdom:";
        //for (int i = 0; i < 11; i++)
        //{
        //    randomResult2 += random2.Range(1, 360) + ",";
        //}
        //Debug.Log(randomResult2);
        //1.randomResult 与 randomResult2 随机值并不一致，
        //2.randomResult 与 randomResult2 随机值完全一致，
    }
    /// <summary>
    /// 资源加载完成之后回调，
    /// </summary>
    public void StartGame()
    {
        
    }
 

    // Update is called once per frame
    void Update()
    {
        // WorldManager.OnUpdate();
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>().HeroLogic.velocity = new FixMath.FixIntVector3(0, 6, 0);
        //     BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>().HeroLogic.isAddForce=true;
        // }
    }
}
