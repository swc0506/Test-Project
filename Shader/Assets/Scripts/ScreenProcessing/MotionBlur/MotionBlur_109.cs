using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur_109 : PostEffectBase
{
    [Range(0, 0.9f)]
    public float blurAmount = 0.5f;
    
    //堆积纹理 用于存储之前渲染的结果
    private RenderTexture lastFrame;
    
    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Material != null)
        {
            if (lastFrame == null || lastFrame.width != source.width || lastFrame.height != source.height)
            {
                DestroyImmediate(lastFrame);
                //初始化
                lastFrame = new RenderTexture(source.width, source.height, 0);
                lastFrame.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, lastFrame);//初始化为当前帧的图像 作为颜色缓存区中的颜色
            }
            
            Material.SetFloat("_BlurAmount", 1.0f - blurAmount);
            
            Graphics.Blit(source, lastFrame, Material);
            Graphics.Blit(lastFrame, destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
    
    private void OnDestroy()
    {
        DestroyImmediate(lastFrame);
    }
}
