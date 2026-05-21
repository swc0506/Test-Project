using System;
using UnityEngine;
using ZM.FixIntMath;

namespace My.Physics2D
{
    public class FixIntBoxCollider2D : FixIntCollider2D
    {
        /// <summary>
        /// Box大小
        /// </summary>
        public FixIntVector2 Size { get; protected set; }

        /// <summary>
        /// Box宽度
        /// </summary>
        public FixInt BoxWidth => Size.x / 2;
        
        /// <summary>
        /// Box高度
        /// </summary>
        public FixInt BoxHeight => Size.y / 2;
        
        public FixIntBoxCollider2D(FixIntVector2 logicPos, FixIntVector2 center, FixIntVector2 size) : base(logicPos, center)
        {
            Size = size;
            Collider2DEnum = Collider2DEnum.Box;
        }

        /// <summary>
        /// 检测碰撞
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool DetectCollision(FixIntCollider2D other)
        {
            switch (other.Collider2DEnum)
            {
                case Collider2DEnum.Box:
                    mIsCollision = PhysicsWorld2D.Instance.DetectCollision(this, other as FixIntBoxCollider2D, UseAdjustPos);
                    break;
                case Collider2DEnum.Sphere:
                    mIsCollision = PhysicsWorld2D.Instance.DetectCollision(this, other as FixIntSphereCollider2D, UseAdjustPos);
                    break;
            }
            
            return base.DetectCollision(other);
        }

        public override void SyncLogicSize(FixIntVector2 size)
        {
            base.SyncLogicSize(size);
            Size = size;
        }
    }
}
