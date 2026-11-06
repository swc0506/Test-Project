using Fantasy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;
using ZMGC.Hall;

public class SelectRoleItem : MonoBehaviour
{
    public GameObject selectObj;
    public GameObject noSelectObj;
    public GameObject nameRoot;

    public Text roleNameText;
    public Text levelText;
    public Text modelNameText;

    public Transform roleModelParent;
    private AssetsRequest mAssetRequest;
    private GameObject mRoleObj;//当前角色对象
    private SelectRoleWindow mWindow;
    private RoleData mRoleData;
    /// <summary>
    /// 设置Item数据
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="win"></param>
    public async void SetItemData(RoleData itemData,SelectRoleWindow win)
    {
        mWindow = win;
        mRoleData= itemData;
        levelText.text = "Lv." + itemData.level;
        roleNameText.text=itemData.roleName.ToString();
        modelNameText.text = ConfigCenter.Instance.GetHeroCfgById(itemData.roleid).name;
        nameRoot.SetActive(true);
        //异步加载对应角色模型
        mAssetRequest= await ZMAsset.InstantiateAsync(AssetPathConfig.Hall_ROLE_PATH+ "Role_"+itemData.roleid);
        mRoleObj = mAssetRequest.obj;
        mRoleObj.transform.ReSetParent(roleModelParent);
    }
   
    /// <summary>
    /// 设置Item选中状态
    /// </summary>
    public void SetSelectState(bool isSelect)
    {
        selectObj.SetActive(isSelect);
    }
    public void OnSelectButtonClick()
    {
        if (mRoleObj == null) 
        {
            return;
        }
        //记录上一次选择的Item 单独指定隐藏上次选中的状态
        if (mWindow != null)
            mWindow.HideAllItemSelect();
        SetSelectState(true);
        HallWorld.UserData.CacheSelectRole(mRoleData);
    }
    public void SetRoleActive(bool active)
    {
        mRoleObj?.SetActive(active);
    }
    /// <summary>
    /// 资源释放
    /// </summary>
    public void OnRelease()
    {
        if (mAssetRequest != null)
        {
            mAssetRequest.Release();
            mAssetRequest = null;
        }
    }
}
