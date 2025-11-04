/*------------------------------
 *Title:UI自动化组件查找代码生成工具
 *Author:SWC
 *Data:2025/11/3 20:33:51
 *Description: 变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件查找脚本即可
 *注意:生成会覆盖原有代码
------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using ZMUIFrameWork.Scripts.Runtime.Base;
using ZMUIFrameWork.Scripts.Window;

namespace ZMUIFrameWork
{
	public class PassWindowUIComponent
	{
		public Button closeButton;
		public Text testText;
		public Toggle changeToggle;
		public InputField passInputField;
		public void InitComponent(WindowBase target)
		{
			//组件查找
			closeButton = target.Transform.Find("UIContent/[Button]Close").GetComponent<Button>();
			testText = target.Transform.Find("UIContent/[Text]Test").GetComponent<Text>();
			changeToggle = target.Transform.Find("UIContent/[Toggle]Change").GetComponent<Toggle>();
			passInputField = target.Transform.Find("UIContent/[InputField]Pass").GetComponent<InputField>();
	
			//组件事件绑定
			PassWindow mWindow = (PassWindow)target;
			target.AddButtonClickListener(closeButton, mWindow.OnCloseButtonClick);
			target.AddToggleClickListener(changeToggle, mWindow.OnChangeToggleChange);
			target.AddInputFieldListener(passInputField, mWindow.OnPassInputChange, mWindow.OnPassInputEnd);
		}
	}
}
