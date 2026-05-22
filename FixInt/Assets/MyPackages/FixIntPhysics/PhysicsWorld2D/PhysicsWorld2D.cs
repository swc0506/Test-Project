using System.Collections.Generic;
using UnityEngine;
using ZM.FixIntMath;

namespace  My.Physics2D
{
    public class PhysicsWorld2D
    {
        private static PhysicsWorld2D instance;
        public static PhysicsWorld2D Instance
        {
            get
            {
                instance ??= new PhysicsWorld2D();
                return instance;
            }
        }
        
        /// <summary>
        /// 所有2D碰撞器
        /// </summary>
        private readonly List<FixIntCollider2D> collider2DList = new List<FixIntCollider2D>();
        
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
        /// <param name="fbcA"></param>
        /// <param name="fbcB"></param>
        /// <param name="isUseAdjustPos"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntBoxCollider2D fbcA, FixIntBoxCollider2D fbcB, bool isUseAdjustPos = false)
        {
            if (!fbcA.Active || !fbcB.Active)
                return false;

            if (isUseAdjustPos)
            {
                FixIntVector2 dir = fbcB.LogicPos - fbcA.LogicPos;

                FixInt dotDisX = FixIntVector2.Dot(dir, FixIntVector2.right);
                FixInt dotDisY = FixIntVector2.Dot(dir, FixIntVector2.up);

                // 分别计算 X/Y 轴上 A 与 B 的重叠量（穿透深度）
                FixInt overlapX = (fbcA.BoxWidth + fbcB.BoxWidth) - FixIntMath.Abs(dotDisX);
                FixInt overlapY = (fbcA.BoxHeight + fbcB.BoxHeight) - FixIntMath.Abs(dotDisY);

                // 两个轴都有重叠才算碰撞
                if (overlapX > FixInt.Zero && overlapY > FixInt.Zero)
                {
                    FixIntVector2 adjustDir;
                    FixInt adjustLength;

                    // 选择穿透最浅的轴（最小穿透向量原则），使推出方向最自然
                    if (overlapX < overlapY)
                    {
                        adjustDir = dotDisX > FixInt.Zero ? FixIntVector2.left : FixIntVector2.right;
                        adjustLength = overlapX;
                    }
                    else
                    {
                        adjustDir = dotDisY > FixInt.Zero ? FixIntVector2.down : FixIntVector2.up;
                        adjustLength = overlapY;
                    }

                    fbcA.AdjustPos = adjustDir * adjustLength;
                    return true;
                }
                return false;
            }
            else
            {
                bool detectX = fbcA.X + fbcA.BoxWidth >= fbcB.X - fbcB.BoxWidth &&
                               fbcA.X - fbcA.BoxWidth <= fbcB.X + fbcB.BoxWidth;
                bool detectY = fbcA.Y + fbcA.BoxHeight >= fbcB.Y - fbcB.BoxHeight &&
                               fbcA.Y - fbcA.BoxHeight <= fbcB.Y + fbcB.BoxHeight; 
                return detectY && detectX;
            }
            
        }
        
        /// <summary>
        /// 检测Box与Sphere碰撞
        /// </summary>
        /// <param name="fbcA"></param>
        /// <param name="fbsB"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntBoxCollider2D fbcA, FixIntSphereCollider2D fbsB, bool isUseAdjustPos = false)
        {
            if (!fbcA.Active || !fbsB.Active) return false;
            
            // 向量
            FixIntVector2 dir = fbsB.LogicPos - fbcA.LogicPos;

            FixInt dotDisX = FixIntVector2.Dot(dir, FixIntVector2.right);
            FixInt dotDisY = FixIntVector2.Dot(dir, FixIntVector2.up);

            FixInt clampX = FixIntMath.Clamp(dotDisX, -fbcA.BoxWidth, fbcA.BoxWidth);
            FixInt clampY = FixIntMath.Clamp(dotDisY, -fbcA.BoxHeight, fbcA.BoxHeight);
            FixIntVector2 closedPoint = new FixIntVector2(fbcA.X + clampX, fbcA.Y + clampY);
            
            //获取最邻近圆球的点到圆球中心点的向量
            FixIntVector2 closedPointDir = fbsB.LogicPos - closedPoint;

            return closedPointDir.sqrMagnitude < fbsB.Radius * fbsB.Radius;
        }
        
        /// <summary>
        /// 检测Sphere碰撞
        /// </summary>
        /// <param name="fbsA"></param>
        /// <param name="fbsB"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntSphereCollider2D fbsA, FixIntSphereCollider2D fbsB, bool isUseAdjustPos = false)
        {
            if (!fbsA.Active || !fbsB.Active)
                return false;
            
            FixInt distance = (fbsA.LogicPos - fbsB.LogicPos).SqrMagnitude();
            FixInt radiusSum = fbsA.Radius + fbsB.Radius;
            return distance <= radiusSum * radiusSum;
        }

        #region 添加删除

        public void AddCollider2D(FixIntCollider2D collider2D)
        {
            collider2DList.Add(collider2D);
        }
        
        public void RemoveCollider2D(FixIntCollider2D collider2D)
        {
            collider2DList.Remove(collider2D);
        }

        #endregion

        public FixIntBoxCollider2D GenerateFixIntBoxFormUnityCollider(BoxCollider2D uCollider, bool managerCollider = true)
        {
            var collider2D = new FixIntBoxCollider2D(uCollider.transform.localPosition,
                new FixIntVector2(uCollider.offset), new FixIntVector2(uCollider.size));

            collider2D.SetRenderObj(uCollider.gameObject);
            if (managerCollider)
            {
                AddCollider2D(collider2D);
            }
            return collider2D;
        }
        
        public FixIntSphereCollider2D GenerateFixIntSphereFormUnityCollider(CircleCollider2D uCollider, bool managerCollider = true)
        {
            var collider2D = new FixIntSphereCollider2D(uCollider.transform.localPosition,
                new FixIntVector2(uCollider.offset), uCollider.radius);

            collider2D.SetRenderObj(uCollider.gameObject);
            if (managerCollider)
            {
                AddCollider2D(collider2D);
            }
            return collider2D;
        }
    }
}