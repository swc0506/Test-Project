using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class UiMesh : MaskableGraphic
{
    [FormerlySerializedAs("m_MaterialAnimationNames"), SerializeField]
    private string[] m_MaterialAnimationNames;


    //private Mesh m_Mesh;
    private List<Vector3> m_Vertex;
    private List<Vector2> m_UV;
    private List<Color> m_Colors;
    private int[] m_Triangles;
    private MeshRenderer m_MeshRenderer;
    private MaterialPropertyBlock m_MatBlock;

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
		
	protected override void Start()
	{
        if (!Application.isPlaying)
            return;

        m_MeshRenderer = GetComponent<MeshRenderer>();

        SortingGroup sortgroup = GetComponent<SortingGroup>();
        if (sortgroup != null)
            DestroyImmediate(sortgroup);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            m_Vertex = new List<Vector3>(mesh.vertexCount);
            m_UV = new List<Vector2>(mesh.vertexCount);
            m_Colors = new List<Color>(mesh.vertexCount);

            mesh.GetVertices(m_Vertex);
            mesh.GetUVs(0, m_UV);
            mesh.GetColors(m_Colors);

            m_Triangles = mesh.GetTriangles(0);
        }

        if (m_MeshRenderer != null)
        {
            if (m_MaterialAnimationNames.Length > 0)
            {
                // this.m_Material = new Material(m_MeshRenderer.sharedMaterial);
                //this.m_Material.name = m_MeshRenderer.sharedMaterial + "_Clone";
                this.m_Material = m_MeshRenderer.material;
                m_MatBlock = new MaterialPropertyBlock();
            }
            else
            {
                this.m_Material = m_MeshRenderer.sharedMaterial;
            }
        }

        if (this.m_MeshRenderer != null && this.m_MeshRenderer.enabled)
        {
            this.m_MeshRenderer.enabled = false;
        }

        this.raycastTarget = false;
		SetAllDirty ();
	}

    protected override void OnDestroy()
    {
        if (m_MeshRenderer != null && m_MaterialAnimationNames.Length > 0)
        {
            DestroyImmediate(m_Material);
            m_Material = null;
        }

/*
        if (this.m_MeshRenderer != null && !this.m_MeshRenderer.enabled)
        {
            this.m_MeshRenderer.enabled = true;
        }*/
    }


    /*
        public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();

            if (this.m_MeshRenderer != null)
            {
                this.m_MeshRenderer.sharedMaterial = this.m_Material;
            }
        }*/

    protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (this.m_Vertex == null)
		{
            toFill.Clear();
            return;
		}

		this.GenerateMeshUI(toFill);
	}
	protected virtual void Update()
	{
        /*// 只有Transform有改变，才需要重新填充Vertex
		if (transform.hasChanged)
		{
			this.SetVerticesDirty();

            transform.hasChanged = false;

        }*/

        if (m_MatBlock != null && m_MeshRenderer != null)
        {
            m_MeshRenderer.GetPropertyBlock(m_MatBlock);
            for (int i = 0; i < m_MaterialAnimationNames.Length; ++i)
            {
                Vector4 dest = m_MatBlock.GetVector(m_MaterialAnimationNames[i]);
                m_Material.SetVector(m_MaterialAnimationNames[i], dest);
            }
        }
    }

	private void GenerateMeshUI(VertexHelper vh)
	{
        vh.Clear();

        Color meshColor = color;
        if (m_Colors.Count >= m_Vertex.Count)
        {
            for (int i = 0; i < m_Vertex.Count; ++i)
            {
                vh.AddVert(m_Vertex[i], meshColor * m_Colors[i], m_UV[i]);
            }
        }

        else
        {
            for (int i = 0; i < m_Vertex.Count; ++i)
            {
                vh.AddVert(m_Vertex[i], meshColor, m_UV[i]);
            }
        }

        int triangelCount = m_Triangles.Length / 3;
        for (int i = 0; i < triangelCount; ++i)
        {
            vh.AddTriangle(m_Triangles[i * 3], m_Triangles[i * 3 + 1], m_Triangles[i * 3 + 2]);
        }
    } 

/*
    [ContextMenu("GetMaterialInstace")]
    public void GetMaterialInstance()
    {
        this.m_Material = m_MeshRenderer.material;
    }*/
		
}
