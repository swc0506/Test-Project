using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[FriendClass(typeof(DlgMain))]
	public static  class DlgMainSystem
	{

		public static void RegisterUIEvent(this DlgMain self)
		{
			self.View.EButton_RoleButton.AddListenerAsync(() => { return self.OnRoleButtonClickHandler(); });
		}

		public static void ShowWindow(this DlgMain self, Entity contextData = null)
		{
			self.Refresh().Coroutine();
		}

		public static async ETTask Refresh(this DlgMain self)
		{
			Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.ZoneScene().CurrentScene());
			NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

			self.View.EText_LevelText.SetText($"Lv.{numericComponent.GetAsInt(NumericType.Level)}");
			self.View.EText_GoldText.SetText($"金币:{numericComponent.GetAsInt(NumericType.Gold)}");
			self.View.EText_EXPText.SetText($"EXP:{numericComponent.GetAsInt(NumericType.Exp)}");

			await ETTask.CompletedTask;
        }

		public static async ETTask OnRoleButtonClickHandler(this DlgMain self)
		{
            //try
            //{
            //	int error = await NumericHelper.TestUpdateNumeric(self.ZoneScene());

            //	if (error != ErrorCode.ERR_Success)
            //	{
            //                 return;
            //	}

            //	Log.Debug("发送更新属性测试消息成功");
            //}
            //catch (Exception e)
            //{
            //	Log.Error(e.ToString());
            //}

            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_RoleInfo);

			await ETTask.CompletedTask;
        }
	}
}
