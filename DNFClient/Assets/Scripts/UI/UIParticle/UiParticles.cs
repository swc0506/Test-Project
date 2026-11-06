using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace UiParticles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class UiParticles : MaskableGraphic
    {
        [FormerlySerializedAs("m_ParticleSystem"), SerializeField]
        private ParticleSystem m_ParticleSystem;
        [FormerlySerializedAs("m_RenderMode"), SerializeField, Tooltip("Render mode of particles")]
        private UiParticleRenderMode m_RenderMode;
        [FormerlySerializedAs("m_StretchedSpeedScale"), SerializeField, Tooltip("Speed Scale for streched billboards")]
        private float m_StretchedSpeedScale = 1f;
        [FormerlySerializedAs("m_StretchedLenghScale"), SerializeField, Tooltip("Speed Scale for streched billboards")]
        private float m_StretchedLenghScale = 1f;
        [FormerlySerializedAs("m_IgnoreTimescale"), SerializeField, Tooltip("If true, particles ignore timescale")]
        private bool m_IgnoreTimescale;
        private ParticleSystemRenderer m_ParticleSystemRenderer;
        private ParticleSystem.Particle[] m_Particles;
        public ParticleSystem ParticleSystem
        {
            get
            {
                return this.m_ParticleSystem;
            }
            set
            {
                if (SetPropertyUtility.SetClass<ParticleSystem>(ref this.m_ParticleSystem, value))
                {
                    this.SetAllDirty();
                }
            }
        }
        public override Texture mainTexture
        {
            get
            {
                if (this.material != null && this.material.mainTexture != null)
                {
                    return this.material.mainTexture;
                }
                return Graphic.s_WhiteTexture;
            }
        }
        public UiParticleRenderMode RenderMode
        {
            get
            {
                return this.m_RenderMode;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<UiParticleRenderMode>(ref this.m_RenderMode, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        //Mesh Particle
        private bool initMesh;
        private Mesh particleMesh;
        private Vector3[] vertices;
        private Color[] colors;
        private List<Vector2> uv0s;
        private List<Vector2> animateUV0s;
        private Vector3[] normals;
        private Vector4[] tangent;
        private int[] particleTriangles;

        protected override void Awake()
        {
            ParticleSystem component = base.GetComponent<ParticleSystem>();
            ParticleSystemRenderer component2 = base.GetComponent<ParticleSystemRenderer>();
            /*if (this.m_Material == null)
			{
				this.m_Material = component2.get_sharedMaterial();
			}
			if (component2.get_renderMode() == 1)
			{
				this.RenderMode = UiParticleRenderMode.StreachedBillboard;
			}*/
            if (this.m_Material == null)
            {
                this.m_Material = component2.sharedMaterial;
            }
            if (component2.renderMode == ParticleSystemRenderMode.Stretch)
            {
                this.RenderMode = UiParticleRenderMode.StreachedBillboard;
            }
            if (component2.renderMode == ParticleSystemRenderMode.Mesh)
            {
                this.RenderMode = UiParticleRenderMode.Mesh;
            }
            base.Awake();
            this.ParticleSystem = component;
            this.m_ParticleSystemRenderer = component2;
            this.raycastTarget = false;

            initMesh = false;
        }

        protected override void Start()
        {
            if (!Application.isPlaying)
                return;

            SortingGroup sortgroup = GetComponent<SortingGroup>();
            if (sortgroup != null)
                DestroyImmediate(sortgroup);


            if (this.m_ParticleSystemRenderer != null && this.m_ParticleSystemRenderer.enabled)
            {
                this.m_ParticleSystemRenderer.enabled = false;
            }
        }

        public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();
            /*if (this.m_ParticleSystemRenderer != null)
			{
				this.m_ParticleSystemRenderer.set_sharedMaterial(this.m_Material);
			}*/
            if (this.m_ParticleSystemRenderer != null)
            {
                this.m_ParticleSystemRenderer.sharedMaterial = this.m_Material;
            }
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (this.ParticleSystem == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }
            this.GenerateParticlesBillboards(toFill);
        }
        protected virtual void Update()
        {
            if (!this.m_IgnoreTimescale)
            {
                /*if (this.ParticleSystem != null && this.ParticleSystem.get_isPlaying())
				{
					this.SetVerticesDirty();
				}*/

                if (this.ParticleSystem != null && this.ParticleSystem.isPlaying)
                {
                    this.SetVerticesDirty();
                }
            }
            else
            {
                if (this.ParticleSystem != null)
                {
                    this.ParticleSystem.Simulate(Time.unscaledDeltaTime, true, false);
                    this.SetVerticesDirty();
                }
            }
            /*if (this.m_ParticleSystemRenderer != null && this.m_ParticleSystemRenderer.get_enabled())
			{
				this.m_ParticleSystemRenderer.set_enabled(false);
			}*/
        }
        private void InitParticlesBuffer()
        {
            /*if (this.m_Particles == null || this.m_Particles.Length < this.ParticleSystem.get_main().get_maxParticles())
			{
				this.m_Particles = new ParticleSystem.Particle[this.ParticleSystem.get_main().get_maxParticles()];
			}*/
            if (this.m_Particles == null || this.m_Particles.Length < this.ParticleSystem.main.maxParticles)
            {
                this.m_Particles = new ParticleSystem.Particle[this.ParticleSystem.main.maxParticles];
            }
        }
        private void GenerateParticlesBillboards(VertexHelper vh)
        {
            this.InitParticlesBuffer();
            int particles = this.ParticleSystem.GetParticles(this.m_Particles);
            vh.Clear();

            if (m_RenderMode == UiParticleRenderMode.Billboard || m_RenderMode == UiParticleRenderMode.StreachedBillboard)
            {
                for (int i = 0; i < particles; i++)
                {
                    this.DrawParticleBillboard(this.m_Particles[i], vh);
                }
            }
            else if (m_RenderMode == UiParticleRenderMode.Mesh)
            {
                for (int i = 0; i < particles; i++)
                {
                    this.DrawParticleMesh(this.m_Particles[i], vh);
                }
            }

        }

        private void CheckInitMeshData()
        {
            if (initMesh == false)
            {
                initMesh = true;
                particleMesh = m_ParticleSystemRenderer.mesh;
                vertices = particleMesh.vertices;
                colors = particleMesh.colors == null ? null : particleMesh.colors;
                uv0s = new List<Vector2>();
                particleTriangles = particleMesh.GetTriangles(0);
                particleMesh.GetUVs(0, uv0s);

                if (this.ParticleSystem.textureSheetAnimation.enabled)
                {
                    animateUV0s = new List<Vector2>(uv0s.Count);
                    animateUV0s.AddRange(uv0s);
                }
            }
        }

        private void DrawParticleMesh(ParticleSystem.Particle particle, VertexHelper vh)
        {
            CheckInitMeshData();
            //Particle
            Vector3 vector = particle.position;
            Quaternion quaternion = Quaternion.Euler(particle.rotation3D);
            if (this.ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
            {
                vector = base.rectTransform.InverseTransformPoint(vector);
            }
            Color32 currentColor = particle.GetCurrentColor(this.ParticleSystem);
            int currentVertCount = vh.currentVertCount;
            Vector3 currentSize3D = particle.GetCurrentSize3D(this.ParticleSystem);

            bool sheetAnimation = this.ParticleSystem.textureSheetAnimation.enabled;
            if (sheetAnimation)
            {
                float num = particle.startLifetime - particle.remainingLifetime;
                Vector2[] array = new Vector2[4];

                this.EvaluateTexturesheetUVs(particle, num, array);

                float xStart = array[0].x;
                float xRange = array[2].x - array[0].x;
                float yStart = array[0].y;
                float yRange = array[2].y - array[0].y;

                for (int i = 0; i < uv0s.Count; ++i)
                {
                    float x = xStart + uv0s[i].x * xRange;
                    float y = yStart + uv0s[i].y * yRange;

                    animateUV0s[i] = new Vector2(x, y);
                }
            }

            //Vertex Triangle
            if (colors.Length == 0)
            {
                for (int i = 0; i < particleMesh.vertexCount; i++)
                {
                    vh.AddVert(quaternion * (new Vector3(vertices[i].x * currentSize3D.x, vertices[i].y * currentSize3D.y, vertices[i].z * currentSize3D.z)) + vector, currentColor, sheetAnimation ? animateUV0s[i] : uv0s[i]);
                }
            }
            else
            {
                for (int i = 0; i < particleMesh.vertexCount; i++)
                {
                    vh.AddVert(quaternion * (new Vector3(vertices[i].x * currentSize3D.x, vertices[i].y * currentSize3D.y, vertices[i].z * currentSize3D.z)) + vector, currentColor * colors[i], sheetAnimation ? animateUV0s[i] : uv0s[i]);
                }
            }

            int triangleCount = particleTriangles.Length / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                vh.AddTriangle(particleTriangles[i * 3] + currentVertCount, particleTriangles[i * 3 + 1] + currentVertCount, particleTriangles[i * 3 + 2] + currentVertCount);
            }
        }

        private void DrawParticleBillboard(ParticleSystem.Particle particle, VertexHelper vh)
        {
            /*Vector3 vector = particle.get_position();
			Quaternion quaternion = Quaternion.Euler(particle.get_rotation3D());
			if (this.ParticleSystem.get_main().get_simulationSpace() == 1)
			{
				vector = base.get_rectTransform().InverseTransformPoint(vector);
			}
			float num = particle.get_startLifetime() - particle.get_remainingLifetime();
			float timeAlive = num / particle.get_startLifetime();
			Vector3 currentSize3D = particle.GetCurrentSize3D(this.ParticleSystem);
			if (this.m_RenderMode == UiParticleRenderMode.StreachedBillboard)
			{
				this.GetStrechedBillboardsSizeAndRotation(particle, timeAlive, ref currentSize3D, out quaternion);
			}
			Vector3 vector2 = new Vector3(-currentSize3D.x * 0.5f, currentSize3D.y * 0.5f);
			Vector3 vector3 = new Vector3(currentSize3D.x * 0.5f, currentSize3D.y * 0.5f);
			Vector3 vector4 = new Vector3(currentSize3D.x * 0.5f, -currentSize3D.y * 0.5f);
			Vector3 vector5 = new Vector3(-currentSize3D.x * 0.5f, -currentSize3D.y * 0.5f);
			vector2 = quaternion * vector2 + vector;
			vector3 = quaternion * vector3 + vector;
			vector4 = quaternion * vector4 + vector;
			vector5 = quaternion * vector5 + vector;
			Color32 currentColor = particle.GetCurrentColor(this.ParticleSystem);
			int currentVertCount = vh.get_currentVertCount();
			Vector2[] array = new Vector2[4];
			if (!this.ParticleSystem.get_textureSheetAnimation().get_enabled())
			{
				this.EvaluateQuadUVs(array);
			}
			else
			{
				this.EvaluateTexturesheetUVs(particle, num, array);
			}
			vh.AddVert(vector5, currentColor, array[0]);
			vh.AddVert(vector2, currentColor, array[1]);
			vh.AddVert(vector3, currentColor, array[2]);
			vh.AddVert(vector4, currentColor, array[3]);
			vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);*/

            Vector3 vector = particle.position;
            Quaternion quaternion = Quaternion.Euler(particle.rotation3D);
            if (this.ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
            {
                vector = base.rectTransform.InverseTransformPoint(vector);
            }
            float num = particle.startLifetime - particle.remainingLifetime;
            float timeAlive = num / particle.startLifetime;
            Vector3 currentSize3D = particle.GetCurrentSize3D(this.ParticleSystem);
            if (this.m_RenderMode == UiParticleRenderMode.StreachedBillboard)
            {
                this.GetStrechedBillboardsSizeAndRotation(particle, timeAlive, ref currentSize3D, out quaternion);
            }
            Vector3 vector2 = new Vector3(-currentSize3D.x * 0.5f, currentSize3D.y * 0.5f);
            Vector3 vector3 = new Vector3(currentSize3D.x * 0.5f, currentSize3D.y * 0.5f);
            Vector3 vector4 = new Vector3(currentSize3D.x * 0.5f, -currentSize3D.y * 0.5f);
            Vector3 vector5 = new Vector3(-currentSize3D.x * 0.5f, -currentSize3D.y * 0.5f);
            vector2 = quaternion * vector2 + vector;
            vector3 = quaternion * vector3 + vector;
            vector4 = quaternion * vector4 + vector;
            vector5 = quaternion * vector5 + vector;
            Color32 currentColor = particle.GetCurrentColor(this.ParticleSystem);
            int currentVertCount = vh.currentVertCount;
            Vector2[] array = new Vector2[4];
            if (!this.ParticleSystem.textureSheetAnimation.enabled)
            {
                this.EvaluateQuadUVs(array);
            }
            else
            {
                this.EvaluateTexturesheetUVs(particle, num, array);
            }
            vh.AddVert(vector5, currentColor, array[0]);
            vh.AddVert(vector2, currentColor, array[1]);
            vh.AddVert(vector3, currentColor, array[2]);
            vh.AddVert(vector4, currentColor, array[3]);
            vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }
        private void EvaluateQuadUVs(Vector2[] uvs)
        {
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(0f, 1f);
            uvs[2] = new Vector2(1f, 1f);
            uvs[3] = new Vector2(1f, 0f);
        }
        private void EvaluateTexturesheetUVs(ParticleSystem.Particle particle, float timeAlive, Vector2[] uvs)
        {
            /*ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = this.ParticleSystem.get_textureSheetAnimation();
			float num = particle.get_startLifetime() / (float)textureSheetAnimation.get_cycleCount();
			float num2 = timeAlive % num;
			float num3 = num2 / num;
			int num4 = textureSheetAnimation.get_numTilesY() * textureSheetAnimation.get_numTilesX();
			float num5 = textureSheetAnimation.get_frameOverTime().Evaluate(num3);
			float num6 = 0f;
			ParticleSystemAnimationType animation = textureSheetAnimation.get_animation();
			if (animation != null)
			{
				if (animation == 1)
				{
					num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)textureSheetAnimation.get_numTilesX()), 0f, (float)(textureSheetAnimation.get_numTilesX() - 1));
					int num7 = textureSheetAnimation.get_rowIndex();
					if (textureSheetAnimation.get_useRandomRow())
					{
						Random.InitState((int)particle.get_randomSeed());
						num7 = Random.Range(0, textureSheetAnimation.get_numTilesY());
					}
					num6 += (float)(num7 * textureSheetAnimation.get_numTilesX());
				}
			}
			else
			{
				num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)num4), 0f, (float)(num4 - 1));
			}
			int num8 = (int)num6 % textureSheetAnimation.get_numTilesX();
			int num9 = (int)num6 / textureSheetAnimation.get_numTilesX();
			float num10 = 1f / (float)textureSheetAnimation.get_numTilesX();
			float num11 = 1f / (float)textureSheetAnimation.get_numTilesY();
			num9 = textureSheetAnimation.get_numTilesY() - 1 - num9;
			float num12 = (float)num8 * num10;
			float num13 = (float)num9 * num11;
			float num14 = num12 + num10;
			float num15 = num13 + num11;
			uvs[0] = new Vector2(num12, num13);
			uvs[1] = new Vector2(num12, num15);
			uvs[2] = new Vector2(num14, num15);
			uvs[3] = new Vector2(num14, num13);*/

            ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = this.ParticleSystem.textureSheetAnimation;
            // 每个循环播放的时间
            float num = particle.startLifetime / (float)textureSheetAnimation.cycleCount;
            float num2 = timeAlive % num;
            // UV动画时间周期0~1
            float num3 = num2 / num;
            // UV动画帧数
            int num4 = textureSheetAnimation.numTilesY * textureSheetAnimation.numTilesX;
            // 当前UV动画所在帧位置（0~1）
            float num5 = textureSheetAnimation.frameOverTime.Evaluate(num3);
            float num6 = 0f;
            ParticleSystemAnimationType animation = textureSheetAnimation.animation;
            if (animation != null)
            {
                if (animation == ParticleSystemAnimationType.SingleRow)
                {
                    num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)textureSheetAnimation.numTilesX), 0f, (float)(textureSheetAnimation.numTilesX - 1));
                    int num7 = textureSheetAnimation.rowIndex;
                    if (textureSheetAnimation.useRandomRow)
                    {
                        UnityEngine.Random.InitState((int)particle.randomSeed);
                        num7 = UnityEngine.Random.Range(0, textureSheetAnimation.numTilesY);
                    }
                    num6 += (float)(num7 * textureSheetAnimation.numTilesX);
                }
                else
                {
                    num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)num4), 0f, (float)(num4 - 1));
                }
            }
            else
            {
                num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)num4), 0f, (float)(num4 - 1));
            }
            int num8 = (int)num6 % textureSheetAnimation.numTilesX;
            int num9 = (int)num6 / textureSheetAnimation.numTilesX;
            float num10 = 1f / (float)textureSheetAnimation.numTilesX;
            float num11 = 1f / (float)textureSheetAnimation.numTilesY;
            num9 = textureSheetAnimation.numTilesY - 1 - num9;
            float num12 = (float)num8 * num10;
            float num13 = (float)num9 * num11;
            float num14 = num12 + num10;
            float num15 = num13 + num11;
            uvs[0] = new Vector2(num12, num13);
            uvs[1] = new Vector2(num12, num15);
            uvs[2] = new Vector2(num14, num15);
            uvs[3] = new Vector2(num14, num13);
        }
        private void GetStrechedBillboardsSizeAndRotation(ParticleSystem.Particle particle, float timeAlive01, ref Vector3 size3D, out Quaternion rotation)
        {
            /*Vector3 zero = Vector3.get_zero();
			if (this.ParticleSystem.get_velocityOverLifetime().get_enabled())
			{
				zero.x = this.ParticleSystem.get_velocityOverLifetime().get_x().Evaluate(timeAlive01);
				zero.y = this.ParticleSystem.get_velocityOverLifetime().get_y().Evaluate(timeAlive01);
				zero.z = this.ParticleSystem.get_velocityOverLifetime().get_z().Evaluate(timeAlive01);
			}
			Vector3 vector = particle.get_velocity() + zero;
			float num = Vector3.Angle(vector, Vector3.get_up());
			int num2 = (vector.x >= 0f) ? -1 : 1;
			rotation = Quaternion.Euler(new Vector3(0f, 0f, num * (float)num2));
			size3D.y *= this.m_StretchedLenghScale;
			size3D += new Vector3(0f, this.m_StretchedSpeedScale * vector.get_magnitude());*/

            Vector3 zero = Vector3.zero;
            if (this.ParticleSystem.velocityOverLifetime.enabled)
            {
                zero.x = this.ParticleSystem.velocityOverLifetime.x.Evaluate(timeAlive01);
                zero.y = this.ParticleSystem.velocityOverLifetime.y.Evaluate(timeAlive01);
                zero.z = this.ParticleSystem.velocityOverLifetime.z.Evaluate(timeAlive01);
            }
            Vector3 vector = particle.velocity + zero;
            float num = Vector3.Angle(vector, Vector3.up);
            int num2 = (vector.x >= 0f) ? -1 : 1;
            rotation = Quaternion.Euler(new Vector3(0f, 0f, num * (float)num2));
            size3D.y *= this.m_StretchedLenghScale;
            size3D += new Vector3(0f, this.m_StretchedSpeedScale * vector.magnitude);
        }
    }
}
