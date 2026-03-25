using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeWithDepthNormalsTexture_114 : PostEffectBase
{
    [Range(0, 1)] 
    public float edgeOnly = 0;

    public Color edgeColor = Color.black;
    public Color backgroundColor = Color.white;
    public float sampleDistance = 1;
    public float sensitivityDepth = 1;
    public float sensitivityNormal = 1;
    
    void Start()
    {
        // |=避免关闭深度纹理
        Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    protected override void UpdateProperties()
    {
        if (Material != null)
        {
            Material.SetFloat("_EdgeOnly", edgeOnly);
            Material.SetColor("_EdgeColor", edgeColor);
            Material.SetColor("_EdgeGroundColor", backgroundColor);
            Material.SetFloat("_SampleDistance", sampleDistance);
            Material.SetFloat("_SensitivityDepth", sensitivityDepth);
            Material.SetFloat("_SensitivityNormal", sensitivityNormal);
        }
    }
}
