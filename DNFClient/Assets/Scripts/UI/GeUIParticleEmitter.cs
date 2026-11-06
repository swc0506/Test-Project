using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public enum EUIEffEmitShape
    {
        Point,
        Circle,
        Rect,
        Segment,
        Directional,
    }

    public class Vector2Serializer
    {
        static public Vector2 ToVector2(string value)
        {
            string[] vec2 = value.Split(',');
            return new Vector2(float.Parse(vec2[0]), float.Parse(vec2[1]));
        }

        static public string FromVector2(Vector2 value)
        {
            return string.Format("{0},{1}", value.x, value.y);
        }
    }

    public class RectSerializer
    {
        static public Rect ToRect(string value)
        {
            string[] rect = value.Split(',');
            return new Rect(float.Parse(rect[0]), float.Parse(rect[1]), float.Parse(rect[2]), float.Parse(rect[3]));
        }

        static public string FromRect(Rect value)
        {
            return string.Format("{0},{1},{2},{3}", value.x, value.y, value.width, value.height);
        }
    }

    public class GeUIParticleEmitterBase
    {
        protected EUIEffEmitShape m_EmitterShape = EUIEffEmitShape.Point;

        protected float m_EmitSpeed = 1.0f;
        protected float m_EmitRandomRange = 0.5f;

		protected float m_EmiterRate = 1;
		protected float m_DelayEmit = 0;
        protected int m_PartiPerEmission = 1;
        protected int m_MaxParticles = 20;
        protected int m_NumberOfParticles = 0;
        protected uint m_DurationMS = 0;

        protected bool m_Relative = false;

        private float m_ElapsedEmitTime = 0;

        public bool relative { set { m_Relative = value; } get { return m_Relative; } }
		public float emitRate { set { m_EmiterRate = value; } get { return m_EmiterRate; } }
		public float delayEmit { set { m_DelayEmit = value; } get { return m_DelayEmit; } }
        public int partiPerEmission { set { m_PartiPerEmission = value; } get { return m_PartiPerEmission; } }
        public int maxParticles { set { m_MaxParticles = value; } get { return m_MaxParticles; } }

        public uint durationMS { set { m_DurationMS = value; } get { return m_DurationMS; } }
        public float emiterRate { get { return m_EmiterRate; } }

        public EUIEffEmitShape emitterShape
        {
            get { return m_EmitterShape; }
        }

        public EUIEffEmitShape GetEmitShape()
        {
            return emitterShape;
        }

        public virtual void Emit(float speed, ref GeUIParticle particle)
        {
            particle.Position = Vector2.zero;
            particle.Velocity = Vector2.zero;
        }

        public virtual void LoadDataBlock(GeUIEffectDataBlock[] dataBlock)
        {
            for (int i = 0; i < dataBlock.Length; ++ i)
            {
                switch(dataBlock[i].m_Name)
                {
                    case "m_EmitSpeed":
                        m_EmitSpeed = float.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_EmitRandomRange":
                        m_EmitRandomRange = float.Parse(dataBlock[i].m_Value);
					break;
					case "m_DelayEmit":
						m_DelayEmit = float.Parse(dataBlock[i].m_Value);
					break;
                    case "m_EmiterRate":
                        m_EmiterRate = float.Parse(dataBlock[i].m_Value);
					break;
                    case "m_PartiPerEmission":
                        m_PartiPerEmission = int.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_MaxParticles":
                        m_MaxParticles = int.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_NumberOfParticles":
                        m_NumberOfParticles = int.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_DurationMS":
                        m_DurationMS = uint.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_Relative":
                        m_Relative = bool.Parse(dataBlock[i].m_Value);
                        break;
                    default:
                        continue;
                }
            }
        }
        public virtual void SaveDataBlock(ref GeUIEffectDataBlock[] dataBlock)
        {
            List<GeUIEffectDataBlock> dataBlockList = new List<GeUIEffectDataBlock>();

            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitSpeed"         , m_EmitSpeed.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitRandomRange"   , m_EmitRandomRange.ToString()));
			dataBlockList.Add(new GeUIEffectDataBlock("m_DelayEmit"         , m_DelayEmit.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_EmiterRate"        , m_EmiterRate.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_PartiPerEmission"  , m_PartiPerEmission.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_MaxParticles"      , m_MaxParticles.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_NumberOfParticles" , m_NumberOfParticles.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_DurationMS"        , m_DurationMS.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_Relative"          , m_Relative.ToString()));

            dataBlock = dataBlockList.ToArray();
        }
        public virtual void DrawGizmo(GeUIEffectParticle particle)
        {
        }

        protected Vector2 _GetPointInSegment(Vector2 pointA, Vector2 pointB, float percent)
        {
            Vector2 newPoint;
            newPoint = new Vector2(pointA.x + (percent * (pointB.x - pointA.x)), pointA.y + (percent * (pointB.y - pointA.y)));
            return newPoint;
        }
    }

    public class GeUIParticleEmitterPoint : GeUIParticleEmitterBase
    {
        public GeUIParticleEmitterPoint()
        {
            m_EmitterShape = EUIEffEmitShape.Point;
        }

        public override void Emit(float speed,ref GeUIParticle particle)
        {
            particle.Position = new Vector2(Random.Range(0, 0), Random.Range(0, 0));
            particle.Velocity = new Vector2(Mathf.Sin(Random.Range(0, 360)),
                                            Mathf.Sin(Random.Range(0, 100f))).normalized * speed;
        }

        public override void LoadDataBlock(GeUIEffectDataBlock[] dataBlock)
        {
            base.LoadDataBlock(dataBlock);
        }
        public override void SaveDataBlock(ref GeUIEffectDataBlock[] dataBlock)
        {
            base.SaveDataBlock(ref dataBlock);
        }
    }

    public enum EUIParticleEmitMode
    {
        Edge,
        Vertices,
        Area,
    }

    public enum EUIParticleVelMode
    {
        None,
        Radiant,
        RadiantFlip,
        RadiantTwoSide,
    }

    public class GeUIParticleEmitterCircle : GeUIParticleEmitterBase
    {
        public float radius
        {
            set
            {
                if (value == m_Radius) return;

                m_Radius = value;
                _RebuildVertices(m_CircleSegments, m_Radius);
            }

            get { return m_Radius; } }
        public int circleSegments
        {
            set
            {
                if (value == m_CircleSegments) return;
                m_CircleSegments = value;
                _RebuildVertices(m_CircleSegments, m_Radius);
            }
            get { return m_CircleSegments; }
        }
        public EUIParticleEmitMode emitMode { set { m_EmitMode = value; } get { return m_EmitMode; } }

        public EUIParticleVelMode velocityMode { set { m_VelocityMode = value; } get { return m_VelocityMode; } }

        private EUIParticleVelMode m_VelocityMode = EUIParticleVelMode.None;
        private float m_Radius = 10.0f;
        private int m_CircleSegments = 20;
        private EUIParticleEmitMode m_EmitMode = EUIParticleEmitMode.Area;
        private List<Vector2> m_Vertices = new List<Vector2>();

        public GeUIParticleEmitterCircle()
        {
            m_EmitterShape = EUIEffEmitShape.Circle;
        }

        public override void Emit(float speed, ref GeUIParticle particle)
        {
            switch (m_EmitMode)
            {
                case EUIParticleEmitMode.Area:
                    float degree = Mathf.Deg2Rad * Random.Range(0, 360);
                    float sin = Mathf.Sin(degree);
                    float cos = Mathf.Cos(degree);

                    float rad = m_Radius * Random.Range(0, 1.0f);

                    particle.Position.x = cos * rad;
                    particle.Position.y = sin * rad;
                    break;

                case EUIParticleEmitMode.Edge:
                    int rndP1 = Random.Range(0, m_CircleSegments);
                    int rndP2 = rndP1 + 1;
                    if (rndP2 >= m_CircleSegments) rndP2 = 0;
                    particle.Position = _GetPointInSegment(m_Vertices[rndP1], m_Vertices[rndP2], Random.Range(0, 100) / 100.0f);

                    break;

                case EUIParticleEmitMode.Vertices:
                    particle.Position = m_Vertices[Random.Range(0, m_CircleSegments)];
                    break;
            }

            switch(m_VelocityMode)
            {
                case EUIParticleVelMode.None:
                    particle.Velocity = new Vector2(Random.Range(-1.0f, 1.0f),
                                                    Random.Range(-1.0f, 1.0f)).normalized;
                    break;
                case EUIParticleVelMode.Radiant:
                    particle.Velocity = new Vector2(particle.Position.x,
                                                    particle.Position.y).normalized;
                    break;
                case EUIParticleVelMode.RadiantFlip:
                    particle.Velocity = new Vector2( - particle.Position.x,
                                                     - particle.Position.y).normalized;
                    break;
                case EUIParticleVelMode.RadiantTwoSide:
                    particle.Velocity = new Vector2(particle.Position.x,
                                                    particle.Position.y).normalized;
                    particle.Velocity *= Mathf.Sign( Random.Range(-1.0f, 1.0f));
                    break;
            }

            particle.Velocity.x *= speed;
            particle.Velocity.y *= speed;
        }

        public override void LoadDataBlock(GeUIEffectDataBlock[] dataBlock)
        {
            base.LoadDataBlock(dataBlock);

            for (int i = 0; i < dataBlock.Length; ++i)
            {
                switch (dataBlock[i].m_Name)
                {
                    case "m_Radius":
                        m_Radius = float.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_CircleSegments":
                        m_CircleSegments = int.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_EmitMode":
                        m_EmitMode = (EUIParticleEmitMode)int.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_VelocityMode":
                        m_VelocityMode = (EUIParticleVelMode)int.Parse(dataBlock[i].m_Value);
                        break;
                    default:
                        continue;
                }
            }

            _RebuildVertices(m_CircleSegments, m_Radius);
        }

        public override void SaveDataBlock(ref GeUIEffectDataBlock[] dataBlock)
        {
            base.SaveDataBlock(ref dataBlock);

            List<GeUIEffectDataBlock> dataBlockList = new List<GeUIEffectDataBlock>();

            dataBlockList.AddRange(dataBlock);

            dataBlockList.Add(new GeUIEffectDataBlock("m_Radius", m_Radius.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_CircleSegments", m_CircleSegments.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitMode", ((int)m_EmitMode).ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_VelocityMode", ((int)m_VelocityMode).ToString()));

            dataBlock = dataBlockList.ToArray();
        }

        protected void _RebuildVertices(int sides, float radius)
        {
            m_Vertices.Clear();
            float x, y, t;
            for (int i = 0; i < sides; i++)
            {
                t = 2 * Mathf.PI * ((float)i / (float)sides);
                x = Mathf.Cos(t) * radius;
                y = Mathf.Sin(t) * radius;

                Vector2 vertice = new Vector2(x, y);
                m_Vertices.Add(vertice);
            }
        }
    }

    public class GeUIParticleEmitterRect : GeUIParticleEmitterBase
    {
        public Vector2 dimension { set { m_Dimension = value; } get { return m_Dimension; } }
        public EUIParticleEmitMode emitMode { set { m_EmitMode = value; } get { return m_EmitMode; } }
        public EUIParticleVelMode velocityMode { set { m_VelocityMode = value; } get { return m_VelocityMode; } }

        private Vector2 m_Dimension = new Vector2(100, 100);
        private EUIParticleEmitMode m_EmitMode = EUIParticleEmitMode.Area;
        private EUIParticleVelMode m_VelocityMode = EUIParticleVelMode.None;

        public GeUIParticleEmitterRect()
        {
            m_EmitterShape = EUIEffEmitShape.Rect;
        }

        public override void Emit(float speed, ref GeUIParticle particle)
        {
            switch (m_EmitMode)
            {
                case EUIParticleEmitMode.Area:
                    particle.Position = new Vector2(Random.Range( - m_Dimension.x ,  + m_Dimension.x), Random.Range( - m_Dimension.y, + m_Dimension.y));
                    break;

                case EUIParticleEmitMode.Edge:
                    particle.Position = Vector2.zero;
                    switch (Random.Range(0, 4))
                    {
                        case 0:
                            particle.Position = new Vector2(Random.Range( - m_Dimension.x,  + m_Dimension.x), - m_Dimension.y);
                            break;
                        case 1:
                            particle.Position = new Vector2( - m_Dimension.x, Random.Range( - m_Dimension.y,  + m_Dimension.y));
                            break;
                        case 2:
                            particle.Position = new Vector2(Random.Range( - m_Dimension.x,  + m_Dimension.x), + m_Dimension.y);
                            break;
                        case 3:
                            particle.Position = new Vector2(+ m_Dimension.x, Random.Range( - m_Dimension.y,  + m_Dimension.y));
                            break;
                        default:
                            particle.Position = new Vector2(+ m_Dimension.x, Random.Range( - m_Dimension.y,  + m_Dimension.y));
                            break;
                    }
                    break;

                case EUIParticleEmitMode.Vertices:
                    Vector2[] rectVertices = new Vector2[] {
                        new Vector2(- m_Dimension.x, - m_Dimension.y),
                        new Vector2(- m_Dimension.x, + m_Dimension.y),
                        new Vector2(+ m_Dimension.x, + m_Dimension.y),
                        new Vector2(+ m_Dimension.x, - m_Dimension.y) };
                    particle.Position = rectVertices[Random.Range(0, rectVertices.Length)];
                    break;
            }


            switch (m_VelocityMode)
            {
                case EUIParticleVelMode.None:
                    particle.Velocity = new Vector2(Random.Range(-1.0f, 1.0f),
                                                    Random.Range(-1.0f, 1.0f)).normalized;
                    break;
                case EUIParticleVelMode.Radiant:
                    particle.Velocity = new Vector2(particle.Position.x,
                                                    particle.Position.y).normalized;
                    break;
                case EUIParticleVelMode.RadiantFlip:
                    particle.Velocity = new Vector2(- particle.Position.x,
                                                    - particle.Position.y).normalized;
                    break;
                case EUIParticleVelMode.RadiantTwoSide:
                    particle.Velocity = new Vector2(particle.Position.x,
                                                    particle.Position.y).normalized;
                    particle.Velocity *= Mathf.Sign(Random.Range(-1.0f, 1.0f));
                    break;
            }

            particle.Velocity.x *= speed;
            particle.Velocity.y *= speed;
        }

        public override void LoadDataBlock(GeUIEffectDataBlock[] dataBlock)
        {
            base.LoadDataBlock(dataBlock);

            for (int i = 0; i < dataBlock.Length; ++i)
            {
                switch (dataBlock[i].m_Name)
                {
                    case "m_Dimension":
                        m_Dimension = Vector2Serializer.ToVector2(dataBlock[i].m_Value);
                        break;
                    case "m_EmitMode":
                        m_EmitMode = (EUIParticleEmitMode)int.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_VelocityMode":
                        m_VelocityMode = (EUIParticleVelMode)int.Parse(dataBlock[i].m_Value);
                        break;
                    default:
                        continue;
                }
            }

        }
        public override void SaveDataBlock(ref GeUIEffectDataBlock[] dataBlock)
        {
            base.SaveDataBlock(ref dataBlock);

            List<GeUIEffectDataBlock> dataBlockList = new List<GeUIEffectDataBlock>();

            dataBlockList.AddRange(dataBlock);

            dataBlockList.Add(new GeUIEffectDataBlock("m_Dimension", Vector2Serializer.FromVector2(m_Dimension)));
            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitMode", ((int)m_EmitMode).ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_VelocityMode", ((int)m_VelocityMode).ToString()));

            dataBlock = dataBlockList.ToArray();
        }
    }

    public class GeUIParticleEmitterSegment : GeUIParticleEmitterBase
    {
        public Vector2 segmentBegin { set { m_SegBegin = value; } get { return m_SegBegin; } }
        public Vector2 segmentEnd { set { m_SegEnd = value; } get { return m_SegEnd; } }

        public float emitDirection { set { m_EmitDirection = value; } get { return m_EmitDirection; } }
        public float emitSpread { set { m_EmitSpread = value; } get { return m_EmitSpread; } }

        private float m_EmitDirection = 0.0f;
        private float m_EmitSpread = 0;

        private Vector2 m_SegBegin = Vector2.zero;
        private Vector2 m_SegEnd = Vector2.zero;
        public GeUIParticleEmitterSegment()
        {
            m_EmitterShape = EUIEffEmitShape.Segment;
        }

        public override void LoadDataBlock(GeUIEffectDataBlock[] dataBlock)
        {
            base.LoadDataBlock(dataBlock);
            for (int i = 0; i < dataBlock.Length; ++i)
            {
                switch (dataBlock[i].m_Name)
                {
                    case "m_EmitDirection":
                        m_EmitDirection = float.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_EmitSpread":
                        m_EmitSpread = float.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_SegBegin":
                        m_SegBegin = Vector2Serializer.ToVector2(dataBlock[i].m_Value);
                        break;
                    case "m_SegEnd":
                        m_SegEnd = Vector2Serializer.ToVector2(dataBlock[i].m_Value);
                        break;
                    default:
                        continue;
                }
            }
        }
        public override void SaveDataBlock(ref GeUIEffectDataBlock[] dataBlock)
        {
            base.SaveDataBlock(ref dataBlock);

            List<GeUIEffectDataBlock> dataBlockList = new List<GeUIEffectDataBlock>();

            dataBlockList.AddRange(dataBlock);

            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitDirection", m_EmitDirection.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitSpread", m_EmitSpread.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_SegBegin", Vector2Serializer.FromVector2(m_SegBegin)));
            dataBlockList.Add(new GeUIEffectDataBlock("m_SegEnd", Vector2Serializer.FromVector2(m_SegEnd)));

            dataBlock = dataBlockList.ToArray();
        }

        public override void Emit(float speed, ref GeUIParticle particle)
        {
            particle.Position = _GetPointInSegment(m_SegBegin, m_SegEnd, Random.Range(0, 100) * 0.01f);
            particle.Velocity = new Vector2(Mathf.Sin((Mathf.Deg2Rad * (m_EmitDirection + Random.Range(-m_EmitSpread * 0.5f, m_EmitSpread * 0.5f)))),
                                            Mathf.Cos((Mathf.Deg2Rad * (m_EmitDirection + Random.Range(-m_EmitSpread * 0.5f, m_EmitSpread * 0.5f))))).normalized * speed;
        }
    }

    public class GeUIParticleEmitterDirectional : GeUIParticleEmitterBase
    {
        public float emitDirection { set { m_EmitDirection = value; } get { return m_EmitDirection; } }
        public float emitSpread { set { m_EmitSpread = value; } get { return m_EmitSpread; } }

        private float m_EmitDirection = 0.0f;
        private float m_EmitSpread = 0;

        public GeUIParticleEmitterDirectional()
        {
            m_EmitterShape = EUIEffEmitShape.Directional;
        }

        public override void LoadDataBlock(GeUIEffectDataBlock[] dataBlock)
        {
            base.LoadDataBlock(dataBlock);

            for (int i = 0; i < dataBlock.Length; ++i)
            {
                switch (dataBlock[i].m_Name)
                {
                    case "m_EmitDirection":
                        m_EmitDirection = float.Parse(dataBlock[i].m_Value);
                        break;
                    case "m_EmitSpread":
                        m_EmitSpread = float.Parse(dataBlock[i].m_Value);
                        break;
                    default:
                        continue;
                }
            }

        }
        public override void SaveDataBlock(ref GeUIEffectDataBlock[] dataBlock)
        {
            base.SaveDataBlock(ref dataBlock);

            List<GeUIEffectDataBlock> dataBlockList = new List<GeUIEffectDataBlock>();

            dataBlockList.AddRange(dataBlock);

            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitDirection", m_EmitDirection.ToString()));
            dataBlockList.Add(new GeUIEffectDataBlock("m_EmitSpread", m_EmitSpread.ToString()));

            dataBlock = dataBlockList.ToArray();
        }

        public override void Emit(float speed, ref GeUIParticle particle)
        {
            particle.Position = Vector2.zero;
            particle.Velocity = new Vector2(Mathf.Sin((Mathf.Deg2Rad * (m_EmitDirection + Random.Range(- m_EmitSpread * 0.5f, m_EmitSpread * 0.5f)))),
                                            Mathf.Cos((Mathf.Deg2Rad * (m_EmitDirection + Random.Range(- m_EmitSpread * 0.5f, m_EmitSpread * 0.5f))))).normalized * speed;
        }
    }
}
