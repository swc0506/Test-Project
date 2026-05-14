using System.Collections.Generic;
using UnityEngine;

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
            return false;
        }
        
        /// <summary>
        /// 检测Box与Sphere碰撞
        /// </summary>
        /// <param name="fbcA"></param>
        /// <param name="fbsB"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntBoxCollider2D fbcA, FixIntSphereCollider2D fbsB)
        {
            return false;
        }
        
        /// <summary>
        /// 检测Sphere碰撞
        /// </summary>
        /// <param name="fbsA"></param>
        /// <param name="fbsB"></param>
        /// <returns></returns>
        public bool DetectCollision(FixIntSphereCollider2D fbsA, FixIntSphereCollider2D fbsB)
        {
            return false;
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
        
        
    }
}