using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlurWithDepth_112 : PostEffectBase
{
    [Range(0, 1)]
    public float blurSize = 0.5f;
    //用于记录上一次的变换矩阵变量
    private Matrix4x4 frontWorldToClipMatrix;

    private void Start()
    {
        // 开启深度纹理
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
    
    private void OnEnable()
    {
        // 初始化前一帧的变换矩阵 用 观察到的裁剪变换矩阵 * 世界到观察变换矩阵 = 世界到裁剪变换矩阵
        frontWorldToClipMatrix = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
    }
    
    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Material != null)
        {
            //设置模糊程度
            Material.SetFloat("_BlurSize", blurSize);
            //设置上一帧世界空间到裁剪空间的矩阵
            Material.SetMatrix("_FrontWorldToClipMatrix", frontWorldToClipMatrix);
            //计算这一帧的变换矩阵
            frontWorldToClipMatrix = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
            //设置这一帧 裁剪到世界空间的变换矩阵
            Material.SetMatrix("_ClipToWorldMatrix", frontWorldToClipMatrix.inverse);
            //执行后处理
            Graphics.Blit(source, destination, Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
