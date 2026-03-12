using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection_104 : PostEffectBase
{
    public Color edgeColor;
    public Color backColor;
    [Range(0, 1)] public float backExtent;

    protected override void UpdateProperties()
    {
        if (Material != null)
        {
            Material.SetColor("_EdgeColor", edgeColor);
            Material.SetColor("_BackColor", backColor);
            Material.SetFloat("_BackExtent", backExtent);
        }
    }
}
