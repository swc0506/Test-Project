/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/8/6 18:42:03
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class RoundWindowDataComponent:MonoBehaviour
	{
		public   GameObject  ReplayRootGameObject;

		public   Button  QuickenButton;

		public   Image  scaleImage;

		public   Button  PauseButton;

		public   Button  JumpButton;

		public   Button  AutoButton;

		public   Text  AutoText;

		public   Text  RoundText;

		public   Text  LogicFrameText;

		public   BattleCardItem[]    RootBattleCardItemArray;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     RoundWindow mWindow=(RoundWindow)target;
		     target.AddButtonClickListener(QuickenButton,mWindow.OnQuickenButtonClick);
		     target.AddButtonClickListener(PauseButton,mWindow.OnPauseButtonClick);
		     target.AddButtonClickListener(JumpButton,mWindow.OnJumpButtonClick);
		     target.AddButtonClickListener(AutoButton,mWindow.OnAutoButtonClick);
		}
	}
}
