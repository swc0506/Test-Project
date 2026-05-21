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
                // 向量
                FixIntVector2 dir = fbcB.LogicPos - fbcA.LogicPos;

                FixInt dotDisX = FixIntVector2.Dot(dir, FixIntVector2.right);
                FixInt dotDisY = FixIntVector2.Dot(dir, FixIntVector2.up);

                FixInt clampX = FixIntMath.Clamp(dotDisX, -fbcB.BoxWidth, fbcB.BoxWidth);
                FixInt clampY = FixIntMath.Clamp(dotDisY, -fbcB.BoxHeight, fbcB.BoxHeight);
                FixIntVector2 closedPoint = new FixIntVector2(fbcB.X + clampX, fbcB.Y + clampY);
                
                FixIntVector2 closedPointDir = fbcA.LogicPos - closedPoint;

                FixInt dotLenX = FixIntMath.Abs(FixIntVector2.Dot(dir, FixIntVector2.right));
                FixInt dotLenY = FixIntMath.Abs(FixIntVector2.Dot(dir, FixIntVector2.up));
                if (dotLenX < fbcA.BoxWidth && dotLenY < fbcA.BoxHeight)
                {
                    //获取最邻近点到目标Collider中心点的向量的长度
                    FixInt closedDirLength = closedPointDir.magnitude;
                    //计算穿插距离
                    FixInt insertLength = (dotLenY> dotLenX? fbcA.BoxHeight: fbcA.BoxWidth) - closedDirLength;
                    //计算修正位置
                    fbcA.AdjustPos= -(closedPointDir.normalized * insertLength);
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