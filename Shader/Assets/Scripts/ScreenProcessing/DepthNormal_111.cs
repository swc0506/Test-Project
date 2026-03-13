using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthNormal_111 : PostEffectBase
{
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
