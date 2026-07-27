/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/27 15:25:25
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class HallButtonsWidowDataComponent:MonoBehaviour
	{
		public   Button  MainCityButton;

		public   Button  HerosButton;

		public   Button  BackPackButton;

		public   Button  PVELevelButton;

		public   Button  CarbonButton;

		public   Button  TradeUnionButton;

		public   Button  ForeignAidButton;

		public   Button  ChatButton;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     HallButtonsWidow mWindow=(HallButtonsWidow)target;
		     target.AddButtonClickListener(MainCityButton,mWindow.OnMainCityButtonClick);
		     target.AddButtonClickListener(HerosButton,mWindow.OnHerosButtonClick);
		     target.AddButtonClickListener(BackPackButton,mWindow.OnBackPackButtonClick);
		     target.AddButtonClickListener(PVELevelButton,mWindow.OnPVELevelButtonClick);
		     target.AddButtonClickListener(CarbonButton,mWindow.OnCarbonButtonClick);
		     target.AddButtonClickListener(TradeUnionButton,mWindow.OnTradeUnionButtonClick);
		     target.AddButtonClickListener(ForeignAidButton,mWindow.OnForeignAidButtonClick);
		     target.AddButtonClickListener(ChatButton,mWindow.OnChatButtonClick);
		}
	}
}
