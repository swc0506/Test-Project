
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[EnableMethod]
	public  class ES_AttributeItem : Entity,ET.IAwake<UnityEngine.Transform>,IDestroy 
	{
		public UnityEngine.UI.Text EAttributeValueText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EAttributeValueText == null )
     			{
		    		this.m_EAttributeValueText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"EAttributeValue");
     			}
     			return this.m_EAttributeValueText;
     		}
     	}

		public UnityEngine.UI.Button EButton_AddButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_AddButton == null )
     			{
		    		this.m_EButton_AddButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Add");
     			}
     			return this.m_EButton_AddButton;
     		}
     	}

		public UnityEngine.UI.Image EButton_AddImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_AddImage == null )
     			{
		    		this.m_EButton_AddImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_Add");
     			}
     			return this.m_EButton_AddImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EAttributeValueText = null;
			this.m_EButton_AddButton = null;
			this.m_EButton_AddImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_EAttributeValueText = null;
		private UnityEngine.UI.Button m_EButton_AddButton = null;
		private UnityEngine.UI.Image m_EButton_AddImage = null;
		public Transform uiTransform = null;
	}
}
