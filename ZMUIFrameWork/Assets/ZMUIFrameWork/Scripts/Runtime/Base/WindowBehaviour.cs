using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehaviour
{
    public GameObject GameObject { get; set; }
    public Transform Transform { get; set; }
    public Canvas Canvas { get; set; }
    public string Name { get; set; }
    public bool Visible => visible;

    protected bool visible;
    
    public virtual void OnAwake(){}
    public virtual void OnShow(){}
    public virtual void OnUpdate(){}
    public virtual void OnHide(){}
    public virtual void OnDestroy(){}
    public virtual void SetVisible(bool isVisible) { }
}
