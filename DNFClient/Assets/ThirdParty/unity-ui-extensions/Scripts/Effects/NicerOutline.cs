/// Credit Melang
/// Sourced from - http://forum.unity3d.com/members/melang.593409/

using System.Collections.Generic;
namespace UnityEngine.UI.Extensions
{
	//An outline that looks a bit nicer than the default one. It has less "holes" in the outline by drawing more copies of the effect
	[AddComponentMenu("UI/Effects/Extensions/Nicer Outline")]
	public class NicerOutline : BaseMeshEffect
	{
		[SerializeField]
		private Color m_EffectColor = new Color (0f, 0f, 0f, 0.5f);
		
		[SerializeField]
		private Vector2 m_EffectDistance = new Vector2 (1f, -1f);
		
		[SerializeField]
		private bool m_UseGraphicAlpha = true;
        //
        // Properties
        //
        private Text m_FoundText = null;
        static readonly float factor = 1.0f / 255.0f;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_FoundText = GetComponent<Text>();
        }

        public Color effectColor
		{
			get
			{
				return this.m_EffectColor;
			}
			set
			{
				this.m_EffectColor = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}
		
		public Vector2 effectDistance
		{
			get
			{
				return this.m_EffectDistance;
			}
			set
			{
				if (value.x > 600f)
				{
					value.x = 600f;
				}
				if (value.x < -600f)
				{
					value.x = -600f;
				}
				if (value.y > 600f)
				{
					value.y = 600f;
				}
				if (value.y < -600f)
				{
					value.y = -600f;
				}
				if (this.m_EffectDistance == value)
				{
					return;
				}
				this.m_EffectDistance = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}
		
		public bool useGraphicAlpha
		{
			get
			{
				return this.m_UseGraphicAlpha;
			}
			set
			{
				this.m_UseGraphicAlpha = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;

            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                Vector3 v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;
                var newColor = color;
                if (m_UseGraphicAlpha)
                    newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
                vt.color = newColor;
                verts[i] = vt;
            }
        }

        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }

        protected void ApplyShaderEx(List<UIVertex> verts,int num, Color32 color, float xOffset, float yOffset)
        {
            UIVertex vt;
            int needCapacity = verts.Count * 9;
            if (verts.Capacity < needCapacity)
                verts.Capacity = needCapacity;

            UIVertex[] vertCache = ArrayPool<UIVertex>.Instance.AllocateArray(needCapacity);

            if (m_UseGraphicAlpha)
            {
                for (int i = 0, icnt = num; i < icnt; ++i)
                {
                    vt = verts[i];

                    vertCache[i + num * 0] = vt;
                    vertCache[i + num * 0].position += new Vector3(xOffset, yOffset, 0);
                    vertCache[i + num * 0].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 0].color.a * color.a * factor));

                    vertCache[i + num * 1] = vt;
                    vertCache[i + num * 1].position += new Vector3(xOffset, -yOffset, 0);
                    vertCache[i + num * 1].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 1].color.a * color.a * factor));

                    vertCache[i + num * 2] = vt;
                    vertCache[i + num * 2].position += new Vector3(-xOffset, yOffset, 0);
                    vertCache[i + num * 2].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 2].color.a * color.a * factor));

                    vertCache[i + num * 3] = vt;
                    vertCache[i + num * 3].position += new Vector3(-xOffset, -yOffset, 0);
                    vertCache[i + num * 3].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 3].color.a * color.a * factor));

                    vertCache[i + num * 4] = vt;
                    vertCache[i + num * 4].position += new Vector3(xOffset, 0, 0);
                    vertCache[i + num * 4].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 4].color.a * color.a * factor));

                    vertCache[i + num * 5] = vt;
                    vertCache[i + num * 5].position += new Vector3(-xOffset, 0, 0);
                    vertCache[i + num * 5].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 5].color.a * color.a * factor));

                    vertCache[i + num * 6] = vt;
                    vertCache[i + num * 6].position += new Vector3(0, yOffset, 0);
                    vertCache[i + num * 6].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 6].color.a * color.a * factor));

                    vertCache[i + num * 7] = vt;
                    vertCache[i + num * 7].position += new Vector3(0, -yOffset, 0);
                    vertCache[i + num * 7].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 7].color.a * color.a * factor));

                    vertCache[i + num * 8] = vt;
                }
            }
            else
            {
                for (int i = 0, icnt = num; i < icnt; ++i)
                {
                    vt = verts[i];

                    vertCache[i + num * 0] = vt;
                    vertCache[i + num * 0].position += new Vector3(xOffset, yOffset, 0);
                    vertCache[i + num * 0].color = color;

                    vertCache[i + num * 1] = vt;
                    vertCache[i + num * 1].position += new Vector3(xOffset, -yOffset, 0);
                    vertCache[i + num * 1].color = color;

                    vertCache[i + num * 2] = vt;
                    vertCache[i + num * 2].position += new Vector3(-xOffset, yOffset, 0);
                    vertCache[i + num * 2].color = color;

                    vertCache[i + num * 3] = vt;
                    vertCache[i + num * 3].position += new Vector3(-xOffset, -yOffset, 0);
                    vertCache[i + num * 3].color = color;

                    vertCache[i + num * 4] = vt;
                    vertCache[i + num * 4].position += new Vector3(xOffset, 0, 0);
                    vertCache[i + num * 4].color = color;

                    vertCache[i + num * 5] = vt;
                    vertCache[i + num * 5].position += new Vector3(-xOffset, 0, 0);
                    vertCache[i + num * 5].color = color;

                    vertCache[i + num * 6] = vt;
                    vertCache[i + num * 6].position += new Vector3(0, yOffset, 0);
                    vertCache[i + num * 6].color = color;

                    vertCache[i + num * 7] = vt;
                    vertCache[i + num * 7].position += new Vector3(0, -yOffset, 0);
                    vertCache[i + num * 7].color = color;

                    vertCache[i + num * 8] = vt;
                }
            }

            verts.Clear();
            verts.AddRange(vertCache);
            ArrayPool<UIVertex>.Instance.ReleaseArray(vertCache);
        }
        protected void ApplyShaderFast(List<UIVertex> verts, int num, Color32 color, float xOffset, float yOffset)
        {
            UIVertex vt;
            int needCapacity = verts.Count * 5;
            if (verts.Capacity < needCapacity)
                verts.Capacity = needCapacity;

            UIVertex[] vertCache = ArrayPool<UIVertex>.Instance.AllocateArray(needCapacity);

            if (m_UseGraphicAlpha)
            {
                for (int i = 0, icnt = num; i < icnt; ++i)
                {
                    vt = verts[i];

                    vertCache[i + num * 0] = vt;
                    vertCache[i + num * 0].position += new Vector3(xOffset, yOffset, 0);
                    vertCache[i + num * 0].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 0].color.a * color.a * factor));

                    vertCache[i + num * 1] = vt;
                    vertCache[i + num * 1].position += new Vector3(xOffset, -yOffset, 0);
                    vertCache[i + num * 1].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 1].color.a * color.a * factor));

                    vertCache[i + num * 2] = vt;
                    vertCache[i + num * 2].position += new Vector3(-xOffset, yOffset, 0);
                    vertCache[i + num * 2].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 2].color.a * color.a * factor));

                    vertCache[i + num * 3] = vt;
                    vertCache[i + num * 3].position += new Vector3(-xOffset, -yOffset, 0);
                    vertCache[i + num * 3].color = new Color32(color.r, color.g, color.b, (byte)(vertCache[i + num * 3].color.a * color.a * factor));

                    vertCache[i + num * 4] = vt;
                }
            }
            else
            {
                for (int i = 0, icnt = num; i < icnt; ++i)
                {
                    vt = verts[i];

                    vertCache[i + num * 0] = vt;
                    vertCache[i + num * 0].position += new Vector3(xOffset, yOffset, 0);
                    vertCache[i + num * 0].color = color;

                    vertCache[i + num * 1] = vt;
                    vertCache[i + num * 1].position += new Vector3(xOffset, -yOffset, 0);
                    vertCache[i + num * 1].color = color;

                    vertCache[i + num * 2] = vt;
                    vertCache[i + num * 2].position += new Vector3(-xOffset, yOffset, 0);
                    vertCache[i + num * 2].color = color;

                    vertCache[i + num * 3] = vt;
                    vertCache[i + num * 3].position += new Vector3(-xOffset, -yOffset, 0);
                    vertCache[i + num * 3].color = color;

                    vertCache[i + num * 4] = vt;
                }
            }

            verts.Clear();
            verts.AddRange(vertCache);
            ArrayPool<UIVertex>.Instance.ReleaseArray(vertCache);
        }
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive ())
			{
				return;
			}
            List < UIVertex > verts =  GamePool.ListPool<UIVertex>.Get(); //new List<UIVertex>();
            vh.GetUIVertexStream(verts);

            //Text foundtext = GetComponent<Text>();
            //
            //float best_fit_adjustment = 1f;
            //
            //if (foundtext && foundtext.resizeTextForBestFit)  
            //{
            //	best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / (foundtext.resizeTextMaxSize-1); //max size seems to be exclusive 
            //
            //}

            if(null == m_FoundText)
                m_FoundText = GetComponent<Text>();

            float best_fit_adjustment = 1f;

            if (m_FoundText && m_FoundText.resizeTextForBestFit)
            {
                best_fit_adjustment = (float)m_FoundText.cachedTextGenerator.fontSizeUsedForBestFit / (m_FoundText.resizeTextMaxSize - 1); //max size seems to be exclusive 

            }

            float distanceX = this.effectDistance.x * best_fit_adjustment;
			float distanceY = this.effectDistance.y * best_fit_adjustment;

            // 			int start = 0;
            // 			int count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, distanceX, distanceY);
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, distanceX, -distanceY);
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, -distanceX, distanceY);
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, -distanceX, -distanceY);
            // 
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, distanceX, 0);
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, -distanceX, 0);
            // 
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, 0, distanceY);
            // 			start = count;
            // 			count = verts.Count;
            // 			this.ApplyShadow (verts, this.effectColor, start, verts.Count, 0, -distanceY);

            ApplyShaderEx(verts, verts.Count, this.effectColor, distanceX, distanceY);
            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
			GamePool.ListPool<UIVertex>.Release(verts);
        }

#if UNITY_EDITOR
		protected override void OnValidate ()
		{
			this.effectDistance = this.m_EffectDistance;
			base.OnValidate ();
		}
#endif
	}
}
