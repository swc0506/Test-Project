using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson_100 : MonoBehaviour
{
    public Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //加入特性，使渲染器只渲染不透明的物体
    [ImageEffectOpaque]
    //source:渲染器渲染的原始图像，destination:渲染器渲染的目标图像
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);//将source复制到destination
        Graphics.Blit(source, destination, material);//将source复制到destination，并使用material的shader进行处理
    }
}
