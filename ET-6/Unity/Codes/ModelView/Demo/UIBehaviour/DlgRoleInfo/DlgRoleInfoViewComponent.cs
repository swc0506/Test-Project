
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgRoleInfoViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Text E_CombatEffectivenessText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CombatEffectivenessText == null )
     			{
		    		this.m_E_CombatEffectivenessText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"BackGround/TopBack/E_CombatEffectiveness");
     			}
     			return this.m_E_CombatEffectivenessText;
     		}
     	}

		public ES_AttributeItem ES_AttributeItem
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_es_attributeitem == null )
     			{
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"BackGround/AttributeBackGround/AttributeAddGroup/ES_AttributeItem");
		    	   this.m_es_attributeitem = this.AddChild<ES_AttributeItem,Transform>(subTrans);
     			}
     			return this.m_es_attributeitem;
     		}
     	}

		public ES_AttributeItem ES_AttributeItem1
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_es_attributeitem1 == null )
     			{
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"BackGround/AttributeBackGround/AttributeAddGroup/ES_AttributeItem1");
		    	   this.m_es_attributeitem1 = this.AddChild<ES_AttributeItem,Transform>(subTrans);
     			}
     			return this.m_es_attributeitem1;
     		}
     	}

		public ES_AttributeItem ES_AttributeItem2
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_es_attributeitem2 == null )
     			{
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"BackGround/AttributeBackGround/AttributeAddGroup/ES_AttributeItem2");
		    	   this.m_es_attributeitem2 = this.AddChild<ES_AttributeItem,Transform>(subTrans);
     			}
     			return this.m_es_attributeitem2;
     		}
     	}

		public ES_AttributeItem ES_AttributeItem3
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_es_attributeitem3 == null )
     			{
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"BackGround/AttributeBackGround/AttributeAddGroup/ES_AttributeItem3");
		    	   this.m_es_attributeitem3 = this.AddChild<ES_AttributeItem,Transform>(subTrans);
     			}
     			return this.m_es_attributeitem3;
     		}
     	}

		public UnityEngine.UI.Text E_AttributePointText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_AttributePointText == null )
     			{
		    		this.m_E_AttributePointText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"BackGround/AttributeBackGround/E_AttributePoint");
     			}
     			return this.m_E_AttributePointText;
     		}
     	}

		public UnityEngine.UI.LoopVerticalScrollRect E_AttributesLoopVerticalScrollRect
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_AttributesLoopVerticalScrollRect == null )
     			{
		    		this.m_E_AttributesLoopVerticalScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopVerticalScrollRect>(this.uiTransform.gameObject,"BackGround/AttributeInfoGroup/E_Attributes");
     			}
     			return this.m_E_AttributesLoopVerticalScrollRect;
     		}
     	}

		public UnityEngine.UI.Button E_CloseButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CloseButton == null )
     			{
		    		this.m_E_CloseButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"E_Close");
     			}
     			return this.m_E_CloseButton;
     		}
     	}

		public UnityEngine.UI.Image E_CloseImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CloseImage == null )
     			{
		    		this.m_E_CloseImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_Close");
     			}
     			return this.m_E_CloseImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_CombatEffectivenessText = null;
			this.m_es_attributeitem?.Dispose();
			this.m_es_attributeitem = null;
			this.m_es_attributeitem1?.Dispose();
			this.m_es_attributeitem1 = null;
			this.m_es_attributeitem2?.Dispose();
			this.m_es_attributeitem2 = null;
			this.m_es_attributeitem3?.Dispose();
			this.m_es_attributeitem3 = null;
			this.m_E_AttributePointText = null;
			this.m_E_AttributesLoopVerticalScrollRect = null;
			this.m_E_CloseButton = null;
			this.m_E_CloseImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_E_CombatEffectivenessText = null;
		private ES_AttributeItem m_es_attributeitem = null;
		private ES_AttributeItem m_es_attributeitem1 = null;
		private ES_AttributeItem m_es_attributeitem2 = null;
		private ES_AttributeItem m_es_attributeitem3 = null;
		private UnityEngine.UI.Text m_E_AttributePointText = null;
		private UnityEngine.UI.LoopVerticalScrollRect m_E_AttributesLoopVerticalScrollRect = null;
		private UnityEngine.UI.Button m_E_CloseButton = null;
		private UnityEngine.UI.Image m_E_CloseImage = null;
		public Transform uiTransform = null;
	}
}
