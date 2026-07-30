/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/30 18:24:41
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
	public class LevelWindowDataComponent:MonoBehaviour
	{
		public   Button  Level1Button;

		public   Button  Level2Button;

		public   Button  Level3Button;

		public   Button  Level4Button;

		public   Button  Level5Button;

		public   Button  Level6Button;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LevelWindow mWindow=(LevelWindow)target;
		     target.AddButtonClickListener(Level1Button,mWindow.OnLevel1ButtonClick);
		     target.AddButtonClickListener(Level2Button,mWindow.OnLevel2ButtonClick);
		     target.AddButtonClickListener(Level3Button,mWindow.OnLevel3ButtonClick);
		     target.AddButtonClickListener(Level4Button,mWindow.OnLevel4ButtonClick);
		     target.AddButtonClickListener(Level5Button,mWindow.OnLevel5ButtonClick);
		     target.AddButtonClickListener(Level6Button,mWindow.OnLevel6ButtonClick);
		}
	}
}
