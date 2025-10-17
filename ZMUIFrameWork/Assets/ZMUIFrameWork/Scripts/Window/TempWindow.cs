/*------------------------------
 *Title:UI表现层脚本自动化代码生成工具
 *Author:SWC
 *Data:2025/10/17 16:10:02
 *Description: 表现层只负责页面逻辑与交互，不允许编写业务逻辑代码
 *注意:生成不会覆盖原有代码，会进行新增操作
------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using ZMUIFrameWork;
using ZMUIFrameWork.Scripts.Runtime.Base;

namespace ZMUIFrameWork.Scripts.Window
{
	public class TempWindow : WindowBase
	{
		public TempWindowUIComponent uiComp = new TempWindowUIComponent();
	
		#region 声明周期函数
		//调用机制与Mono Awake一致
		public override void OnAwake()
		{
			base.OnAwake();
			 uiComp.InitComponent(this);
		}
		//物体显示时执行
		public override void OnShow()
		{
			base.OnShow();
		}
		//物体隐藏时执行
		public override void OnHide()
		{
			base.OnHide();
		}
		//物体销毁时执行
		public override void OnDestroy()
		{
			base.OnDestroy();
		}
		#endregion

		#region API Function
		
		#endregion

		#region UI组件事件
		public void OnCloseButtonClick()
		{
			HideWindow();
		}
		#endregion
	}
}
