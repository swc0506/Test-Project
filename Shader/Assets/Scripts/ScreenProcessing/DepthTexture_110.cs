using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthTexture_110 : PostEffectBase
{
    void OnStart()
    {
        //开启深度纹理
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
}
