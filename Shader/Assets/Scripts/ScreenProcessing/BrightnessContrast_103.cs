using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightnessContrast_103 : PostEffectBase
{
    [Range(0,5)]
    public float brightness = 1.0f;
    [Range(0,5)]
    public float contrast = 1.0f;
    [Range(0,5)]
    public float saturation = 1.0f;
    
    /// <summary>
    /// 更新材质球的属性
    /// </summary>
    protected override void UpdateProperties()
    {
        if (Material != null)
        {
            Material.SetFloat("_Brightness", brightness);
            Material.SetFloat("_Contrast", contrast);
            Material.SetFloat("_Saturation", saturation);
        }
    }
}
