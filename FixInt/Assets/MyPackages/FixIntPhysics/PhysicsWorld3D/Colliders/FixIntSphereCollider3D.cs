using ZM.FixIntMath;

namespace My.Physics3D
{
    public class FixIntSphereCollider3D : FixIntCollider3D
    {
        public FixInt Radius { get; protected set; }
        
        public FixIntSphereCollider3D(FixIntVector3 logicPos, FixIntVector3 center, FixInt radius) : base(logicPos, center)
        {
            Radius = radius;
            Collider3DEnum = Collider3DEnum.Sphere;
        }
        
        /// <summary>
        /// 检测碰撞
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool DetectCollision(FixIntCollider3D other)
        {
            switch (other.Collider3DEnum)
            {
                case Collider3DEnum.Box:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(other as FixIntBoxCollider3D, this);
                    break;
                case Collider3DEnum.Sphere:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, other as FixIntSphereCollider3D);
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