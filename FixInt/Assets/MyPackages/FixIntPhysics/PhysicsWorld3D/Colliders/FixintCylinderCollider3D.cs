using My.FixIntPhysics3D;
using UnityEngine;
using ZM.FixIntMath;

namespace My.Physics3D
{
    public class FixintCylinderCollider3D : FixIntCollider3D
    {
        /// <summary>
        /// 半径
        /// </summary>
        public FixInt Radius { get; private set; }

        /// <summary>
        /// 高度
        /// </summary>
        public FixInt Height { get; private set; }

        /// <summary>
        /// 最高点
        /// </summary>
        public FixInt MaxHeightPoint { get; private set; }

        /// <summary>
        /// 最低点
        /// </summary>
        public FixInt MinHeightPoint { get; private set; }

        /// <summary>
        /// 大小(对应3D空间下的缩放)
        /// </summary>
        private FixIntVector3 Scale;
#if  UNITY_EDITOR
        /// <summary>
        /// 可视化碰撞体边框绘制对象
        /// </summary>
        public CylinderColliderBounds cylinderColliderBounds;
#endif
        public FixintCylinderCollider3D(FixIntVector3 logicPos, FixInt radius, FixIntVector3 center, FixInt height,
            FixIntVector3 scale) : base(logicPos, center)
        {
            this.Radius = radius;
            this.Height = height;
            this.Scale = scale;
          
            Collider3DEnum=Collider3DEnum.Cylinder;
#if  UNITY_EDITOR
            GameObject obj=new GameObject(RenderObj==null?"FixintCylinderCollider3D":RenderObj.name);
            cylinderColliderBounds = obj.AddComponent<CylinderColliderBounds>();
#endif
            SyncHeightPoint();
        }

        public override bool DetectCollision(FixIntCollider3D target)
        {
            switch (target.Collider3DEnum)
            {
                case Collider3DEnum.Box:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, target as FixIntBoxCollider3D,UseAdjustPos);
                    break;
                case Collider3DEnum.Cylinder:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, target as FixintCylinderCollider3D);
                    break;
                case Collider3DEnum.Sphere:
                    mIsCollision = PhysicsWorld3D.Instance.DetectCollision(this, target as FixIntSphereCollider3D);
                    break;
            }
        
            return base.DetectCollision(target);
        }
 
        /// <summary>
        /// 同步位置
        /// </summary>
        /// <param name="logicPos"></param>
        public override void SyncLogicPos(FixIntVector3 logicPos)
        {
            base.SyncLogicPos(logicPos);
            SyncHeightPoint();
        }

        /// <summary>
        /// 同步半径
        /// </summary>
        /// <param name="logicRadius"></param>
        public override void SyncLogicRadius(FixInt logicRadius)
        {
            base.SyncLogicRadius(logicRadius);
            Radius = logicRadius;
            SyncBoundsRenderData();
        }

        /// <summary>
        /// 同步缩放
        /// </summary>
        /// <param name="logicSize"></param>
        public override void SyncLogicSize(FixIntVector3 logicSize)
        {
            base.SyncLogicSize(logicSize);
            Scale = logicSize;
            SyncHeightPoint();
        }

        /// <summary>
        /// 同步高度
        /// </summary>
        /// <param name="height"></param>
        public override void SyncLogicHeight(FixInt height)
        {
            base.SyncLogicHeight(height);
            SyncHeightPoint();
        }

        /// <summary>
        /// 同步最高位置和最低位置
        /// </summary>
        protected void SyncHeightPoint()
        {
            FixInt halfHeight = (this.Height * Scale.y) / 2;
            MaxHeightPoint = LogicPos.y + halfHeight;
            MinHeightPoint = LogicPos.y - halfHeight;
            SyncBoundsRenderData();
        }

        /// <summary>
        /// 目标点是否在圆柱体的高度内
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool ContainTargetHeight(FixInt y)
        {
            return y <= MaxHeightPoint && y >= MinHeightPoint;
        }
        /// <summary>
        /// 同步碰撞体渲染数据
        /// </summary>
        private void SyncBoundsRenderData()
        {
#if  UNITY_EDITOR
            cylinderColliderBounds.SyncRenderData(LogicPos,Radius, Height,Center);
#endif
        }
        public override void OnRelease()
        {
            base.OnRelease();
#if UNITY_EDITOR
            cylinderColliderBounds.OnRelease();
            cylinderColliderBounds=null;
#endif
        }
    }
}

