using System.Collections.Generic;

namespace ET
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgRole :Entity,IAwake,IUILogic
	{

		public DlgRoleViewComponent View { get => this.Parent.GetComponent<DlgRoleViewComponent>();}

		public Dictionary<int, Scroll_Item_Role> ScrollItemRoleDict;


    }
}
