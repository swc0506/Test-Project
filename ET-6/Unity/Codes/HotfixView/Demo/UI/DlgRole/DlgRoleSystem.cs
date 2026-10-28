using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [FriendClass(typeof(DlgRole))]
    [FriendClassAttribute(typeof(ET.RoleInfoComponent))]
    [FriendClassAttribute(typeof(ET.RoleInfo))]
    public static class DlgRoleSystem
    {

        public static void RegisterUIEvent(this DlgRole self)
        {
            self.View.E_StartGameButton.AddListenerAsync(() => { return self.OnStartGameClickHandler(); });
            self.View.E_CreateRoleButton.AddListenerAsync(() => { return self.OnCreateRoleClickHandler(); });
            self.View.E_DeleteRoleButton.AddListenerAsync(() => { return self.OnDeleteRoleClickHandler(); });
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.AddItemRefreshListener((Transform transform, int index) =>
            {
                self.OnRoleListRefreshHandler(transform, index);
            });
        }

        public static void ShowWindow(this DlgRole self, Entity contextData = null)
        {
            self.RefreshRoleItem();
        }

        public static void HideWindow(this DlgRole self)
        {
            self.RemoveUIScrollItems(ref self.ScrollItemRoleDict);
        }

        public static void RefreshRoleItem(this DlgRole self)
        {
            int count = self.ZoneScene().GetComponent<RoleInfoComponent>().RoleInfos.Count;
            self.AddUIScrollItems(ref self.ScrollItemRoleDict, count);
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.SetVisible(true, count);
        }

        public static async ETTask OnCreateRoleClickHandler(this DlgRole self)
        {
            string name = self.View.E_CreateNameInputField.text;

            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Name is Null");
                return;
            }

            try
            {
                int errorCode = await LoginHelper.CreateRole(self.ZoneScene(), name);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }
                self.RefreshRoleItem();

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }


        public static async ETTask OnDeleteRoleClickHandler(this DlgRole self)
        {
            if (self.ZoneScene().GetComponent<RoleInfoComponent>().CurrentRoleId == 0)
            {
                Log.Error("请选择需要删除的角色");
                return;
            }

            try
            {
                int errorCode = await LoginHelper.DeleteRole(self.ZoneScene());
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }

                self.RefreshRoleItem();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            await ETTask.CompletedTask;

        }

        public static async ETTask OnStartGameClickHandler(this DlgRole self)
        {
            if (self.ZoneScene().GetComponent<RoleInfoComponent>().CurrentRoleId == 0) 
            {
                Log.Error("请选择角色");
                return;
            }

            try
            {
                int errorCode = await LoginHelper.GetRealmKey(self.ZoneScene());
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }

                errorCode = await LoginHelper.EnterGame(self.ZoneScene());
                if(errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }

                self.ZoneScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Main);
                self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Role);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public static void OnRoleListRefreshHandler(this DlgRole self, Transform transform, int index)
        {
            Scroll_Item_Role scroll_Item_Role = self.ScrollItemRoleDict[index].BindTrans(transform);
            RoleInfo info = self.ZoneScene().GetComponent<RoleInfoComponent>().RoleInfos[index];

            scroll_Item_Role.EButton_RoleImage.color = info.Id == self.ZoneScene().GetComponent<RoleInfoComponent>().CurrentRoleId ? Color.red : Color.gray;

            scroll_Item_Role.EText_RoleText.SetText(info.Name);
            scroll_Item_Role.EButton_RoleButton.AddListener(() => { self.OnRoleItemClickHandler(info.Id); });
        }

        public static void OnRoleItemClickHandler(this DlgRole self, long roleId)
        {
            self.ZoneScene().GetComponent<RoleInfoComponent>().CurrentRoleId = roleId;
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.RefillCells();
        }
    }
}
