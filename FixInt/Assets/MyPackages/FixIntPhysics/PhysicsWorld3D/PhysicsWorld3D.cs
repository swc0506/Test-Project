using System.Collections.Generic;
using UnityEngine;
using ZM.FixIntMath;

namespace My.Physics3D
{
    public class PhysicsWorld3D
    {
        private static PhysicsWorld3D instance;

        public static PhysicsWorld3D Instance
        {
            get
            {
                instance ??= new PhysicsWorld3D();
                return instance;
            }
        }

        /// <summary>
        /// 所有2D碰撞器
        /// </summary>
        private readonly List<FixIntCollider3D> collider3DList = new List<FixIntCollider3D>();

        /// <summary>
        /// 逻辑帧更新接口
        /// </summary>
        public void OnLogicFrameUpdate()
        {
            for (int i = 0; i < collider3DList.Count; i++)
            {
                var item = collider3DList[i];
                for (int j = 0; j < collider3DList.Count; j++)
                {
                    var target = collider3DList[j];
                    if (item == target)
                        continue;

                    if (item.DetectCollision(target))
                    {
                        Debug.Log("碰撞");
                    }
                }
            }
        }

        /// <summary>
        /// 检测Box碰撞
        /// </summary>
        /// <param name="boxA"></param>
        /// <param name="boxB"></param>
        /// <param name="isUseAdjustPos"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntBoxCollider3D boxA, FixIntBoxCollider3D boxB, bool isUseAdjustPos = false)
        {
            if (!boxA.Active || !boxB.Active)
                return false;

            FixIntVector3 minA = boxA.LogicPos - boxA.HalfSize;
            FixIntVector3 maxA = boxA.LogicPos + boxA.HalfSize;
            FixIntVector3 minB = boxB.LogicPos - boxB.HalfSize;
            FixIntVector3 maxB = boxB.LogicPos + boxB.HalfSize;
            //判断A是否在B的左边或右边，只有在中间，两物才会在X轴上相交
            if (maxA.x < minB.x || minA.x > maxB.x) return false;
            //判断A是否在B的上边或者下边，只有在中间，两物体才会在Y轴上相交
            if (maxA.y < minB.y || minA.y > maxB.y) return false;
            //判断A是否在B的前面或后面，只有在中间，两物体才会在Z轴上相交
            if (maxA.z < minB.z || minA.z > maxB.z) return false;

            return true;
        }

        /// <summary>
        /// 检测Box与Sphere碰撞
        /// </summary>
        /// <param name="box"></param>
        /// <param name="sphere"></param>
        /// <param name="isUseAdjustPos"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntBoxCollider3D box, FixIntSphereCollider3D sphere, bool isUseAdjustPos = false)
        {
            if (!box.Active || !sphere.Active) return false;

            //获取BoxCollider的最小点和最大点
            FixIntVector3 minA = box.LogicPos - box.HalfSize;
            FixIntVector3 maxA = box.LogicPos + box.HalfSize;
            //获取圆球的中心点
            FixIntVector3 sphereCenter = sphere.LogicPos;

            //计算Box离Sphere最近点
            FixIntVector3 closedPoint = new FixIntVector3(
                FixIntMath.Clamp(sphereCenter.x, minA.x, maxA.x),
                FixIntMath.Clamp(sphereCenter.y, minA.y, maxA.y),
                FixIntMath.Clamp(sphereCenter.z, minA.z, maxA.z)
            );
            //检查距离圆球最近的点到圆球重点长度是否小于圆球的半径，如果小于则说明发生了碰撞
            return (closedPoint - sphereCenter).sqrMagnitude <= FixIntMath.Pow(sphere.Radius, 2);
        }

        /// <summary>
        /// 检测Sphere碰撞
        /// </summary>
        /// <param name="sA"></param>
        /// <param name="sB"></param>
        /// <param name="isUseAdjustPos"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntSphereCollider3D sA, FixIntSphereCollider3D sB, bool isUseAdjustPos = false)
        {
            if (!sA.Active || !sB.Active)
                return false;

            return (sA.LogicPos - sB.LogicPos).sqrMagnitude < FixIntMath.Pow(sA.Radius + sB.Radius, 2);
        }

        public bool DetectCollision(FixintCylinderCollider3D ccA, FixintCylinderCollider3D ccB,
            bool isUseAdjustPos = false)
        {
            //有任意一个Collider没有开启碰撞，则碰撞无效
            if (!ccA.Active || !ccB.Active) return false;
            //1.首先检测两个碰撞体在Y轴上是否会发生重叠，如果不会，那么一定不会发生碰撞
            if (ccA.MaxHeightPoint < ccB.MinHeightPoint || ccA.MinHeightPoint > ccB.MaxHeightPoint) return false;
            //当两个圆柱体在Y轴上存在重叠的可能时，只需要计算两个圆柱体半径之和是否小于两个圆柱体之间的距离即可
            FixIntVector3 colliderAPos = new FixIntVector3(ccA.X, ccA.Y, ccA.Z);
            FixIntVector3 colliderBPos = new FixIntVector3(ccB.X, ccA.Y, ccB.Z);
            //获取两个点之间的向量
            FixIntVector3 dir = colliderAPos - colliderBPos;
            //计算平方长度是否小于两个碰撞体半径和的平方
            return dir.sqrMagnitude < FixIntMath.Pow(ccA.Radius + ccB.Radius, 2);
        }

