/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/7/22 0:16:38
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using System.Collections.Generic;
using Fantasy;
using ZM.AssetFrameWork;
using ZMGC.Hall;

public class DungeonLoadingWindow : WindowBase
{

    public DungeonLoadingWindowDataComponent dataCompt;
    private DungeonSelectMsgMgr mDungeonsMsgLayer;
    private TeamDataMgr mTeamDataLayer;
    /// <summary>
    /// 所有玩家进度item列表
    /// </summary>
    private List<DungeonLoadingItem> mItemList = new List<DungeonLoadingItem>();
    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<DungeonLoadingWindowDataComponent>();
        dataCompt.InitComponent(this);
        mDisableAnim = true;
        base.OnAwake();
        mDungeonsMsgLayer = HallWorld.GetExitsMsgMgr<DungeonSelectMsgMgr>();
        mTeamDataLayer= HallWorld.GetExitsDataMgr<TeamDataMgr>();
    }
    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        UIEventControl.AddEvent( UIEventEnum.SceneProgressUpdate, OnSceneProgressUpdate);
    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
        UIEventControl.RemoveEvent(UIEventEnum.SceneProgressUpdate, OnSceneProgressUpdate);
        foreach (DungeonLoadingItem item in mItemList)
            item.OnRelease();
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion
    #region API Function
    public void InitView(List<RoleData> roleInfoList)
    {
        foreach (var item in roleInfoList)
        {
            GameObject itemObj = ZMAsset.Instantiate(AssetPathConfig.HALL_PREFABS_ITEM_PATH + "DungeonLoadingItem", dataCompt.DownHorizationTransform);
            DungeonLoadingItem itemScript = itemObj.GetComponent<DungeonLoadingItem>();
            itemScript.SetItemData(item, 0);
            mItemList.Add(itemScript);
        }
    }
    public void OnSceneProgressUpdate(object data)
    {
        float curProgress = (float)data / 100;
        mDungeonsMsgLayer.SendLoadDungeonsProgressMessage(mTeamDataLayer.TeamID,HallWorld.UserData.AccountId,curProgress);
    }
    #endregion
    #region UI组件事件
    public void OnCloseButtonClick()
    {

        HideWindow();
    }
    #endregion
}
