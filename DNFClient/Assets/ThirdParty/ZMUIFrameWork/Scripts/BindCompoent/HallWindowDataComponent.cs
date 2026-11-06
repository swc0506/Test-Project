/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2024/7/7 14:45:04
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace ZMUIFrameWork
{
	public class HallWindowDataComponent:MonoBehaviour
	{
		public   Button  HeadButton;

		public   Button  DailyButton;

		public   Button  CultivateButton;

		public   Toggle  TaskToggle;

		public   Button  TransferButton;

		public   Button  ChallengeButton;

		public   JoystickUGUI  RoleJoystickUGUI;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     HallWindow mWindow=(HallWindow)target;
		     target.AddButtonClickListener(HeadButton,mWindow.OnHeadButtonClick);
		     target.AddButtonClickListener(DailyButton,mWindow.OnDailyButtonClick);
		     target.AddButtonClickListener(CultivateButton,mWindow.OnCultivateButtonClick);
		     target.AddToggleClickListener(TaskToggle,mWindow.OnTaskToggleChange);
		     target.AddButtonClickListener(TransferButton,mWindow.OnTransferButtonClick);
		     target.AddButtonClickListener(ChallengeButton,mWindow.OnChallengeButtonClick);
		}
	}
}
