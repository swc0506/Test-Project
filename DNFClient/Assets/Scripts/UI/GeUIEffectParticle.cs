using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FixMath;

namespace UnityEngine.UI
{
    public class GeUIParticle
    {
        public Vector2 Position;
        public Vector2 LastEmitPos;
        public float InitSize;
        public float Size;
        public Vector2 Velocity;
        public float SpinVel;
        public float Life;
        public float NTLife;
        public float Rotation;
        public Color Color;
        public float Alpha;
        public int TexFrame;
        public int TexFrameRate;
        public int TimeStampMS;
        public int LastFrameMS;

        public GeUIParticleEmitterBase Emiter = null;
        public GeUIEffectGeo m_GeometryData = null;
    }

    public enum EUIAnimMode
    {
        Once,
        Loop,
    }
    

    public enum EColorChannel
    {
        Red,
        Green,
        Blue,
        Alpha,

        MaxChannelNum,
    }

    public class GeUIEffectParticle : Graphic, ISerializationCallbackReceiver
    {
        public class GeUIAtlasTexture
        {
            public Texture[] textureArray
            {
                get
                {
                    return m_TextureArray;
                }
                set
                {
                    bool isSame = true;
                    if (null != m_TextureArray)
                    {
                        if (m_TextureArray.Length == value.Length)
                        {
                            for (int i = 0; i < value.Length; ++i)
                            {
                                if (m_TextureArray[i] != value[i])
                                    isSame = false;
                            }
                        }
                        else
                            isSame = false;
                    }
                    else
                        isSame = false;

                    if (isSame && !m_HasDroped)
                        return;

                    m_TextureArray = null;
                    m_AtlasTexture = null;
                    m_AtlasUVs.Clear();

                    m_IsGenerated = false;
                    m_TextureArray = value;
                    if (value.Length > 1)
                        CreateTextureAtlas();
                    else if (1 == value.Length)
                    {
                        m_AtlasTexture = value[0];
                    }
                    m_HasDroped = false;
                }
            }

            public Texture texture
            {
                get { return m_AtlasTexture; }
            }

            public bool hasDroped
            {
                get { return m_HasDroped; }
            }

            public List<Vector4> atlasUVs
            {
                get { return m_AtlasUVs; }
            }

            private Texture[] m_TextureArray = null;
            private Texture[] DEFAULT_TEX = new Texture[] { s_WhiteTexture };


            private Texture m_AtlasTexture = null;

            private List<Vector4> m_AtlasUVs = new List<Vector4>();
            private bool m_HasDroped = false;
            private bool m_IsGenerated = false;

            public void CreateTextureAtlas()
            {
                float TilesOffset = 2;
                List<int> validTextures = new List<int>();
                Vector2 TextureAtlasSize = new Vector2(TilesOffset, TilesOffset);
                m_AtlasUVs.Clear();

                float greaterHeight = 0;
                for (int i = 0; i < m_TextureArray.Length; i++)
                {
                    if (m_TextureArray != null)
                    {
                        if (greaterHeight < m_TextureArray[i].height) greaterHeight = m_TextureArray[i].height + (TilesOffset * 2);
                        TextureAtlasSize = new Vector2(TextureAtlasSize.x + m_TextureArray[i].width + TilesOffset, greaterHeight);
                        validTextures.Add(i);
                    }
                }

                Texture2D newTextureAtlas = new Texture2D(Mathf.RoundToInt(TextureAtlasSize.x), Mathf.RoundToInt(TextureAtlasSize.y), TextureFormat.ARGB32, false);
                newTextureAtlas.name = "GeUIParticleAtlasTex";
                Color fillColor = Color.clear;
                Color[] fillPixels = new Color[newTextureAtlas.width * newTextureAtlas.height];
                for (int i = 0; i < fillPixels.Length; i++)
                {
                    fillPixels[i] = fillColor;
                }
                newTextureAtlas.SetPixels(fillPixels);

                int currentXPosition = Mathf.RoundToInt(TilesOffset);
                for (int i = 0; i < validTextures.Count; i++)
                {
                    RenderTexture previous = RenderTexture.active;
                    Texture currentTexture = m_TextureArray[validTextures[i]];
                    RenderTexture tempReadableTexture = RenderTexture.GetTemporary(currentTexture.width, currentTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(currentTexture, tempReadableTexture);
                    RenderTexture.active = tempReadableTexture;
                    newTextureAtlas.ReadPixels(new Rect(0, 0, tempReadableTexture.width, tempReadableTexture.height), currentXPosition, Mathf.RoundToInt(TilesOffset));
                    newTextureAtlas.Apply();

                    Vector4 newUV = new Vector4((currentXPosition - TilesOffset / 2) / newTextureAtlas.width, 0, (currentXPosition - TilesOffset / 2 + m_TextureArray[i].width) / newTextureAtlas.width, 1);
                    m_AtlasUVs.Add(newUV);

                    RenderTexture.ReleaseTemporary(tempReadableTexture);
                    RenderTexture.active = previous;
                    currentXPosition += currentTexture.width + Mathf.RoundToInt(TilesOffset);
                }

                if (null != m_AtlasTexture && m_IsGenerated)
                {
                    Texture.Destroy(m_AtlasTexture);
                }
                m_IsGenerated = true;
                m_AtlasTexture = newTextureAtlas;
            }

            public void DropTextureAtlas()
            {
                m_TextureArray = DEFAULT_TEX;
                m_AtlasTexture = s_WhiteTexture;
                m_IsGenerated = false;
                m_AtlasUVs.Clear();
                m_HasDroped = true;
            }
        }


        #region 粒子通用属性

        [SerializeField]
        protected bool m_PlayOnAwake = true;

