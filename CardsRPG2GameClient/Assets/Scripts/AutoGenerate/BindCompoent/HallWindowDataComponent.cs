/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/27 15:11:30
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class HallWindowDataComponent:MonoBehaviour
	{
		public   Button  RecruitButton;

		public   Button  XianquButton;

		public   Button  DailyTaskButton;

		public   Button  FriendButton;

		public   Button  MailButton;

		public   Button  RankButton;

		public   Text  NickNameText;

		public   Text  goldText;

		public   Button  GoldButton;

		public   Text  diamondText;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     HallWindow mWindow=(HallWindow)target;
		     target.AddButtonClickListener(RecruitButton,mWindow.OnRecruitButtonClick);
		     target.AddButtonClickListener(XianquButton,mWindow.OnXianquButtonClick);
		     target.AddButtonClickListener(DailyTaskButton,mWindow.OnDailyTaskButtonClick);
		     target.AddButtonClickListener(FriendButton,mWindow.OnFriendButtonClick);
		     target.AddButtonClickListener(MailButton,mWindow.OnMailButtonClick);
		     target.AddButtonClickListener(RankButton,mWindow.OnRankButtonClick);
		     target.AddButtonClickListener(GoldButton,mWindow.OnGoldButtonClick);
		}
	}
}
