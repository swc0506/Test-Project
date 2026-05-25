using My.FixIntPhysics3D;
using UnityEngine;
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
        
#if  UNITY_EDITOR
        /// <summary>
        /// 可视化碰撞体边框绘制对象
        /// </summary>
        public BoxColliderBounds boxColliderBounds;
#endif
        
        public FixIntBoxCollider3D(FixIntVector3 logicPos, FixIntVector3 center, FixIntVector3 size) : base(logicPos, center)
        {
            Size = size;
            HalfSize = size / 2;
            Collider3DEnum = Collider3DEnum.Box;
#if  UNITY_EDITOR
            GameObject obj=new GameObject(RenderObj==null?"FixIntBoxCollider3D":RenderObj.name);
            boxColliderBounds = obj.AddComponent<BoxColliderBounds>();
            SyncBoundsRenderData();
#endif
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
                case Collider3DEnum.Cylinder:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(other as FixintCylinderCollider3D,this, UseAdjustPos);
                    break;
            }
            
            return base.DetectCollision(other);
        }

        public override void SyncLogicSize(FixIntVector3 size)
        {
            base.SyncLogicSize(size);
            Size = size;
            HalfSize = size / 2;
            SyncBoundsRenderData();
        }
        
        /// <summary>
        /// 同步碰撞体渲染数据
        /// </summary>
        private void SyncBoundsRenderData()
        {
#if  UNITY_EDITOR
            boxColliderBounds.SyncRenderData(LogicPos,Center,Size);
#endif
        }

        public override void OnRelease()
        {
            base.OnRelease();
#if UNITY_EDITOR
            boxColliderBounds.OnRelease();
            boxColliderBounds=null;
#endif
        }
    }
}
