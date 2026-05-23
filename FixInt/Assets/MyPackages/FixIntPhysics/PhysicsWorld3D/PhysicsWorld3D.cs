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
        private readonly List<FixIntCollider3D> collider2DList = new List<FixIntCollider3D>();

        /// <summary>
        /// 逻辑帧更新接口
        /// </summary>
        public void OnLogicFrameUpdate()
        {
            for (int i = 0; i < collider2DList.Count; i++)
            {
                var item = collider2DList[i];
                for (int j = 0; j < collider2DList.Count; j++)
                {
                    var target = collider2DList[j];
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

        #region 添加删除

        public void AddCollider3D(FixIntCollider3D collider3D)
        {
            collider2DList.Add(collider3D);
        }

        public void RemoveCollider3D(FixIntCollider3D collider3D)
        {
            collider2DList.Remove(collider3D);
        }

        #endregion

        public FixIntBoxCollider3D GenerateFixIntBoxFormUnityCollider(BoxCollider uCollider,
            bool managerCollider = true)
        {
            var collider3D = new FixIntBoxCollider3D(new FixIntVector3(uCollider.transform.localPosition),
                new FixIntVector3(uCollider.center), new FixIntVector2(uCollider.size));

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
            var collider3D = new FixIntSphereCollider3D(new FixIntVector3(uCollider.transform.localPosition),
                new FixIntVector3(uCollider.center), uCollider.radius);

            collider3D.SetRenderObj(uCollider.gameObject);
            if (managerCollider)
            {
                AddCollider3D(collider3D);
            }

            return collider3D;
        }
    }
}