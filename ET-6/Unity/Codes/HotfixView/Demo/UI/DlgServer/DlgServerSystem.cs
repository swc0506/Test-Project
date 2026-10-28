using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;

namespace ET
{
    [FriendClass(typeof(DlgServer))]
    [FriendClassAttribute(typeof(ET.ServerInfosComponent))]
    [FriendClassAttribute(typeof(ET.ServerInfo))]
    public static class DlgServerSystem
    {

        public static void RegisterUIEvent(this DlgServer self)
        {
            self.View.E_SelectServerButton.AddListenerAsync(() => { return self.OnSelectServerClickHnadler(); });
            self.View.ELoopScrollList_ServersLoopVerticalScrollRect.AddItemRefreshListener((Transform transfrom, int index) =>
            {
                self.OnScrollItemRefreshHandler(transfrom, index);
            });
        }

        public static void ShowWindow(this DlgServer self, Entity contextData = null)
        {
            int count = self.ZoneScene().GetComponent<ServerInfosComponent>().ServerInfoList.Count;
            self.AddUIScrollItems(ref self.ScrollItemServerDict, count);
            self.View.ELoopScrollList_ServersLoopVerticalScrollRect.SetVisible(true, count);
        }

        public static void HideWindow(this DlgServer self)
        {
            self.RemoveUIScrollItems(ref self.ScrollItemServerDict);
        }

        public static async ETTask OnSelectServerClickHnadler(this DlgServer self)
        {
            bool isSelect = self.ZoneScene().GetComponent<ServerInfosComponent>().CurrentServerId != 0;

            if (!isSelect)
            {
                Log.Error("请先选择服务器");
                return;
            }

            try
            {
                int errorCode = await LoginHelper.GetRoles(self.ZoneScene());
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }

                self.ZoneScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Role);
                self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Server);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }

        public static void OnScrollItemRefreshHandler(this DlgServer self, Transform transfrom, int index)
        {
            Scroll_Item_Server scroll_Item_Server = self.ScrollItemServerDict[index].BindTrans(transfrom);
            ServerInfo info = self.ZoneScene().GetComponent<ServerInfosComponent>().ServerInfoList[index];
            scroll_Item_Server.EButton_ServerImage.color = info.Id == self.ZoneScene().GetComponent<ServerInfosComponent>().CurrentServerId ? Color.red : Color.cyan;
            scroll_Item_Server.EText_ServerText.SetText(info.ServerName);
            scroll_Item_Server.EButton_ServerButton.AddListener(() => { self.OnSelectServerItemHnadler(info.Id); });
        }

        public static void OnSelectServerItemHnadler(this DlgServer self, long serverId)
        {
            self.ZoneScene().GetComponent<ServerInfosComponent>().CurrentServerId = int.Parse(serverId.ToString());
            Log.Debug($"当前选择的服务器 Id 是：{serverId}");
            self.View.ELoopScrollList_ServersLoopVerticalScrollRect.RefillCells();
        }
    }
}
