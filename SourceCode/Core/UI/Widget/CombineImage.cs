using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    [AddComponentMenu("UI/Extensions/Combine Image")]
    public class CombineImage : MaskableGraphic
    {
        [System.Serializable]
        private enum SlicingType
        {
            Half,
            Quarter,
        }

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

        [SerializeField] private bool m_PreserveAspect = false;

        public bool preserveAspect
        {
            get { return m_PreserveAspect; }
            set
            {
                if (m_PreserveAspect != value)
                {
                    m_PreserveAspect = value;
                    SetVerticesDirty();
                }
            }
        }


        [SerializeField] private SlicingType slicingType = SlicingType.Half;


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


        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = sprite != null ? UnityEngine.Sprites.DataUtility.GetPadding(sprite) : Vector4.zero;
            var size = new Vector2(mainTexture.width, mainTexture.height);

            Rect r = GetPixelAdjustedRect();

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                float spriteRatio = 0; //图片实际大小
                float rectRatio = r.width / r.height;
                if (slicingType == SlicingType.Half)
                {
                    spriteRatio = size.x * 2 / size.y;
                }
                else
                {
                    spriteRatio = size.x / size.y;
                }

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                r.x + r.width * v.x,
                r.y + r.height * v.y,
                r.x + r.width * v.z,
                r.y + r.height * v.w
            );

            return v;
        }


        /// <summary>
        /// Generate vertices for a half Image.
        /// </summary>
        void GenerateHalfSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            Vector4 uv = sprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : new Vector4(0, 0, 1, 1);

            var color32 = color;
            vh.Clear();

            //计算宽高;  
            float width = v.z - v.x;
            //float height = v.w - v.y;

            //左边顶点;  
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z - width * 0.5f, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z - width * 0.5f, v.y), color32, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            //右边顶点;  
            vh.AddVert(new Vector3(v.x + width * 0.5f, v.y), color32, new Vector2(uv.z, uv.y));
            vh.AddVert(new Vector3(v.x + width * 0.5f, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.x, uv.y));

            vh.AddTriangle(4, 5, 6);
            vh.AddTriangle(6, 7, 4);
        }


        /// <summary>
        /// Generate vertices for a quarter Image.
        /// </summary>
        void GenerateQuarterSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            Vector4 uv = sprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : new Vector4(0, 0, 1, 1);

            var color32 = color;
            vh.Clear();

            //计算宽高;  
            float width = v.z - v.x;
            float height = v.w - v.y;

            //左上角顶点;  
            vh.AddVert(new Vector3(v.x, v.y + height / 2), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z - width / 2, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z - width / 2, v.y + height / 2), color32, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            //左下角顶点;  
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.x, v.w - height / 2), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.z - width / 2, v.w - height / 2), color32, new Vector2(uv.z, uv.y));
            vh.AddVert(new Vector3(v.z - width / 2, v.y), color32, new Vector2(uv.z, uv.w));

            vh.AddTriangle(4, 5, 6);
            vh.AddTriangle(6, 7, 4);

            //右下角顶点;  
            vh.AddVert(new Vector3(v.x + width / 2, v.y), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.x + width / 2, v.w - height / 2), color32, new Vector2(uv.z, uv.y));
            vh.AddVert(new Vector3(v.z, v.w - height / 2), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.x, uv.w));

            vh.AddTriangle(8, 9, 10);
            vh.AddTriangle(10, 11, 8);

            //右上角顶点;  
            vh.AddVert(new Vector3(v.x + width / 2, v.y + height / 2), color32, new Vector2(uv.z, uv.y));
            vh.AddVert(new Vector3(v.x + width / 2, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z, v.y + height / 2), color32, new Vector2(uv.x, uv.y));

            vh.AddTriangle(12, 13, 14);
            vh.AddTriangle(14, 15, 12);
        }

        /// <summary>
        /// Update the UI renderer mesh.
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (mainTexture == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            if (slicingType == SlicingType.Half)
            {
                GenerateHalfSprite(toFill, m_PreserveAspect);
            }
            else if (slicingType == SlicingType.Quarter)
            {
                GenerateQuarterSprite(toFill, m_PreserveAspect);
            }
        }


        public float pixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (sprite)
                    spritePixelsPerUnit = sprite.pixelsPerUnit;

                float referencePixelsPerUnit = 100;
                if (canvas)
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;

                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }

        public override void SetNativeSize()
        {
            float w = mainTexture.width / pixelsPerUnit;
            float h = mainTexture.height / pixelsPerUnit;
            rectTransform.anchorMax = rectTransform.anchorMin;
            if (slicingType == SlicingType.Half)
            {
                rectTransform.sizeDelta = new Vector2(w * 2, h);
            }
            else if (slicingType == SlicingType.Quarter)
            {
                rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
            }

            SetAllDirty();
        }
    }
}