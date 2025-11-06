using System;
using UnityEditor;
using UnityEngine;

namespace ZMUIFrameWork.Scripts.Runtime.Base
{
    public class WindowBehaviour
    {
        public GameObject GameObject { get; set; }
        public Transform Transform { get; set; }
        public Canvas Canvas { get; set; }
        public bool PopStack { get; set; }//是否是从堆栈系统弹出的弹窗
        public Action<WindowBase> PopStackListener { get; set; }
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
}