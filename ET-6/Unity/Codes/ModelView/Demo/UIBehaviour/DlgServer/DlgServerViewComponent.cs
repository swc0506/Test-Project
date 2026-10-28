
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgServerViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button E_SelectServerButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SelectServerButton == null )
     			{
		    		this.m_E_SelectServerButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Sprite_BackGround/E_SelectServer");
     			}
     			return this.m_E_SelectServerButton;
     		}
     	}

		public UnityEngine.UI.Image E_SelectServerImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SelectServerImage == null )
     			{
		    		this.m_E_SelectServerImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Sprite_BackGround/E_SelectServer");
     			}
     			return this.m_E_SelectServerImage;
     		}
     	}

		public UnityEngine.UI.LoopVerticalScrollRect ELoopScrollList_ServersLoopVerticalScrollRect
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoopScrollList_ServersLoopVerticalScrollRect == null )
     			{
		    		this.m_ELoopScrollList_ServersLoopVerticalScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopVerticalScrollRect>(this.uiTransform.gameObject,"Sprite_BackGround/ELoopScrollList_Servers");
     			}
     			return this.m_ELoopScrollList_ServersLoopVerticalScrollRect;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_SelectServerButton = null;
			this.m_E_SelectServerImage = null;
			this.m_ELoopScrollList_ServersLoopVerticalScrollRect = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Button m_E_SelectServerButton = null;
		private UnityEngine.UI.Image m_E_SelectServerImage = null;
		private UnityEngine.UI.LoopVerticalScrollRect m_ELoopScrollList_ServersLoopVerticalScrollRect = null;
		public Transform uiTransform = null;
	}
}
