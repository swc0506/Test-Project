/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/2/11 18:29:59
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using ZMGC.Hall;
using ZM.AssetFrameWork;
using System.Collections.Generic;
using DG.Tweening;

public class CreateRuleWindow : WindowBase
{
    public CreateRuleWindowDataComponent dataCompt;
    private UserDataMgr mUserDataLayer;
    private List<RoleSelectItem> mItemList = new List<RoleSelectItem>();
    private int mCurSelectRoleid; //当前选中的角色id
    private DOTweenAnimation mDoTweenAnim;
    private GameObject mPortraitObj; //立绘对象

    #region 声明周期函数

    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        mDisableAnim = true;
        dataCompt = gameObject.GetComponent<CreateRuleWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
        mDoTweenAnim = dataCompt.RolePortraitRootTransform.GetComponent<DOTweenAnimation>();
        mUserDataLayer = HallWorld.GetExitsDataMgr<UserDataMgr>();
        for (int i = 0; i < mUserDataLayer.createRuleIdList.Count; i++)
        {
            GameObject itemObj = ZMAssetsFrame.Instantiate(AssetPathConfig.HALL_PREFABS_ITEM_PATH + "RuleISelecttem",
                dataCompt.ContentTransform, Vector3.zero, Vector3.one, Quaternion.identity);
            RoleSelectItem itemScript = itemObj.GetComponent<RoleSelectItem>();
            itemScript.SetItemData(mUserDataLayer.createRuleIdList[i]);
            mItemList.Add(itemScript);
        }

        SelectRoleUpdate(mUserDataLayer.createRuleIdList[0]);
    }

    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        Debug.Log("CreateRuleWindow OnShow");
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
    }

    #endregion

    #region API Function

    public void SelectRoleUpdate(int roleid)
    {
        mCurSelectRoleid = roleid;
        mUserDataLayer.RoleId = roleid;
        mUserDataLayer.userName = dataCompt.NameInputField.text;
        HideAllItemSelect();
        SelectTargetItem();

        if (mPortraitObj != null)
        {
            ZMAssetsFrame.Release(mPortraitObj);
            mPortraitObj = null;
        }

        //创建角色立绘
        mPortraitObj = ZMAssetsFrame.Instantiate(AssetPathConfig.Hall_EFFECTS_PATH + "RulePortrait/" + roleid,
            dataCompt.RolePortraitRootTransform, Vector3.zero, Vector3.one, Quaternion.identity);
        dataCompt.RolePortraitRootTransform.localPosition = new Vector3(-1500, 0, 0);
        mDoTweenAnim.DORestart();
    }

    private void HideAllItemSelect()
    {
        foreach (var item in mItemList)
        {
            item.SetSelectState(true);
        }
    }

    private void SelectTargetItem()
    {
        foreach (var item in mItemList)
        {
            if (item.roleid == mCurSelectRoleid)
            {
                item.SetSelectState(false);
                break;
            }
        }
    }

    #endregion

    #region UI组件事件

    public void OnCloseButtonClick()
    {
        HideWindow();
    }

    public void OnEnterGameButtonClick()
    {
        //Main.Instance.LoadSceneAsync();
        dataCompt.EnterGameButton.interactable = false;
        HallWorld.EnterBattleWorld();
    }

    public void OnNameInputChange(string text)
    {
    }

    public void OnNameInputEnd(string text)
    {
        HallWorld.GetExitsDataMgr<UserDataMgr>().userName = text;
    }

    #endregion
}