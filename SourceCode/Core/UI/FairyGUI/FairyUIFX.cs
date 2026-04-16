#if FAIRYGUI
using System;
using Core.FS;
using FairyGUI;
using UnityEngine;

namespace Core.UI
{
    public class FairyUIFX : UIFX
    {
        private GoWrapper goWrp;
        private GGraph holder;

        private Vector3 rawOffset;

        private GButton touchBtn;
        private float prevX;

        protected override void OnLoadSuccess()
        {
            goWrp = new GoWrapper();
            goWrp.SetWrapTarget(Skin, stencil);
            goWrp.supportStencil = true;
            goWrp.touchable = false;
            holder = new GGraph();
            holder.touchable = false;
            holder.SetNativeObject(goWrp);
        }

        protected override void RefreshActive()
        {
            base.RefreshActive();
            if (null != holder)
            {
                holder.visible = IsPlaying;
            }

            if (null != Skin)
            {
                Skin.SetActive(IsPlaying);
            }
        }

        protected override void RefreshAnimator()
        {
            if (!string.IsNullOrEmpty(stateName) && null != Skin)
            {
                if (null == animator)
                {
                    animator = Skin.GetComponentInChildren<Animator>(true);
                }

                if (null != animator)
                {
                    animator.Play(stateName);
                }
            }
        }

        protected override void RefreshParent()
        {
            if (null == holder || holder.isDisposed)
            {
                return;
            }

            if (null == parent)
            {
                holder.RemoveFromParent();
            }
            else
            {
                if (parent is GGraph graph)
                {
                    int index = graph.parent.GetChildIndex(graph);
                    graph.parent.AddChildAt(holder, index);
                    holder.relations.CopyFrom(graph.relations);
                    holder.pivotAsAnchor = graph.pivotAsAnchor;
                    holder.pivot = graph.pivot;
                    holder.scale = graph.scale;
                    holder.position = graph.position;

                    rawOffset = graph.position;
                }
                else if (parent is GComponent comp)
                {
                    comp.AddChild(holder);
                }
            }
        }

        protected override void RefreshScale()
        {
            if (null != goWrp)
            {
                goWrp.scale = new Vector2(100, 100);
            }

            if (null != Skin)
            {
                Skin.transform.localScale = scale;
            }
        }

        protected override void RefreshPosition()
        {
            if (null != goWrp)
            {
                goWrp.position = pos + rawOffset;
            }
        }

        protected override void Refresh3DPosition()
        {
            if (null != Skin)
            {
                var pos = pos3d;
                pos.z += UIFXManager.Instance.offsetDistance;
                Skin.transform.localPosition = pos;
            }
        }

        protected override void RefreshAngle()
        {
            if (null != Skin)
            {
                Skin.transform.localEulerAngles = angle;
            }
        }

        protected override void RefreshTouchArea()
        {
            if (null != touchBtn && touchBtn.isDisposed)
            {
                return;
            }

            if (null == touchArea)
            {
                if (null != touchBtn)
                {
                    touchBtn.RemoveFromParent();
                    touchBtn.onClick.Clear();
                }
            }
            else
            {
                if (null == touchBtn)
                {
                    string pkgName = UIFXManager.Instance.touchPkgName;
                    string compName = UIFXManager.Instance.touchCompName;
                    touchBtn = (GButton)FairyGUIManager.Instance.CreateObject(pkgName, compName);
                    if (null != touchBtn)
                    {
                        touchBtn.onTouchBegin.Add(OnTouchBegin);
                        touchBtn.onTouchMove.Add(OnTouchMove);
                    }
                }

                if (null == touchBtn)
                {
                    return;
                }

                if (touchArea is GGraph graph)
                {
                    int index = graph.parent.GetChildIndex(graph);
                    graph.parent.AddChildAt(touchBtn, index);
                    touchBtn.relations.CopyFrom(graph.relations);
                    touchBtn.pivotAsAnchor = graph.pivotAsAnchor;
                    touchBtn.pivot = graph.pivot;
                    touchBtn.scale = graph.scale;
                    touchBtn.position = graph.position;
                    touchBtn.size = graph.size;
                }
            }
        }

        private void OnTouchBegin(EventContext context)
        {
            if (disableDrag)
            {
                return;
            }

            InputEvent input = (InputEvent)context.data;
            prevX = input.x;
        }

        private void OnTouchMove(EventContext context)
        {
            if (disableDrag)
            {
                return;
            }

            InputEvent input = (InputEvent)context.data;
            float x = input.x;
            float delta = x - prevX;
            if (null != Skin)
            {
                angle.y -= delta;
                RefreshAngle();
            }

            prevX = x;
        }


        public override void AddClick<T>(T callback)
        {
            base.AddClick(callback);
            if (null != touchArea && null != touchBtn)
            {
                if (callback is EventCallback0 callback0)
                {
                    touchBtn.onClick.Add(callback0);
                }
                else if (callback is EventCallback1 callback1)
                {
                    touchBtn.onClick.Add(callback1);
                }
            }
        }

        public override void RemoveClick<T>(T callback)
        {
            base.RemoveClick(callback);
            if (null != touchBtn)
            {
                if (callback is EventCallback0 callback0)
                {
                    touchBtn.onClick.Remove(callback0);
                }
                else if (callback is EventCallback1 callback1)
                {
                    touchBtn.onClick.Remove(callback1);
                }
            }
        }

        internal override void Clear()
        {
            base.Clear();
            SetTouchArea(null);
        }

        internal override void Dispose()
        {
            base.Dispose();
            if (null != goWrp)
            {
                goWrp.Dispose();
                goWrp = null;
            }

            if (null != holder)
            {
                holder.Dispose();
                holder = null;
            }

            if (null != touchBtn)
            {
                touchBtn.Dispose();
                touchBtn = null;
            }
        }
    }
}
#endif