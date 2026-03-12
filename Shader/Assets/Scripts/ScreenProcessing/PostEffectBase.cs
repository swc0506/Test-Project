using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectBase : MonoBehaviour
{
    //屏幕后处理会使用的shader
    public Shader shader;
    //用于动态创建的材质球
    private Material material;
    
    protected Material Material
    {
        get
        {
            if (shader == null || !shader.isSupported)//shader不存在或不支持
                return null;
            else
            {
                if (material == null)
                    material = new Material(shader);
                else if (material.shader != shader)
                    material.shader = shader;
                material.hideFlags = HideFlags.DontSave;//不保存材质球
                return material;
            }
        }
    }
    
    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateProperties();
        
        if (Material == null)
            Graphics.Blit(source, destination);
        else
            Graphics.Blit(source, destination, Material);//将source复制到destination，并使用material的shader进行处理
    }
    
    //更新材质球的属性
    protected virtual void UpdateProperties()
    {
    }
}
