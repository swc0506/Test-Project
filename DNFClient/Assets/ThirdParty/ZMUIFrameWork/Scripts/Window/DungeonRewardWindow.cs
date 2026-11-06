/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/9/15 21:12:00
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using System.Collections.Generic;
using ZM.AssetFrameWork;

public class DungeonRewardWindow : WindowBase
{

    public DungeonRewardWindowDataComponent dataCompt;
    public List<CardItem> mCardItemList=new List<CardItem>();

    public bool isReceiveReward = false;    
    private int mTimerCount =9;
    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<DungeonRewardWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
        for (int i = 0; i < dataCompt.CardTopRootTransform.childCount; i++)
        {
            CardItem itemScript= dataCompt.CardTopRootTransform.GetChild(i).GetComponent<CardItem>();
            itemScript.Initlizate(i);
            mCardItemList.Add(itemScript);
        }

     }
    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        LogicTimerManager.Instance.DelayCall(1, OnTiemrCallback,10);
    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (CardItem item in mCardItemList)
        {
            item.OnRelease();
        }
    }
    #endregion
    #region API Function
    public void OnTiemrCallback()
    {
        mTimerCount--;
        dataCompt.TimeImage.sprite = ZMAsset.LoadPNGAtlasSprite(AssetPathConfig.Hall_TEXTURES_PATH+ "ReConnect/pck_Number","Pk_pStartNum"+mTimerCount);
        if (mTimerCount == 0)
        { 
            HideWindow();
            UIModule.Instance.GetWindow<BattleWindow>().dataCompt.ExitButton.gameObject.SetActive(true);
        }
    }
    #endregion
    #region UI组件事件
    public void OnCloseButtonClick()
    {
        HideWindow();
        UIModule.Instance.GetWindow<BattleWindow>().dataCompt.ExitButton.gameObject.SetActive(true);
    }
    #endregion
}
