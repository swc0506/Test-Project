using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    [AddComponentMenu("UI/Extensions/Circle Image")]
    public class CircleImage : MaskableGraphic
    {
        [SerializeField] private Texture m_Texture;
        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get { return m_Texture; }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetAllDirty();
            }
        }
        
        
        [SerializeField] private Sprite m_Sprite;
        public Sprite sprite
        {
            get { return m_Sprite; }
            set
            {
                if (m_Sprite == value)
                    return;

                m_Sprite = value;
                SetAllDirty();
            }
        }
        

        [Range(0, 1)] [SerializeField] private float m_FillAmount = 1.0f;

        public float fillAmount
        {
            get { return m_FillAmount; }
            set
            {
                if (m_FillAmount != value)
                {
                    m_FillAmount = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField] private bool m_Fill = true;

        public bool fill
        {
            get { return m_Fill; }
            set
            {
                if (m_Fill != value)
                {
                    m_Fill = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField] private float m_Thickness = 5f;

        public float thickness
        {
            get { return m_Thickness; }
            set
            {
                float val = (float) Mathf.Clamp(value, 0, rectTransform.rect.width / 2);
                if (m_Thickness != val)
                {
                    m_Thickness = val;
                    SetVerticesDirty();
                }
            }
        }

        [Range(3, 180)] [SerializeField] private int m_Segments = 60;

        public int segments
        {
            get { return m_Segments; }
            set
            {
                if (m_Segments != value)
                {
                    m_Segments = value;
                    SetVerticesDirty();
                }
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null && m_Sprite == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }

                    return s_WhiteTexture;
                }

                return m_Texture != null ? m_Texture : (m_Sprite != null ? m_Sprite.texture : s_WhiteTexture);
            }
        }
        
        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }

            return vbo;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float outer = -rectTransform.pivot.x * rectTransform.rect.width;
            float inner = -rectTransform.pivot.x * rectTransform.rect.width + this.thickness;

            vh.Clear();

            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;

            float degrees = 360f / segments;
            int fa = (int) ((segments + 1) * this.fillAmount);
            float tw = rectTransform.rect.width;

            for (int i = 0; i < fa; i++)
            {
                float rad = Mathf.Deg2Rad * (i * degrees);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);

                StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos0, out pos1, out pos2, out pos3, c,
                    s);

                uv0 = new Vector2(pos0.x / tw + 0.5f, pos0.y / tw + 0.5f);
                uv1 = new Vector2(pos1.x / tw + 0.5f, pos1.y / tw + 0.5f);
                uv2 = new Vector2(pos2.x / tw + 0.5f, pos2.y / tw + 0.5f);
                uv3 = new Vector2(pos3.x / tw + 0.5f, pos3.y / tw + 0.5f);

                vh.AddUIVertexQuad(SetVbo(new[] {pos0, pos1, pos2, pos3}, new[] {uv0, uv1, uv2, uv3}));
            }
        }

        private void StepThroughPointsAndFill(float outer, float inner, ref Vector2 prevX, ref Vector2 prevY,
            out Vector2 pos0, out Vector2 pos1, out Vector2 pos2, out Vector2 pos3, float c, float s)
        {
            pos0 = prevX;
            pos1 = new Vector2(outer * c, outer * s);

            if (fill)
            {
                pos2 = Vector2.zero;
                pos3 = Vector2.zero;
            }
            else
            {
                pos2 = new Vector2(inner * c, inner * s);
                pos3 = prevY;
            }

            prevX = pos1;
            prevY = pos2;
        }
    }
}