        [SerializeField]
        protected UnityEngine.Material m_EffMaterial;

        [SerializeField]
        protected Texture[] m_TextureArray;

        [SerializeField]
        protected bool m_IsAnimatedTex = false;
        [SerializeField]
        protected EUIAnimMode m_AnimMode = EUIAnimMode.Loop;

        [SerializeField]
        protected float m_LifeTime = 2.0f;
        [SerializeField]
        protected float m_LifeTimeRangeRate = 0;

        [SerializeField]
        protected Color m_ParticleColor = Color.white;
        [SerializeField]
        protected float[] m_ColorRangeRate = new float[(int)EColorChannel.MaxChannelNum] { 0, 0, 0, 0 };
        [SerializeField]
        protected Gradient m_ColorRamp = new Gradient();
        float m_ColorRampColKeysBeginTime = 0.0f;
        float m_ColorRampColKeysEndTime = 0.0f;
        float m_ColorRampAlphaKeysBeginTime = 0.0f;
        float m_ColorRampAlphaKeysEndTime = 0.0f;
        [SerializeField]
        protected AnimationCurve m_AlphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        float m_AlphaCurveBeginTime = 0.0f;
        float m_AlphaCurveEndTime = 0.0f;
        [SerializeField]
        private bool m_UseLifeColor = false;

        [SerializeField]
        protected float m_Size = 10;
        [SerializeField]
        protected float m_SizeRangeRate = 0;
        [SerializeField]
        private AnimationCurve m_SizeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        float m_SizeCurveBeginTime = 0.0f;
        float m_SizeCurveEndTime = 0.0f;

        [SerializeField]
        protected float m_Speed = 5;
        [SerializeField]
        protected float m_SpeedRangeRate = 0;
        /// 自旋速度
        [SerializeField]
        protected float m_SpinVelocity = 0;
        [SerializeField]
        protected float m_SpinVelRangeValue = 0;

        [SerializeField]
        protected float m_Rotate = 0;
        [SerializeField]
        protected float m_RotateRangeValue = 0;
        /// 为1完全跟随初速度方向
        [SerializeField]
        protected bool m_AlignedSpeed = false;
        [SerializeField]
        protected bool m_VectorParticle = false;
        [SerializeField]
        protected float m_VectorScalar = 1.0f;

        [SerializeField]
        protected Vector2 m_Gravity = Vector2.zero;
        [SerializeField]
        protected Vector2 m_WaveFreq = Vector2.zero;
        [SerializeField]
        protected Vector2 m_WaveAmplitude = Vector2.zero;
        [SerializeField]
        protected Vector2 m_TurbulenceFreq = Vector2.zero;
        [SerializeField]
        protected Vector2 m_TurbulenceAmplitude = Vector2.zero;

        [SerializeField]
        protected uint m_FrameCellX = 1;
        [SerializeField]
        protected uint m_FrameCellY = 1;
        [SerializeField]
        protected uint m_FrameNum = 1;
        [SerializeField]
        protected float m_FrameRate = 30;
        [SerializeField]
        protected bool m_AlignToLife = true;

        [SerializeField]
        protected EUIEffEmitShape m_EmitterShape = EUIEffEmitShape.Point;
        [SerializeField]
        protected string[] m_EmitDataBlockDesc = new string[0];

        protected GeUIEffectDataBlock[] m_EmitDataBlock = null;
        protected GeUIAtlasTexture m_AtlasTexture = new GeUIAtlasTexture();
        protected GeUIParticleEmitterBase m_ParticleEmitter = null;

        protected bool bHasRebuild = false;
        protected bool bUnderRebuild = false;

        protected int m_UpdateTick = 0;
        protected int UPDATE_STEP = 2;

        public GeUIParticleEmitterBase emitter
        {
            get { return m_ParticleEmitter; }
        }

        public GeUIParticleEmitterBase SafeGetEmitter()
        {
            if (null == m_ParticleEmitter)
                _Init();
            return m_ParticleEmitter;
        }
        
		public override Texture mainTexture
        {
            get
            {
#if UNITY_EDITOR
                m_AtlasTexture.textureArray = m_TextureArray;
#endif
                return null == m_AtlasTexture.texture ? s_WhiteTexture : m_AtlasTexture.texture;
            }
        }

        #endregion

        #region 粒子管理
        [SerializeField]
        int m_CurParticleNum = 0;

        List<GeUIParticle> m_Particles = new List<GeUIParticle>();
        List<GeUIParticle> m_RecycleParticles = new List<GeUIParticle>();
        GeUIEffectGeoPool m_UIEffectGeo = new GeUIEffectGeoPool();
        
        int m_CurTimeInMS = 0;
        int m_DelayTimeInMS = 0;
        int m_DeltaMS = 0;
        int m_LastEmission = 0;
        bool m_bFirstUpdate = true;

        [SerializeField]
        bool m_IsPlaying = false;
        [SerializeField]
        bool m_IsActive = false;

        #endregion

        #region 外部函数

        public void StartEmit()
        {
            if(null == m_ParticleEmitter)
                _Init();

            m_IsPlaying = true;
            m_IsActive = true;
            m_CurTimeInMS = 0;
            m_DelayTimeInMS = (int)(m_ParticleEmitter.delayEmit * 1000);
            m_LastEmission = 0;
            m_bFirstUpdate = true;
            m_CurParticleNum = m_Particles.Count;
            Vector3 pos = rectTransform.position;
        }

        public void StopEmit()
        {
            m_IsPlaying = true;
            m_IsActive = false;
        }

        public void RestartEmit()
        {
            StopEmit();
            StartEmit();
        }

        public void PauseEmit()
        {
            m_IsActive = false;
        }

        public void ResumeEmit()
        {
            m_IsActive = true;
        }

