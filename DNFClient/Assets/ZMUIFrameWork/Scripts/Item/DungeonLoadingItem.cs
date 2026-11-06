using Fantasy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;
public class DungeonLoadingItem : MonoBehaviour
{
    public Image headImage;
    public Image headFrameImage;
    public Image sliderImage;
    public Text nickNameText;
    public Text levelText;
    private RoleData mRoleData;
    public void SetItemData(RoleData roleData, float progress)
    {
        UIEventControl.AddEvent(UIEventEnum.DungeonProgressUpdate, OnDungeonProgress);
        mRoleData= roleData;
        levelText.text = "Lv."+ roleData.level;
        nickNameText.text = roleData.roleName;
        headImage.sprite = ZMAsset.LoadSprite(AssetPathConfig.GAME_TEXTURES_PATH+ $"HeadIcon/{roleData.roleid}");
        sliderImage.fillAmount = progress;
    }
    public void OnDungeonProgress(object data)
    {
        if (data != null)
        {
            //这里会有装箱和拆箱操作，提供此方式是为了在特殊场景下提供便利性，酌情使用
            LoadDungeonProgresssMessage loadData = (LoadDungeonProgresssMessage)data;
            if (mRoleData.accountid == loadData.playerid)
            {
                sliderImage.fillAmount = loadData.progress;
            }
        }
    }
    public void OnRelease()
    {
        UIEventControl.RemoveEvent(UIEventEnum.DungeonProgressUpdate, OnDungeonProgress);
        ZMAsset.Release(gameObject,true);
    }
}
