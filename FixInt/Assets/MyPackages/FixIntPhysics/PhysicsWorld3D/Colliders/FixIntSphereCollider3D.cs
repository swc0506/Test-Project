using My.FixIntPhysics3D;
using UnityEngine;
using ZM.FixIntMath;

namespace My.Physics3D
{
    public class FixIntSphereCollider3D : FixIntCollider3D
    {
        public FixInt Radius { get; protected set; }
        
#if  UNITY_EDITOR
        /// <summary>
        /// 可视化碰撞体边框绘制对象
        /// </summary>
        public SphereColliderBounds sphereColliderBounds;
#endif
        
        public FixIntSphereCollider3D(FixIntVector3 logicPos, FixIntVector3 center, FixInt radius) : base(logicPos, center)
        {
            Radius = radius;
            Collider3DEnum = Collider3DEnum.Sphere;
            
#if  UNITY_EDITOR
            GameObject obj=new GameObject(RenderObj==null?"FixIntSphereCollider3D":RenderObj.name);
            sphereColliderBounds = obj.AddComponent<SphereColliderBounds>();
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
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(other as FixIntBoxCollider3D, this);
                    break;
                case Collider3DEnum.Sphere:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, other as FixIntSphereCollider3D);
                    break;
                case Collider3DEnum.Cylinder:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(other as FixintCylinderCollider3D, this);
                    break;
            }
            
            return base.DetectCollision(other);
        }

        public override void SyncLogicRadius(FixInt radius)
        {
            base.SyncLogicRadius(radius);
            Radius = radius;
        }
        
        private void SyncBoundsRenderData()
        {
#if  UNITY_EDITOR
            sphereColliderBounds.SyncRenderData(LogicPos,Radius,Center);
#endif
        }
        public override void OnRelease()
        {
            base.OnRelease();
#if UNITY_EDITOR
            sphereColliderBounds.OnRelease();
            sphereColliderBounds=null;
#endif
        }
    }
}