
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgMainViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button EButton_RoleButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_RoleButton == null )
     			{
		    		this.m_EButton_RoleButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Role");
     			}
     			return this.m_EButton_RoleButton;
     		}
     	}

		public UnityEngine.UI.Image EButton_RoleImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_RoleImage == null )
     			{
		    		this.m_EButton_RoleImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_Role");
     			}
     			return this.m_EButton_RoleImage;
     		}
     	}

		public UnityEngine.UI.Text EText_LevelText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EText_LevelText == null )
     			{
		    		this.m_EText_LevelText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Image_Level/EText_Level");
     			}
     			return this.m_EText_LevelText;
     		}
     	}

		public UnityEngine.UI.Text EText_EXPText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EText_EXPText == null )
     			{
		    		this.m_EText_EXPText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Image_EXP/EText_EXP");
     			}
     			return this.m_EText_EXPText;
     		}
     	}

		public UnityEngine.UI.Text EText_GoldText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EText_GoldText == null )
     			{
		    		this.m_EText_GoldText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Image_Gold/EText_Gold");
     			}
     			return this.m_EText_GoldText;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EButton_RoleButton = null;
			this.m_EButton_RoleImage = null;
			this.m_EText_LevelText = null;
			this.m_EText_EXPText = null;
			this.m_EText_GoldText = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Button m_EButton_RoleButton = null;
		private UnityEngine.UI.Image m_EButton_RoleImage = null;
		private UnityEngine.UI.Text m_EText_LevelText = null;
		private UnityEngine.UI.Text m_EText_EXPText = null;
		private UnityEngine.UI.Text m_EText_GoldText = null;
		public Transform uiTransform = null;
	}
}