        public void Freeze()
        {
            m_IsPlaying = false;
        }

        public void Unfreeze()
        {
            m_IsPlaying = true;
        }

        public void ClearParticles()
        {
            _ClearAllParticles();
        }

        #endregion

        #region 内部函数

        private void _Init()
        {
            _RebuildEmitter();

            if(null == m_TextureArray)
                m_TextureArray = new Texture[] { s_WhiteTexture };

            if(null != m_SizeCurve)
            {
                if (m_SizeCurve.length > 0)
                {
                    m_SizeCurveBeginTime = m_SizeCurve.keys[0].time;
                    m_SizeCurveEndTime = m_SizeCurve.keys[m_SizeCurve.length - 1].time;
                }
            }

            if(null != m_AlphaCurve)
            {
                if (m_AlphaCurve.length > 0)
                {
                    m_AlphaCurveBeginTime = m_AlphaCurve.keys[0].time;
                    m_AlphaCurveEndTime = m_AlphaCurve.keys[m_AlphaCurve.length - 1].time;
                }
            }

            if(null != m_ColorRamp)
            {
                if (m_ColorRamp.colorKeys.Length > 0)
                {
                    m_ColorRampColKeysBeginTime = m_ColorRamp.colorKeys[0].time;
                    m_ColorRampColKeysEndTime = m_ColorRamp.colorKeys[m_ColorRamp.colorKeys.Length - 1].time;
                }
                if (m_ColorRamp.alphaKeys.Length > 0)
                {
                    m_ColorRampAlphaKeysBeginTime = m_ColorRamp.alphaKeys[0].time;
                    m_ColorRampAlphaKeysEndTime = m_ColorRamp.alphaKeys[m_ColorRamp.alphaKeys.Length - 1].time;
                }
            }

            m_AtlasTexture.textureArray = m_TextureArray;
            m_UIEffectGeo.Init(4);
        }

        protected void _Deinit()
        {
            _ClearAllParticles();
            m_UIEffectGeo.Deinit();
        }

        protected void _Update()
        {
            if (m_IsPlaying == false)
                return;
            
            if (m_bFirstUpdate)
            {
                m_DeltaMS = 0;
                m_bFirstUpdate = false;
            }

            if(m_DelayTimeInMS > 0)
            {
                m_DelayTimeInMS -= m_DeltaMS;
                return;
            }

            m_CurTimeInMS += m_DeltaMS;
            if (null != m_ParticleEmitter)
            {
                if (m_CurTimeInMS >= m_ParticleEmitter.durationMS && m_ParticleEmitter.durationMS > 0)
                {
                    m_IsActive = false;
                }

                m_LastEmission += m_DeltaMS;
                if (m_LastEmission >= 100.0f / m_ParticleEmitter.emitRate && m_IsActive == true)
                {
                    m_LastEmission = 0;
                    _AddParticle();
                }
            }

            SetVerticesDirty();
            SetMaterialDirty();
        }

        protected void _RebuildEmitter()
        {
            GeUIParticleEmitterBase emitter = null;
            switch (m_EmitterShape)
            {
                case EUIEffEmitShape.Point:
                    emitter = new GeUIParticleEmitterPoint();
                    break;
                case EUIEffEmitShape.Circle:
                    emitter = new GeUIParticleEmitterCircle();
                    break;
                case EUIEffEmitShape.Rect:
                    emitter = new GeUIParticleEmitterRect();
                    break;
                case EUIEffEmitShape.Segment:
                    emitter = new GeUIParticleEmitterSegment();
                    break;
                case EUIEffEmitShape.Directional:
                    emitter = new GeUIParticleEmitterDirectional();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }


            if (null != emitter)
            {
                GeUIEffectDataBlock[] emitDataBlock = GeUIEffectDataBlockSerializer.FromString(m_EmitDataBlockDesc);
                emitter.LoadDataBlock(emitDataBlock);
            }
            else
                global::Debuger.LogWarning("Unsupported emitter shape type!");

            m_ParticleEmitter = emitter;
        }

        protected GeUIParticle _AllocParticle()
        {
            GeUIParticle newParticle = null;
            if (m_RecycleParticles.Count > 0)
            {
                newParticle = m_RecycleParticles[m_RecycleParticles.Count - 1];
                m_RecycleParticles.RemoveAt(m_RecycleParticles.Count - 1);
                
                return newParticle;
            }

            newParticle = new GeUIParticle();
            newParticle.m_GeometryData = m_UIEffectGeo.AllocGeometry();
            newParticle.Emiter = m_ParticleEmitter;

            return newParticle;
        }

