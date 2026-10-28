
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgRoleViewComponentAwakeSystem : AwakeSystem<DlgRoleViewComponent> 
	{
		public override void Awake(DlgRoleViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgRoleViewComponentDestroySystem : DestroySystem<DlgRoleViewComponent> 
	{
		public override void Destroy(DlgRoleViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
