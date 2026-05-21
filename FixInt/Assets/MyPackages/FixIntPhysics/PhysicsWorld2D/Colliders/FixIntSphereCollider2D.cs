using System.Collections.Generic;
using UnityEngine;
using ZM.FixIntMath;

namespace My.Physics2D
{
    public class FixIntSphereCollider2D : FixIntCollider2D
    {
        public FixInt Radius { get; protected set; }
        
        public FixIntSphereCollider2D(FixIntVector2 logicPos, FixIntVector2 center, FixInt radius) : base(logicPos, center)
        {
            Radius = radius;
            Collider2DEnum = Collider2DEnum.Sphere;
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
                    mIsCollision = PhysicsWorld2D.Instance.DetectCollision(other as FixIntBoxCollider2D, this);
                    break;
                case Collider2DEnum.Sphere:
                    mIsCollision = PhysicsWorld2D.Instance.DetectCollision(this, other as FixIntSphereCollider2D);
                    break;
            }
            
            return base.DetectCollision(other);
        }

        public override void SyncLogicRadius(FixInt radius)
        {
            base.SyncLogicRadius(radius);
            Radius = radius;
        }
    }
}