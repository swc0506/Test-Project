using Fantasy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class TeamRoleItem : MonoBehaviour
{
    public Image headImage;
    public Text LevelText;
    public Text nickNameText;
    
    public void SetItemData(RoleData role)
    {
        headImage.sprite = ZMAsset.LoadSprite(AssetPathConfig.GAME_TEXTURES_PATH+ $"HeadIcon/{role.roleid}");
        LevelText.text = "Lv."+role.level;
        nickNameText.text = role.roleName;
    }

    public void Release()
    {
        ZMAsset.Release(gameObject);
    }
}
