using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZMGC.Battle;
using ZMGC.Hall;
//using ZM.AssetFrameWork;
public class Main : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //初始化资源管理框架
        //ZMAssetsFrame.Instance.InitFrameWork();
        //初始化UI框架
        UIModule.Instance.Initialize();

        WorldManager.CreateWorld<HallWorld>();
        //不允许销毁当前节点
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 资源解压完成之后会调用，
    /// </summary>
    public void StartGame()
    {

    }

    public void LoadSceneAsync()
    {
        StartCoroutine(AsyncLoadScene());
    }

    IEnumerator AsyncLoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Battle");
        operation.allowSceneActivation = false;

        float cur = 0;
        float max = 100;
        while (cur < 90)
        {
            cur = operation.progress * 100.0f;
            yield return null;
        }

        while (cur < max)
        {
            cur++;
            yield return null;
        }
        
        operation.allowSceneActivation = true;//激活
        yield return null;
        //创建英雄
    }

// Update is called once per frame
    void Update()
    {
        //WorldManager.OnUpdate();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>().HeroLogic.velocity = new FixMath.FixIntVector3(0, 6, 0);
            //BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>().HeroLogic.isAddForce=true;
        }
    }
}
