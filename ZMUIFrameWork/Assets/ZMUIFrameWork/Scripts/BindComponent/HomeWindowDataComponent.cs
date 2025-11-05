/*------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:SWC
 *Data:2025/11/5 15:30:16
 *Description: 变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件数据脚本即可
 *注意:生成会覆盖原有代码
------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using ZMUIFrameWork.Scripts.Runtime.Base;
using ZMUIFrameWork.Scripts.Window;

namespace ZMUIFrameWork
{
	public class HomeWindowDataComponent : MonoBehaviour
	{
		public Button closeButton;

		public void InitComponent(WindowBase target)
		{
			//组件事件绑定
			HomeWindow mWindow = (HomeWindow)target;
			target.AddButtonClickListener(closeButton, mWindow.OnCloseButtonClick);
		}
	}
}