        private void _AddParticle()
        {
            int particlesToEmit = m_ParticleEmitter.partiPerEmission;
            if (m_ParticleEmitter.maxParticles > 0 && m_CurParticleNum + particlesToEmit > m_ParticleEmitter.maxParticles)
                particlesToEmit = m_ParticleEmitter.maxParticles - m_CurParticleNum;

            for (int i = 0; i < particlesToEmit; i++)
            {
                GeUIParticle newParticle = _AllocParticle();
                //float partSize = m_Size * m_SizeCurve.Evaluate(m_SizeCurve.keys[0].time);
                float partSize = m_Size * m_SizeCurve.Evaluate(m_SizeCurveBeginTime);
                newParticle.InitSize = m_Size + m_Size * Random.Range(-m_SizeRangeRate, m_SizeRangeRate);
                newParticle.Size = newParticle.InitSize;
                newParticle.NTLife = 1.0f;
                newParticle.Life = m_LifeTime + m_LifeTime * Random.Range(-m_LifeTimeRangeRate, m_LifeTimeRangeRate);

                newParticle.SpinVel = m_SpinVelocity + Random.Range(- m_SpinVelRangeValue, m_SpinVelRangeValue);
                newParticle.Rotation = m_Rotate + Random.Range(-m_RotateRangeValue, m_RotateRangeValue);
                //newParticle.Alpha = m_ParticleColor.a * m_AlphaCurve.Evaluate(m_AlphaCurve.keys[0].time);
                newParticle.Alpha = m_ParticleColor.a * m_AlphaCurve.Evaluate(m_AlphaCurveBeginTime);
                if (false == m_UseLifeColor)
                    newParticle.Color = new Color(m_ParticleColor.r, m_ParticleColor.g, m_ParticleColor.b, newParticle.Alpha);
                else
                {
                    //newParticle.Color = m_ColorRamp.Evaluate(m_ColorRamp.colorKeys[0].time);
                    newParticle.Color = m_ColorRamp.Evaluate(m_ColorRampColKeysBeginTime);
                }

                Color partColor = new Color(
                    newParticle.Color.r + Random.Range(- m_ColorRangeRate[0], m_ColorRangeRate[0]),
                    newParticle.Color.g + Random.Range(- m_ColorRangeRate[1], m_ColorRangeRate[1]),
                    newParticle.Color.b + Random.Range(- m_ColorRangeRate[2], m_ColorRangeRate[2]),
                    newParticle.Alpha);

                newParticle.Color = partColor;

                float partSpeed = m_Speed + m_Speed * Random.Range(- m_SpeedRangeRate, m_SpeedRangeRate);

                if (m_AtlasTexture.textureArray.Length > 0)
                    newParticle.TexFrame = 0;// Random.Range(0, m_Texture.textureArray.Length);

                if(null != m_ParticleEmitter)
                    m_ParticleEmitter.Emit(partSpeed, ref newParticle);

                newParticle.LastEmitPos = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y);

                newParticle.TimeStampMS = m_CurTimeInMS;
                m_Particles.Add(newParticle);
            }

            m_CurParticleNum = m_Particles.Count;
        }

        private void _RemoveParticle(GeUIParticle particle)
        {
            m_Particles.Remove(particle);
            m_RecycleParticles.Add(particle);
            m_CurParticleNum = m_Particles.Count;
        }

        private void _ClearAllParticles()
        {
            m_Particles.RemoveAll(
                p => 
                {
                    m_RecycleParticles.Add(p);
                    return true;
                });

            m_CurParticleNum = 0;
        }

        private Canvas _GetCanvas()
        {
            GameObject selectedGo = this.gameObject;

            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas;

            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas;

            return null;
        }

        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[2];

        private Rect _GetRectCanvasSpace(RectTransform rectTransform)
        {
            if(null == corners)
                corners = new Vector3[4];
            if(null == screenCorners)
                screenCorners = new Vector3[2];

            Canvas canvas = _GetCanvas();

            rectTransform.GetWorldCorners(corners);

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
                screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
            }
            else
            {
                screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
                screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
            }

            float xMin = screenCorners[0].x - (canvas.GetComponent<RectTransform>().sizeDelta.x * rectTransform.pivot.x);
            float yMin = screenCorners[0].y - (canvas.GetComponent<RectTransform>().sizeDelta.y * rectTransform.pivot.y);
            float xMax = screenCorners[1].x - screenCorners[0].x;
            float yMax = screenCorners[1].y - screenCorners[0].y;

            return new Rect(xMin, yMin, xMax, yMax);
        }

        //int[] verticesIndex = new int[0];
        //
        //protected void _SetMesh(VertexHelper vh, Vector2[] vertices, Vector2[] uvs, Color partColor)
        //{
        //    if(verticesIndex.Length < vertices.Length)
        //        verticesIndex = new int[vertices.Length];
        //    
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        var vert = UIVertex.simpleVert;
        //        vert.color = partColor;
        //        vert.position = vertices[i];
        //        vert.uv0 = uvs[i];
        //        vh.AddVert(vert);
        //    
        //        verticesIndex[i] = vh.currentVertCount - 1;
        //    }
        //    
        //    vh.AddTriangle(verticesIndex[0], verticesIndex[1], verticesIndex[2]);
        //    vh.AddTriangle(verticesIndex[2], verticesIndex[3], verticesIndex[0]);
        //}

        //List<UIVertex> verticesList = new List<UIVertex>(4);
        //int[] verticesIndex = new int[4];
        //List<int> indicesList = new List<int>(6);
        //
        //protected void _SetMesh(VertexHelper vh, Vector2[] vertices, Vector2[] uvs, Color partColor)
        //{
        //    verticesList.Clear();
        //    indicesList.Clear();
        //
        //    if (vertices.Length == 4)
        //    {
        //        for (int i = 0; i < vertices.Length; i++)
        //        {
        //            var vert = UIVertex.simpleVert;
        //            vert.color = partColor;
        //            vert.position = vertices[i];
        //            vert.uv0 = uvs[i];
        //
        //            verticesList.Add(vert);
        //            verticesIndex[i] = vh.currentVertCount + i;
        //        }
        //
        //        indicesList.Add(verticesIndex[0]);
        //        indicesList.Add(verticesIndex[1]);
        //        indicesList.Add(verticesIndex[2]);
        //        indicesList.Add(verticesIndex[2]);
        //        indicesList.Add(verticesIndex[3]);
        //        indicesList.Add(verticesIndex[0]);
        //
        //        vh.AddUIVertexStream(verticesList, indicesList);
        //    }
        //}

        //private Vector3 _RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        //{
        //    Vector3 dir = point - pivot;
        //    dir = Quaternion.Euler(angles) * dir;
        //    point = dir + pivot;
        //    return point;
        //}

