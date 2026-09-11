/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/9/10 18:24:08
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，再次生成后会以代码追加的形式新增,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class ReplayItem:MonoBehaviour, IZMUIViewListItem
	{
		#region 自定义字段
		public   Text  timeText;

		public   Button  ReplayButton;

		public   Text  winText;

		private ReplayData itemData;

		#endregion


		#region 生命周期
		
		public void InitListItem()
		{
			//按钮事件自动注册绑定
			ReplayButton.onClick.AddListener(OnReplayButtonClick);
		}

		public void SetListItemShowData(int index, params object[] data)
		{
			itemData = (ReplayData)data[0];
			timeText.text = itemData.battleTime;
			winText.text = itemData.isWin ? "胜利" : "失败";
			ReplayButton.interactable = true;
		}

		public void OnRelease()
		{
			
		}
		
		#endregion


		#region UI组件事件
		private void OnReplayButtonClick()
		{
		
		}

		 #endregion
		 
	}
}
