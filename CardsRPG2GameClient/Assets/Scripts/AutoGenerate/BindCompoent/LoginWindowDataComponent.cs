/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/22 18:45:13
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class LoginWindowDataComponent:MonoBehaviour
	{
		public   Button  StartGameButton;

		public   Button  SelectServerButton;

		public   Button  AgreementButton;

		public   Button  NoticeButton;

		public   Button  ChangeAccountButton;

		public   Button  ServiceButton;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LoginWindow mWindow=(LoginWindow)target;
		     target.AddButtonClickListener(StartGameButton,mWindow.OnStartGameButtonClick);
		     target.AddButtonClickListener(SelectServerButton,mWindow.OnSelectServerButtonClick);
		     target.AddButtonClickListener(AgreementButton,mWindow.OnAgreementButtonClick);
		     target.AddButtonClickListener(NoticeButton,mWindow.OnNoticeButtonClick);
		     target.AddButtonClickListener(ChangeAccountButton,mWindow.OnChangeAccountButtonClick);
		     target.AddButtonClickListener(ServiceButton,mWindow.OnServiceButtonClick);
		}
	}
}