        private Vector3 _RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion quat)
        {
            Vector3 dir = point - pivot;
            dir = quat * dir;
            point = dir + pivot;
            return point;
        }

        private Vector3 _RotatePointAroundPivotOpt(Vector3 point, Vector3 pivot, float sinAngle,float cosAngle)
        {
            float dirX = point.x - pivot.x;
            float dirY = point.y - pivot.y;

            float newX = cosAngle * dirX - sinAngle * dirY;
            float newY = sinAngle * dirX + cosAngle * dirY;

            point.x = newX + pivot.x;
            point.y = newY + pivot.y;

            return point;
        }

        private void _UpdateTime(float deltaTime)
        {
            m_DeltaMS = (int)(deltaTime * 1000);
            //m_DeltaMS = 33;
        }

        private Vector2 _ToCanvasPos(Vector2 pos)
        {
            Vector2 res = new Vector2(pos.x - rectTransform.localPosition.x + rectTransform.position.x,
                                      pos.y - rectTransform.localPosition.y + rectTransform.position.y);

            return res;
        }
        private Vector2 _ToEmitPos(Vector2 pos)
        {
            Vector2 res = new Vector2(pos.x + rectTransform.localPosition.x - rectTransform.position.x,
                                      pos.y + rectTransform.localPosition.y - rectTransform.position.y);

            return res;
        }

        #endregion

        #region Behavior函数
        public virtual void OnBeforeSerialize()
        {

        }

        public virtual void OnAfterDeserialize()
        {
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            m_IsPlaying = false;
            m_IsActive = false;
            base.Start();

            if (true == m_PlayOnAwake )
                StartEmit();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (null == m_AtlasTexture.textureArray || m_AtlasTexture.hasDroped)
            {
                if (null == m_TextureArray)
                    m_TextureArray = new Texture[] { s_WhiteTexture };

                m_AtlasTexture.textureArray = m_TextureArray;
            }

            if (null == m_Particles)
                m_Particles = new List<GeUIParticle>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_AtlasTexture.DropTextureAtlas();
        }

        protected override void OnDestroy()
        {
            if (null != m_ParticleEmitter)
            {
                m_ParticleEmitter.SaveDataBlock(ref m_EmitDataBlock);
                m_EmitDataBlockDesc = GeUIEffectDataBlockSerializer.ToString(m_EmitDataBlock);
            }

            m_Particles.Clear();
            m_RecycleParticles.Clear();
            m_UIEffectGeo.Clear();
        }

        void Update()
        {
            bHasRebuild = false;
#if UNITY_EDITOR
            if (Application.isPlaying)
                _UpdateTime(Time.deltaTime);
#else
            _UpdateTime(Time.deltaTime);
#endif
            _Update();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position + new Vector3(0, -5, 0), transform.position + new Vector3(0, 5, 0));
            Gizmos.DrawLine(transform.position + new Vector3(-5, 0, 0), transform.position + new Vector3(5, 0, 0));
        }

        //         protected override void OnPopulateMesh(VertexHelper vh)
        //         {
        //             vh.Clear();
        // 
        // 			int frameTime = (int)(1000 / m_FrameRate);
        //             for (int i = 0; i < m_Particles.Count; i++)
        //             {
        //                 GeUIParticle particle = m_Particles[i];
        // 
        //                 Vector2 part = particle.Position;
        // 
        //                 Vector2 partCorner1 = Vector2.zero;
        //                 Vector2 partCorner2 = Vector2.zero;
        //                 if (m_VectorParticle)
        //                 {
        //                     float vectorSizeX = particle.Size + m_Speed * m_VectorScalar;
        //                     float vectorSizeY = particle.Size;
        // 
        //                     partCorner1 = part + new Vector2(- vectorSizeX * 0.5f, - vectorSizeY * 0.5f);
        //                     partCorner2 = part + new Vector2(  vectorSizeX * 0.5f,   vectorSizeY * 0.5f);
        //                 }
        //                 else
        //                 {
        //                     partCorner1 = part + new Vector2(-particle.Size * 0.5f, -particle.Size * 0.5f);
        //                     partCorner2 = part + new Vector2( particle.Size * 0.5f,  particle.Size * 0.5f);
        //                 }
        // 
        //                 particle.m_GeometryData.Pos[0] = new Vector2(partCorner1.x, partCorner1.y);
        //                 particle.m_GeometryData.Pos[1] = new Vector2(partCorner1.x, partCorner2.y);
        //                 particle.m_GeometryData.Pos[2] = new Vector2(partCorner2.x, partCorner2.y);
        //                 particle.m_GeometryData.Pos[3] = new Vector2(partCorner2.x, partCorner1.y);
        // 
        //                 particle.m_GeometryData.Pos[0] = _RotatePointAroundPivot(particle.m_GeometryData.Pos[0], particle.Position, new Vector3(0, 0, particle.Rotation));
        //                 particle.m_GeometryData.Pos[1] = _RotatePointAroundPivot(particle.m_GeometryData.Pos[1], particle.Position, new Vector3(0, 0, particle.Rotation));
        //                 particle.m_GeometryData.Pos[2] = _RotatePointAroundPivot(particle.m_GeometryData.Pos[2], particle.Position, new Vector3(0, 0, particle.Rotation));
        //                 particle.m_GeometryData.Pos[3] = _RotatePointAroundPivot(particle.m_GeometryData.Pos[3], particle.Position, new Vector3(0, 0, particle.Rotation));
        // 
        //                 if (m_IsAnimatedTex == true)
        //                 {
        //                     int diff = m_CurTimeInMS - particle.TimeStampMS - frameTime;
        //                     if (diff > 0)
        //                     {
        //                         particle.TimeStampMS = m_CurTimeInMS + diff;
        //                         ++particle.TexFrame;
        //                     }
        // 
        //                     if (EUIAnimMode.Once == m_AnimMode)
        //                         particle.TexFrame = (int)IntMath.Clamp((long)particle.TexFrame, 0, (int)m_FrameNum - 1);
        //                     else if (EUIAnimMode.Loop == m_AnimMode)
        //                         particle.TexFrame = particle.TexFrame % (int)m_FrameNum;
        //                     else
        //                         ;
        //                 }
        // 
        //                 if (m_AtlasTexture.textureArray.Length > 1)
        //                 {
        //                     Vector4 textUV;
        // 
        //                     textUV = m_AtlasTexture.atlasUVs[particle.TexFrame];
        //                     Vector2 uvTopLeft = new Vector2(textUV.x, textUV.y);
        //                     Vector2 uvBottomLeft = new Vector2(textUV.x, textUV.w);
        //                     Vector2 uvTopRight = new Vector2(textUV.z, textUV.y);
        //                     Vector2 uvBottomRight = new Vector2(textUV.z, textUV.w);
        //                     particle.m_GeometryData.UV[0] = uvBottomLeft;
        //                     particle.m_GeometryData.UV[1] = uvTopLeft;
        //                     particle.m_GeometryData.UV[2] = uvTopRight;
        //                     particle.m_GeometryData.UV[3] = uvBottomRight;
        //                 }
        //                 else
        //                 {
        //                     if(m_IsAnimatedTex)
        //                     {
        //                         float cellWidth = 1.0f / m_FrameCellX;
        //                         float cellHeight = 1.0f / m_FrameCellY;
        // 
        //                         int X = particle.TexFrame % (int)m_FrameCellX;
        //                         int Y = particle.TexFrame / (int)m_FrameCellX;
        // 
        //                         particle.m_GeometryData.UV[0] = new Vector2(      X * cellWidth,1 - (Y + 1) * cellHeight);
        //                         particle.m_GeometryData.UV[1] = new Vector2(      X * cellWidth,1 -      Y  * cellHeight);
        //                         particle.m_GeometryData.UV[2] = new Vector2((X + 1) * cellWidth,1 -      Y  * cellHeight);
        //                         particle.m_GeometryData.UV[3] = new Vector2((X + 1) * cellWidth,1 - (Y + 1) * cellHeight);
        //                     }
        //                     else
        //                     {
        //                         Vector2 uvTopLeft = Vector2.zero;
        //                         Vector2 uvBottomLeft = new Vector2(0, 1);
        //                         Vector2 uvTopRight = new Vector2(1, 0);
        //                         Vector2 uvBottomRight = new Vector2(1, 1);
        // 
        //                         particle.m_GeometryData.UV[0] = uvTopLeft;
        //                         particle.m_GeometryData.UV[1] = uvBottomLeft;
        //                         particle.m_GeometryData.UV[2] = uvBottomRight;
        //                         particle.m_GeometryData.UV[3] = uvTopRight;
        //                     }
        //                 }
        // 
        //                 _SetMesh(vh, particle.m_GeometryData.Pos, particle.m_GeometryData.UV, particle.Color);
        //             }
        //         }


        List<UIVertex> verticesList = new List<UIVertex>(4);
        List<int> indicesList = new List<int>(6);
        static int[] verticesIndex = new int[4];

        static UIVertex[] verticesCache = new UIVertex[4];
        static int[] indicesCache = new int[6];

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            verticesList.Clear();
            indicesList.Clear();

            int frameTime = (int)(1000 / m_FrameRate);
            for (int i = 0; i < m_Particles.Count; i++)
            {
                GeUIParticle particle = m_Particles[i];

                Vector2 part = particle.Position;

                Vector2 partCorner1 = Vector2.zero;
                Vector2 partCorner2 = Vector2.zero;
                if (m_VectorParticle)
                {
                    float vectorSizeX = particle.Size + m_Speed * m_VectorScalar;
                    float vectorSizeY = particle.Size;

                    partCorner1 = part + new Vector2(- vectorSizeX * 0.5f, - vectorSizeY * 0.5f);
                    partCorner2 = part + new Vector2(  vectorSizeX * 0.5f,   vectorSizeY * 0.5f);
                }
                else
                {
                    partCorner1 = part + new Vector2(-particle.Size * 0.5f, -particle.Size * 0.5f);
                    partCorner2 = part + new Vector2( particle.Size * 0.5f,  particle.Size * 0.5f);
                }

                particle.m_GeometryData.Pos[0] = new Vector2(partCorner1.x, partCorner1.y);
                particle.m_GeometryData.Pos[1] = new Vector2(partCorner1.x, partCorner2.y);
                particle.m_GeometryData.Pos[2] = new Vector2(partCorner2.x, partCorner2.y);
                particle.m_GeometryData.Pos[3] = new Vector2(partCorner2.x, partCorner1.y);

                //Quaternion rotQuat = Quaternion.Euler(new Vector3(0, 0, particle.Rotation));
                //
                //Vector3 Test0 = _RotatePointAroundPivot(particle.m_GeometryData.Pos[0], particle.Position, rotQuat);
                //Vector3 Test1 = _RotatePointAroundPivot(particle.m_GeometryData.Pos[1], particle.Position, rotQuat);
                //Vector3 Test2 = _RotatePointAroundPivot(particle.m_GeometryData.Pos[2], particle.Position, rotQuat);
                //Vector3 Test3 = _RotatePointAroundPivot(particle.m_GeometryData.Pos[3], particle.Position,rotQuat);

                float radian = particle.Rotation * Mathf.Deg2Rad;
                float sinAngle = Mathf.Sin(radian);
                float cosAngle = Mathf.Cos(radian);

                particle.m_GeometryData.Pos[0] = _RotatePointAroundPivotOpt(particle.m_GeometryData.Pos[0], particle.Position,sinAngle ,cosAngle);
                particle.m_GeometryData.Pos[1] = _RotatePointAroundPivotOpt(particle.m_GeometryData.Pos[1], particle.Position,sinAngle ,cosAngle);
                particle.m_GeometryData.Pos[2] = _RotatePointAroundPivotOpt(particle.m_GeometryData.Pos[2], particle.Position,sinAngle ,cosAngle);
                particle.m_GeometryData.Pos[3] = _RotatePointAroundPivotOpt(particle.m_GeometryData.Pos[3], particle.Position,sinAngle ,cosAngle);

                if (m_IsAnimatedTex == true)
                {
                    int diff = m_CurTimeInMS - particle.TimeStampMS - frameTime;
                    if (diff > 0)
                    {
                        particle.TimeStampMS = m_CurTimeInMS + diff;
                        ++particle.TexFrame;
                    }

                    if (EUIAnimMode.Once == m_AnimMode)
                        particle.TexFrame = (int)FixIntMath.Clamp((long)particle.TexFrame, 0, (int)m_FrameNum - 1);
                    else if (EUIAnimMode.Loop == m_AnimMode)
                        particle.TexFrame = particle.TexFrame % (int)m_FrameNum;
                    else
                        ;
                }

                if (m_AtlasTexture.textureArray.Length > 1)
                {
                    Vector4 textUV;

                    textUV = m_AtlasTexture.atlasUVs[particle.TexFrame];
                    Vector2 uvTopLeft = new Vector2(textUV.x, textUV.y);
                    Vector2 uvBottomLeft = new Vector2(textUV.x, textUV.w);
                    Vector2 uvTopRight = new Vector2(textUV.z, textUV.y);
                    Vector2 uvBottomRight = new Vector2(textUV.z, textUV.w);
                    particle.m_GeometryData.UV[0] = uvBottomLeft;
                    particle.m_GeometryData.UV[1] = uvTopLeft;
                    particle.m_GeometryData.UV[2] = uvTopRight;
                    particle.m_GeometryData.UV[3] = uvBottomRight;
                }
                else
                {
                    if(m_IsAnimatedTex)
                    {
                        float cellWidth = 1.0f / m_FrameCellX;
                        float cellHeight = 1.0f / m_FrameCellY;

                        int X = particle.TexFrame % (int)m_FrameCellX;
                        int Y = particle.TexFrame / (int)m_FrameCellX;

                        particle.m_GeometryData.UV[0] = new Vector2(      X * cellWidth,1 - (Y + 1) * cellHeight);
                        particle.m_GeometryData.UV[1] = new Vector2(      X * cellWidth,1 -      Y  * cellHeight);
                        particle.m_GeometryData.UV[2] = new Vector2((X + 1) * cellWidth,1 -      Y  * cellHeight);
                        particle.m_GeometryData.UV[3] = new Vector2((X + 1) * cellWidth,1 - (Y + 1) * cellHeight);
                    }
                    else
                    {
                        Vector2 uvTopLeft = Vector2.zero;
                        Vector2 uvBottomLeft = new Vector2(0, 1);
                        Vector2 uvTopRight = new Vector2(1, 0);
                        Vector2 uvBottomRight = new Vector2(1, 1);

                        particle.m_GeometryData.UV[0] = uvTopLeft;
                        particle.m_GeometryData.UV[1] = uvBottomLeft;
                        particle.m_GeometryData.UV[2] = uvBottomRight;
                        particle.m_GeometryData.UV[3] = uvTopRight;
                    }
                }

                for (int v = 0; v < 4 ; ++v)
                {
                    var vert = UIVertex.simpleVert;
                    vert.color = particle.Color;
                    vert.position = particle.m_GeometryData.Pos[v];
                    vert.uv0 = particle.m_GeometryData.UV[v];

                    verticesCache[v] = vert;
                    verticesIndex[v] = verticesList.Count + v;
                }

                verticesList.AddRange(verticesCache);

                indicesCache[0] = verticesIndex[0];
                indicesCache[1] = verticesIndex[1];
                indicesCache[2] = verticesIndex[2];
                indicesCache[3] = verticesIndex[2];
                indicesCache[4] = verticesIndex[3];
                indicesCache[5] = verticesIndex[0];

                indicesList.AddRange(indicesCache);
            }

            vh.AddUIVertexStream(verticesList, indicesList);
        }

        public override void Rebuild(CanvasUpdate update)
        {
            if (CanvasUpdate.PreRender != update)
                return;

            if (bHasRebuild)
                return;

            if (bUnderRebuild)
                return;

            bUnderRebuild = true;
            float timeNow = m_CurTimeInMS * 0.001f;
            float deltaTime = m_DeltaMS * 0.001f;

            for (int i = 0; i < m_Particles.Count; i++)
            {
                GeUIParticle curParticle = m_Particles[i];

                Vector2 waveForce = new Vector2(
                                                  Mathf.Sin(timeNow * m_WaveFreq.x) * (m_WaveAmplitude.x),
                                                  Mathf.Sin(timeNow * m_WaveFreq.y) * (m_WaveAmplitude.y));
                Vector2 turbulenceForce = new Vector2(
                                                      (Mathf.PerlinNoise(timeNow * m_TurbulenceFreq.x, 0) * (m_TurbulenceAmplitude.x)) - m_TurbulenceAmplitude.x * 0.5f,
                                                      (Mathf.PerlinNoise(timeNow * m_TurbulenceFreq.y, 0) * (m_TurbulenceAmplitude.y)) - m_TurbulenceAmplitude.y * 0.5f);
                Vector2 gravityForce = m_Gravity;
                Vector2 totalForce = gravityForce + waveForce;

                curParticle.Velocity += totalForce;
                Vector2 oldPosition = curParticle.Position;

                if(m_ParticleEmitter.relative)
                {
                    curParticle.Position += (curParticle.Velocity * deltaTime);
                }
                else
                {
                    Vector2 deltaPos = curParticle.LastEmitPos;
                    curParticle.LastEmitPos = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y);
                    deltaPos -= curParticle.LastEmitPos;

                    curParticle.Position += deltaPos;
                    curParticle.Position += ((curParticle.Velocity + deltaPos) * deltaTime);
                }

                curParticle.Position += turbulenceForce;

                curParticle.NTLife = (curParticle.NTLife * curParticle.Life - deltaTime) / curParticle.Life;

                curParticle.Rotation += curParticle.SpinVel * deltaTime;

                if (true == m_AlignedSpeed)
                {
                    float MovingAngle = Mathf.Atan2(curParticle.Position.y - oldPosition.y, curParticle.Position.x - oldPosition.x) * 180 / Mathf.PI;
                    curParticle.Rotation = MovingAngle;
                }
                
                //curParticle.Size = curParticle.InitSize * m_SizeCurve.Evaluate((1.0f - curParticle.NTLife) * m_SizeCurve.keys[m_SizeCurve.length - 1].time);
                curParticle.Size = curParticle.InitSize * m_SizeCurve.Evaluate((1.0f - curParticle.NTLife) * m_SizeCurveEndTime);
                //curParticle.Alpha = m_ParticleColor.a * m_AlphaCurve.Evaluate((1.0f - curParticle.NTLife) * m_AlphaCurve.keys[m_AlphaCurve.length - 1].time);
                curParticle.Alpha = m_ParticleColor.a * m_AlphaCurve.Evaluate((1.0f - curParticle.NTLife) * m_AlphaCurveEndTime);

                if (m_UseLifeColor == false)
                    curParticle.Color = new Color(curParticle.Color.r, curParticle.Color.g, curParticle.Color.b, curParticle.Alpha);
                else
                {
                    //Color newParticleColor = m_ColorRamp.Evaluate((1.0f - curParticle.NTLife) * m_ColorRamp.colorKeys[m_ColorRamp.colorKeys.Length - 1].time);
                    Color newParticleColor = m_ColorRamp.Evaluate((1.0f - curParticle.NTLife) * m_ColorRampColKeysEndTime);
                    curParticle.Color = new Color(newParticleColor.r, newParticleColor.g, newParticleColor.b, curParticle.Alpha);
                }
                if (curParticle.NTLife <= 0 && curParticle.NTLife != 0)
                    _RemoveParticle(curParticle);
            }

            if (null != m_EffMaterial)
            {
                m_EffMaterial.mainTexture = mainTexture;
                material = m_EffMaterial;
            }

            ++m_UpdateTick;
            if (UPDATE_STEP == m_UpdateTick)
            {
                m_UpdateTick = 0;
                base.Rebuild(update);
            }

            bHasRebuild = true;
            bUnderRebuild = false;
        }

