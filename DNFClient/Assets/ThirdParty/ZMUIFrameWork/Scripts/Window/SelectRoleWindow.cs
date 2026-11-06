/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/7/6 19:08:12
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using System;
using System.Collections.Generic;
using ZMGC.Hall;
using Fantasy;

public class SelectRoleWindow : WindowBase
{

    public SelectRoleWindowDataComponent dataCompt;

    private List<SelectRoleItem> mRoleItemList= new List<SelectRoleItem>();
    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<SelectRoleWindowDataComponent>();
        dataCompt.InitComponent(this);
        mDisableAnim = true;
        FullScreenWindow = true;
        base.OnAwake();
        for (int i = 0; i < dataCompt.RoleListTransform.childCount; i++)
        {
            mRoleItemList.Add(dataCompt.RoleListTransform.GetChild(i).GetComponent<SelectRoleItem>());
        }
    }
    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        ShowRoleList();
    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
        foreach (SelectRoleItem item in mRoleItemList)
            item.OnRelease();
    }
    /// <summary>
    /// 伪隐藏触发函数
    /// </summary>
    /// <param name="value"></param>
    public override void PseudoHidden(int value)
    {
        base.PseudoHidden(value);
        foreach (SelectRoleItem item in mRoleItemList)
            item.SetRoleActive(value==1);
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion
    #region API Function
    private void ShowRoleList()
    {
        List<RoleData> roleDataList= HallWorld.UserData.AllRoleDataList;
        for (int i = 0; i < mRoleItemList.Count; i++)
        {
            if (roleDataList.Count-1>=i)
            {
                mRoleItemList[i].SetItemData(roleDataList[i],this);
            }
            if (i == 0)
            {
                mRoleItemList[i].OnSelectButtonClick();
            }
        }
    }
    public void HideAllItemSelect()
    {
        foreach (SelectRoleItem item in mRoleItemList)
        {
            item.SetSelectState(false);
        }

    }
    #endregion
    #region UI组件事件
    public void OnCloseButtonClick()
    {

        HideWindow();
    }
    public async void OnEnterGameButtonClick()
    {
        PopUpWindow<ReConnectWindow>();
        await HallWorld.GetExitsLogicCtrl<LoginRoleLogicCtrl>().SelectRoleEnterHall();
        UIModule.Instance.HideWindow<ReConnectWindow>();
    }
    public void OnCreateRoleButtonClick()
    {
        UIModule.Instance.PopUpWindow<CreateRoleWindow>();
    }
    public void OnDeleteButtonClick()
    {

    }
    public void OnAllowRightButtonClick()
    {

    }
    public void OnAllowLeftButtonClick()
    {

    }
    #endregion
}
