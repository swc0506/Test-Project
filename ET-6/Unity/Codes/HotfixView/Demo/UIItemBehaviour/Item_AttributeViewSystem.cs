
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class Scroll_Item_AttributeDestroySystem : DestroySystem<Scroll_Item_Attribute> 
	{
		public override void Destroy( Scroll_Item_Attribute self )
		{
			self.DestroyWidget();
		}
	}
}
