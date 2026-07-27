/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/27 18:14:41
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/

using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class RecruitWindowDataComponent:MonoBehaviour
	{
		public   SkeletonGraphic  choukalihuiSkeletonGraphic;

		public   GameObject  DownHorizationiGameObject;

		public   Button  NormalButton;

		public   Button  FirendButton;

		public   Button  SeniorButton;

		public   GameObject  particleGameObject;

		public   Button  HelpButton;

		public   Button  CloseButton;

		public   Button  JumpButton;

		public   GameObject  chooseGameObject;

		public   GameObject  MaskGameObject;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     RecruitWindow mWindow=(RecruitWindow)target;
		     target.AddButtonClickListener(NormalButton,mWindow.OnNormalButtonClick);
		     target.AddButtonClickListener(FirendButton,mWindow.OnFirendButtonClick);
		     target.AddButtonClickListener(SeniorButton,mWindow.OnSeniorButtonClick);
		     target.AddButtonClickListener(HelpButton,mWindow.OnHelpButtonClick);
		     target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
		     target.AddButtonClickListener(JumpButton,mWindow.OnJumpButtonClick);
		}
	}
}
