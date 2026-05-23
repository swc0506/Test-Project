using ZM.FixIntMath;

namespace My.Physics3D
{
    public class FixIntBoxCollider3D : FixIntCollider3D
    {
        /// <summary>
        /// Box大小
        /// </summary>
        public FixIntVector3 Size { get; protected set; }
        
        public FixIntVector3 HalfSize { get; private set; }

        /// <summary>
        /// Box宽度
        /// </summary>
        public FixInt BoxWidth => Size.x / 2;
        
        /// <summary>
        /// Box高度
        /// </summary>
        public FixInt BoxHeight => Size.y / 2;
        
        public FixIntBoxCollider3D(FixIntVector3 logicPos, FixIntVector3 center, FixIntVector3 size) : base(logicPos, center)
        {
            Size = size;
            HalfSize = size / 2;
            Collider3DEnum = Collider3DEnum.Box;
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
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, other as FixIntBoxCollider3D, UseAdjustPos);
                    break;
                case Collider3DEnum.Sphere:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, other as FixIntSphereCollider3D, UseAdjustPos);
                    break;
            }
            
            return base.DetectCollision(other);
        }

        public override void SyncLogicSize(FixIntVector3 size)
        {
            base.SyncLogicSize(size);
            Size = size;
            HalfSize = size / 2;
        }
    }
}
