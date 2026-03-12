using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom_108 : PostEffectBase
{
    [Range(1, 8)]
    public int downsample = 1;
    
    [Range(1, 8)]
    public int iterations = 1;
    
    [Range(0, 3)]
    public float blurSpread = 1f;  
    
    // 亮度阈值
    [Range(0, 4)]
    public float luminance = 0.5f;
    
    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Material != null)
        {
            int rtW = source.width / downsample;
            int rtH = source.height / downsample;
            
            Material.SetFloat("_Luminance", luminance);
            
            //准备一个缓存区
            RenderTexture temp = RenderTexture.GetTemporary(rtW, rtH, 0);
            temp.filterMode = FilterMode.Bilinear;//采用双线性过滤
            
            // 渲染到缓存区
            Graphics.Blit(source, temp, Material, 0);
            
            //渲染到目标 处理图像多次
            for (int i = 0; i < iterations; i++)
            {
                Material.SetFloat("_BlurSpread", 1 + i * blurSpread);
                
                RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
                //渲染到缓存区 处理图像两次
                Graphics.Blit(temp, buffer, Material, 1);//0 执行第一个Pass
                RenderTexture.ReleaseTemporary(temp);
                temp = buffer;
                
                buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
                //渲染到目标
                Graphics.Blit(temp, buffer, Material, 2);
                RenderTexture.ReleaseTemporary(temp);
                temp = buffer;
            }
            
            //提取出的内容进行高斯模糊后 存储到Shader中 用于之后的合成
            Material.SetTexture("_Bloom", temp);
            
            //渲染到目标
            Graphics.Blit(temp, destination);
            //释放缓存区
            RenderTexture.ReleaseTemporary(temp);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
