using System.Collections;
using UnityEngine;

/// <summary>
/// 设置子节点下所有Renderer包括mesh，particle的sortingOrder
/// </summary>
public class Particle3DUIOrder : MonoBehaviour
{
    public int m_Order = 1;
    protected void Start()
    {
        StartCoroutine(_SetRendererOrder());
    }

    IEnumerator _SetRendererOrder()
    {
        var waitForEndOfFrame = new WaitForEndOfFrame();
        yield return waitForEndOfFrame;
        yield return waitForEndOfFrame;

        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer ps in renderers)
            {
                ps.sortingLayerID = canvas.sortingLayerID;
                ps.sortingOrder = canvas.sortingOrder + m_Order;
            }
        }
    }
}