        public bool DetectCollision(FixintCylinderCollider3D cylinder, FixIntBoxCollider3D box, bool isUseAdjustPos = false)
        {
            //有任意一个Collider没有开启碰撞，则碰撞无效
            if (!cylinder.Active || !box.Active) return false;
            //1.获取两个物体之间的向量
            FixIntVector3 dir = cylinder.LogicPos - box.LogicPos;
            //2.计算向量在X轴和Z轴上的投影
            FixInt dotX = FixIntVector3.Dot(dir, FixIntVector3.right);
            FixInt dotZ = FixIntVector3.Dot(dir, FixIntVector3.forward);
            //3.限制投影长度在BOX包围盒内
            FixInt clampX = FixIntMath.Clamp(dotX, -box.Size.x / 2, box.Size.x / 2);
            FixInt clampZ = FixIntMath.Clamp(dotZ, -box.Size.z / 2, box.Size.z / 2);
            //4.计算表面距离圆柱体最近的接触点+轴向偏移(最近接触点)
            FixIntVector3 closedPoint = new FixIntVector3(box.X + clampX, box.Y, box.Z + clampZ);
            //计算最近接触点到圆柱体中心点的向量
            FixIntVector3 closedPointDir = cylinder.LogicPos - closedPoint;
            closedPointDir.y = 0;
            //检测圆柱体是否Box发生碰撞
            if (closedPointDir.sqrMagnitude > cylinder.Radius * cylinder.Radius)
            {
                return false;
            }
            else
            {
                //只需要检测BOX是否在圆柱体的上方或者下方
                FixInt maxHeight = box.Y + box.Size.y / 2;
                FixInt minHeight = box.Y - box.Size.y / 2;
                //验证BOX是否在Cylinder高度内
                if (maxHeight <= cylinder.MaxHeightPoint && maxHeight >= cylinder.MinHeightPoint ||
                    minHeight >= cylinder.MinHeightPoint && minHeight <= cylinder.MaxHeightPoint)
                {
                    //是否开启位置修正
                    if (isUseAdjustPos)
                    {
                        //获取穿插向量的长度
                        FixInt closedDirLength = closedPointDir.magnitude;
                        cylinder.AdjustPos = closedPointDir.normalized * (cylinder.Radius - closedDirLength);
                    }

                    return true;
                }
            }
            //根据碰撞的穿插长度修正圆柱体的位置 TODO

            return false;
        }

        public bool DetectCollision(FixintCylinderCollider3D cylinder, FixIntSphereCollider3D sphere,
            bool isUseAdjustPos = false)
        {
            //有任意一个Collider没有开启碰撞，则碰撞无效
            if (!cylinder.Active || !sphere.Active) return false;
            //1.首先去判断圆球是否在圆柱体的高度内
            if (cylinder.ContainTargetHeight(sphere.Y))
            {
                //获取圆柱体和圆球同高度的位置
                FixIntVector3 identicalHeightPos = new FixIntVector3(cylinder.X, sphere.Y, cylinder.Z);
                //获取两个碰撞体之间的距离
                FixIntVector3 dir = identicalHeightPos - sphere.LogicPos;
                //检测两个碰撞体是否会发生碰撞
                return dir.sqrMagnitude < FixIntMath.Pow(sphere.Radius + cylinder.Radius, 2);
            }
            else
            {
                //获取圆柱体距离圆球体最近的点
                FixIntVector3 closedPointOnCylinder = new FixIntVector3(
                    FixIntMath.Clamp(sphere.X, cylinder.X - cylinder.Radius, cylinder.X + cylinder.Radius),
                    sphere.Y > cylinder.MaxHeightPoint ? cylinder.MaxHeightPoint : cylinder.MinHeightPoint,
                    FixIntMath.Clamp(sphere.Z, cylinder.Z - cylinder.Radius, cylinder.Z + cylinder.Radius)
                );
                //获取圆柱体距离圆球最近的点到圆球中心点的向量
                FixIntVector3 dir = sphere.LogicPos - closedPointOnCylinder;
                //若距离小于圆球的半径，则说明发生碰撞
                return dir.sqrMagnitude < FixIntMath.Pow(sphere.Radius, 2);
            }
        }

        #region 添加删除

        public void AddCollider3D(FixIntCollider3D collider3D)
        {
            collider3DList.Add(collider3D);
        }

        public void RemoveCollider3D(FixIntCollider3D collider3D)
        {
            collider3DList.Remove(collider3D);
        }

        #endregion

        public FixIntBoxCollider3D GenerateFixIntBoxFormUnityCollider(BoxCollider uCollider,
            bool managerCollider = true)
        {
            var collider3D = new FixIntBoxCollider3D(new FixIntVector3(uCollider.transform.position),
                new FixIntVector3(uCollider.center), new FixIntVector3(uCollider.transform.localScale));

            collider3D.SetRenderObj(uCollider.gameObject);
            if (managerCollider)
            {
                AddCollider3D(collider3D);
            }

            return collider3D;
        }

        public FixIntSphereCollider3D GenerateFixIntSphereFormUnityCollider(SphereCollider uCollider,
            bool managerCollider = true)
        {
            var collider3D = new FixIntSphereCollider3D(new FixIntVector3(uCollider.transform.position),
                new FixIntVector3(uCollider.center), uCollider.radius);

            collider3D.SetRenderObj(uCollider.gameObject);
            if (managerCollider)
            {
                AddCollider3D(collider3D);
            }

            return collider3D;
        }

        public FixintCylinderCollider3D GenerateFixIntCylinderFormUnityCollider(CapsuleCollider uCollider,
            bool managerCollider = true)
        {
            var collider = new FixintCylinderCollider3D(new FixIntVector3(uCollider.transform.position),
                new FixInt(uCollider.radius),
                new FixIntVector3(uCollider.center), uCollider.height,
                new FixIntVector3(uCollider.transform.localScale));
            collider.SetRenderObj(uCollider.gameObject);
            if (managerCollider)
            {
                AddCollider3D(collider);
            }

            return collider;
        }
    }
}