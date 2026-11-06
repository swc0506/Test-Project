using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeUIEffectAttach : MonoBehaviour
{
    [SerializeField]
    public List<RectTransform> m_Dummys = new List<RectTransform>();
    [SerializeField]
    public List<RectTransform> m_UIEffects = new List<RectTransform>();

    private List<int> m_TransChanged;

    //private Matrix4x4[] m_DummylocalToWorld;

    // Use this for initialization
    void Start()
    {
        Canvas canvas = null;
        Transform node = transform;
        while (canvas == null && node != null)
        {
            canvas = node.GetComponent<Canvas>();
            node = node.parent;
        }
        if(canvas != null)
        {
            RecursiveSetOrderInLayer(transform, canvas.sortingLayerID, canvas.sortingOrder);
        }

/*
        for (int i = 0; i < m_Dummys.Count; ++i)
        {
            if(m_Dummys[i] == null || m_UIEffects[i] == null)
            {
                m_Dummys[i] = m_Dummys[m_Dummys.Count - 1];
                m_UIEffects[i] = m_UIEffects[m_UIEffects.Count - 1];

                m_Dummys.RemoveAt(m_Dummys.Count - 1);
                m_UIEffects.RemoveAt(m_UIEffects.Count - 1);

                continue;
            }
        }
*/

        m_TransChanged = new List<int>(m_Dummys.Count);

        //m_DummylocalToWorld = new Matrix4x4[m_Dummys.Count];
        for (int i = 0; i < m_Dummys.Count; ++i)
        {
           // m_DummylocalToWorld[i] = m_Dummys[i].localToWorldMatrix;

            Matrix4x4 mat = m_UIEffects[i].parent.worldToLocalMatrix * m_Dummys[i].localToWorldMatrix;
            m_UIEffects[i].localPosition = mat.GetColumn(3);
            m_UIEffects[i].localRotation = Quaternion.LookRotation(
                                     mat.GetColumn(2),
                                     mat.GetColumn(1)
                                 );
            m_UIEffects[i].localScale = new Vector3(
                                     mat.GetColumn(0).magnitude,
                                     mat.GetColumn(1).magnitude,
                                     mat.GetColumn(2).magnitude
                                 );

            m_Dummys[i].hasChanged = false;
        }
    }

    void RecursiveSetOrderInLayer(Transform node, int sortinglayerID, int order)
    {
        if (node == null)
            return;

        ParticleSystem ps = node.GetComponent<ParticleSystem>();
        if(ps != null)
        {
            Renderer renderer = ps.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.sortingLayerID = sortinglayerID;
                renderer.sortingOrder = order;
            }
        }

        MeshRenderer meshRenderer = node.GetComponent<MeshRenderer>();
        if(meshRenderer != null)
        {
            meshRenderer.sortingLayerID = sortinglayerID;
            meshRenderer.sortingOrder = order;
        }

        for(int i = 0; i < node.childCount; ++i)
        {
            RecursiveSetOrderInLayer(node.GetChild(i), sortinglayerID, order);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_TransChanged.Clear();

        for (int i = 0; i < m_Dummys.Count; ++i)
        {
            if (m_Dummys[i] == null || m_UIEffects[i] == null)
            {
                continue;
            }

            RectTransform dummy = m_Dummys[i];
            RectTransform effect  = m_UIEffects[i];

            if(effect.gameObject.activeSelf != dummy.gameObject.activeInHierarchy)
            {
                effect.gameObject.SetActive(dummy.gameObject.activeInHierarchy);
            }

            if(effect.gameObject.activeInHierarchy && m_Dummys[i].hasChanged/*!m_DummylocalToWorld[i].Equals(dummy.localToWorldMatrix)*/)
            {
                m_TransChanged.Add(i);

                //m_DummylocalToWorld[i] = dummy.localToWorldMatrix;

                Matrix4x4 mat = effect.parent.worldToLocalMatrix * dummy.localToWorldMatrix;
                effect.localPosition = mat.GetColumn(3);
                effect.localRotation = Quaternion.LookRotation(
                                         mat.GetColumn(2),
                                         mat.GetColumn(1)
                                     );
                effect.localScale = new Vector3(
                                         mat.GetColumn(0).magnitude,
                                         mat.GetColumn(1).magnitude,
                                         mat.GetColumn(2).magnitude
                                     );
            }
        }
    }

    private void LateUpdate()
    {
        int changedSize = m_TransChanged.Count;
        for (int i = 0; i < changedSize; ++i)
        {
            m_Dummys[m_TransChanged[i]].hasChanged = false;
        }
    }

    public void DelItem(int iIndex)
    {
        m_Dummys.RemoveAt(iIndex);
        m_UIEffects.RemoveAt(iIndex);
        m_TransChanged.Clear();
    }

    public void AddNewItem()
    {
        m_Dummys.Add(null);
        m_UIEffects.Add(null);
    }
}
