
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class Scroll_Item_ServerDestroySystem : DestroySystem<Scroll_Item_Server> 
	{
		public override void Destroy( Scroll_Item_Server self )
		{
			self.DestroyWidget();
		}
	}
}