#if UNITY_EDITOR
        public void RebuildEmitter()
        {
            _RebuildEmitter();
        }
        public void RebuildAtlas()
        {
            m_AtlasTexture.DropTextureAtlas();
        }
        public void ReCookCurves()
        {
            if (null != m_SizeCurve)
            {
                if (m_SizeCurve.length > 0)
                {
                    m_SizeCurveBeginTime = m_SizeCurve.keys[0].time;
                    m_SizeCurveEndTime = m_SizeCurve.keys[m_SizeCurve.length - 1].time;
                }
            }

            if (null != m_AlphaCurve)
            {
                if (m_AlphaCurve.length > 0)
                {
                    m_AlphaCurveBeginTime = m_AlphaCurve.keys[0].time;
                    m_AlphaCurveEndTime = m_AlphaCurve.keys[m_AlphaCurve.length - 1].time;
                }
            }

            if (null != m_ColorRamp)
            {
                if (m_ColorRamp.colorKeys.Length > 0)
                {
                    m_ColorRampColKeysBeginTime = m_ColorRamp.colorKeys[0].time;
                    m_ColorRampColKeysEndTime = m_ColorRamp.colorKeys[m_ColorRamp.colorKeys.Length - 1].time;
                }
                if (m_ColorRamp.alphaKeys.Length > 0)
                {
                    m_ColorRampAlphaKeysBeginTime = m_ColorRamp.alphaKeys[0].time;
                    m_ColorRampAlphaKeysEndTime = m_ColorRamp.alphaKeys[m_ColorRamp.alphaKeys.Length - 1].time;
                }
            }
        }

        public void EditorPlay()
        {
            StartEmit();
        }

        public void EditorStop()
        {
            StopEmit();
        }

        public void UpdateFromEditor(float deltaT)
        {
            GraphicRegistry.RegisterGraphicForCanvas(canvas, this);
            GraphicRebuildTracker.TrackGraphic(this);

            //m_IsActive = true;
            //m_IsPlaying = true;

            //_UpdateTime(0.001f);
            //
            //m_CurTimeInMS += m_DeltaMS;
            //
            //_AddParticle();

            _UpdateTime(deltaT);

            //Rebuild(CanvasUpdate.PreRender);
            //SetAllDirty();
            SetVerticesDirty();
            SetMaterialDirty();

            Vector3 pos = gameObject.transform.localPosition;
            pos.z += 0.001f;
            gameObject.transform.localPosition = pos;
            pos.z -= 0.001f;
            gameObject.transform.localPosition = pos;

            //Canvas.ForceUpdateCanvases();
        }

#endif

        #endregion
    }
}
