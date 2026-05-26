using UnityEngine;
using ZM.FixIntMath;

namespace My.Physics2D
{
    public partial class FixIntCollider2D
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active {get; private set;}
        
        /// <summary>
        /// 逻辑坐标
        /// </summary>
        public FixIntVector2 LogicPos { get; protected set; }
        
        /// <summary>
        /// 中心坐标
        /// </summary>
        public FixIntVector2 Center { get; protected set; }
        
        /// <summary>
        /// 渲染对象
        /// </summary>
        public GameObject RenderObj { get; private set; }
        
        public Collider2DEnum Collider2DEnum { get; protected set; }

        public FixInt X => LogicPos.x;
        public FixInt Y => LogicPos.y;
        
        /// <summary>
        /// 是否使用调整位置
        /// </summary>
        protected bool UseAdjustPos { get; private set; }
        
        /// <summary>
        /// 是否可以修正位置
        /// </summary>
        public bool CanAdjust { get; private set; }
        
        /// <summary>
        /// 需要修正位置
        /// </summary>
        private FixIntVector2 mAdjustPos;

        public FixIntVector2 AdjustPos
        {
            get
            {
                CanAdjust = false;
                return mAdjustPos;
            }
            set
            {
                CanAdjust = true;
                mAdjustPos = value;
            }
        }
        
        public FixIntCollider2D(FixIntVector2 logicPos, FixIntVector2 center)
        {
            this.LogicPos = logicPos + center;
            this.Center = center;
            Active = true;
        }
        
        public void SetRenderObj(GameObject renderObj)
        {
            RenderObj = renderObj;
        }

        public void SyncLogicPos(FixIntVector2 logicPos)
        {
            this.LogicPos = logicPos + Center;
        }

        /// <summary>
        /// 同步大小
        /// </summary>
        /// <param name="size"></param>
        public virtual void SyncLogicSize(FixIntVector2 size)
        {
            
        }

        /// <summary>
        /// 同步半径
        /// </summary>
        /// <param name="radius"></param>
        public virtual void SyncLogicRadius(FixInt radius)
        {
            
        }
        
        /// <summary>
        /// 释放碰撞体
        /// </summary>
        public virtual void OnRelease()
        {
            this.Active = false;
            ClearCollisionData();
            PhysicsWorld2D.Instance.RemoveCollider2D(this);
        } 
        
        public virtual void SetUseAdjustPos(bool useAdjustPos)
        {
            UseAdjustPos = useAdjustPos;
        }
    }
}

