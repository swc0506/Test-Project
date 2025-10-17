/*------------------------------
 *Title:UI自动化组件查找代码生成工具
 *Author:SWC
 *Data:2025/10/17 16:09:59
 *Description: 变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件查找脚本即可
 *注意:生成会覆盖原有代码
------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using ZMUIFrameWork.Scripts.Runtime.Base;
using ZMUIFrameWork.Scripts.Window;

namespace ZMUIFrameWork
{
	public class TempWindowUIComponent
	{
		public Button closeButton;

		public void InitComponent(WindowBase target)
		{
			//组件查找
			closeButton = (Button)target.Transform.GetComponent<Button>();
	
			//组件事件绑定
			TempWindow mWindow = (TempWindow)target;
			target.AddButtonClickListener(closeButton, mWindow.OnCloseButtonClick);
		}
	}
}
