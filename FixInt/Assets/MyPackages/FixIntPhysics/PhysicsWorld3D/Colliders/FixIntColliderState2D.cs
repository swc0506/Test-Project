using System;
using System.Collections.Generic;
using UnityEngine;

namespace My.Physics3D
{
    public partial class FixIntCollider3D
    {
        /// <summary>
        /// 碰撞进入回调
        /// </summary>
        public Action<FixIntCollider3D> OnCollisionEnter3DAction;
        /// <summary>
        /// 碰撞停留回调
        /// </summary>
        public Action<FixIntCollider3D> OnCollisionStay3DAction;
        /// <summary>
        /// 碰撞退出回调
        /// </summary>
        public Action<FixIntCollider3D> OnCollisionExit3DAction;
        /// <summary>
        /// 已经发生碰撞的碰撞体列表
        /// </summary>
        protected List<FixIntCollider3D> mAlreadyOccurCollisionList = new List<FixIntCollider3D>();
        
        /// <summary>
        /// 是否碰撞
        /// </summary>
        protected bool mIsCollision = false;
        
        /// <summary>
        /// 碰撞检测
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool DetectCollision(FixIntCollider3D target)
        {
            if (mIsCollision)
            {
                if (!mAlreadyOccurCollisionList.Contains(target))
                {
                    //首次碰撞
                    OnCollisionEnter3D(target);
                    mAlreadyOccurCollisionList.Add(target);
                }
                else
                {
                    //碰撞停留
                    OnCollisionStay3D(target);
                }
            }
            else
            {
                //碰撞离开
                if (mAlreadyOccurCollisionList.Contains(target))
                {
                    OnCollisionExit3D(target);
                }
            }
        
            return mIsCollision;
        }
        
        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="other">发生碰撞的碰撞体</param>
        public virtual void OnCollisionEnter3D(FixIntCollider3D other)
        {
            OnCollisionEnter3DAction?.Invoke(other);
        }
        
        /// <summary>
        /// 碰撞停留
        /// </summary>
        /// <param name="other">发生碰撞的碰撞体</param>
        public void OnCollisionStay3D(FixIntCollider3D other)
        {
            OnCollisionStay3DAction?.Invoke(other);
        }
        
        /// <summary>
        /// 碰撞退出
        /// </summary>
        /// <param name="other">发生碰撞的碰撞体</param>
        public void OnCollisionExit3D(FixIntCollider3D other)
        {
            //移除退出碰撞的碰撞体
            mAlreadyOccurCollisionList.Remove(other);
            //当已经发生碰撞的碰撞长度为0，则说明当前碰撞体没有发生碰撞，触发碰撞退出回调
            if (mAlreadyOccurCollisionList.Count == 0)
            {
                OnCollisionExit3DAction?.Invoke(other);
            }
        }
        
        /// <summary>
        /// 清理碰撞体列表
        /// </summary>
        protected void ClearCollisionData()
        {
            foreach (var item in mAlreadyOccurCollisionList)
            {
                item.OnCollisionExit3D(this);
            }
            mAlreadyOccurCollisionList.Clear();
        }
    }
}
