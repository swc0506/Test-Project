using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 提供属性与接口，不负责实现
/// </summary>
public class LogicBehaviour
{
    public RenderObject RednerObj { get; protected set; }//渲染对象
    
    public VInt3 LogicPosition { get; set; }//逻辑位置
    
    public virtual void OnCreate()
    {
        
    }
    
    public virtual void OnLogicFrameUpdate()
    {
        
    }
    
    public virtual void OnDestroy()
    {
        
    }
}
