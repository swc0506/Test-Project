using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeUIEffectOrder : MonoBehaviour
{
    public int m_SortingOrderOffset = 0;

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
            RecursiveSetOrderInLayer(transform, canvas.sortingLayerID, canvas.sortingOrder + m_SortingOrderOffset);
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

		Renderer meshRenderer = node.GetComponent<Renderer>();
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
}
