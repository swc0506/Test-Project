using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class RoleSelectItem : MonoBehaviour
{
    public int roleid;
    public Transform noSelectMask;
    public Image roleIconImage;
    public Image roleNameImage;

    public void SetItemData(int roleid)
    {
        this.roleid = roleid;
        roleIconImage.sprite =
            ZMAssetsFrame.LoadPNGAtlasSprite(AssetPathConfig.Hall_TEXTURES_PATH + "CreateRole/p_UI_Creat",
                GetHeroIconName());
        roleNameImage.sprite =
            ZMAssetsFrame.LoadPNGAtlasSprite(AssetPathConfig.Hall_TEXTURES_PATH + "CreateRole/p_UI_Creat",
                GetHeroName());
    }

    public void SetSelectState(bool active)
    {
        noSelectMask.gameObject.SetActive(active);
    }

    public string GetHeroIconName()
    {
        if (roleid == 1000)
        {
            return "UI_Chuangjiao_Guijianshi_Di";
        }
        else if (roleid == 1001)
        {
            return "UI_Chuangjiao_Shenqiangshou_Di";
        }

        return "";
    }

    public string GetHeroName()
    {
        if (roleid == 1000)
        {
            return "UI_Chuangjiao_Liemozhe_Zi";
        }
        else if (roleid == 1001)
        {
            return "UI_Chuangjiao_Shenqiangshou_Zi";
        }

        return "";
    }

    public void OnRuleSelectButtonClick()
    {
        UIModule.Instance.GetWindow<CreateRuleWindow>()?.SelectRoleUpdate(roleid);
    }
}