using System.Collections.Generic;
using My.Physics2D;
using UnityEngine;
using UnityEngine.UI;
using ZM.FixIntMath;
using UnityEngine.InputSystem;

namespace My.FixIntPhysics2D
{
    public class PlayerMove2D : MonoBehaviour
    {
        public Transform[] boxTransArr;

        private FixIntVector2 mLogicPos;
        public int moveSpeed=400;
        public Transform playerTrans;
        private FixIntBoxCollider2D mPlayerCollider;
        

        private readonly List<FixIntBoxCollider2D> mColliderList = new List<FixIntBoxCollider2D>();
        private readonly List<FixIntBoxCollider2D> mWallColliderList = new List<FixIntBoxCollider2D>();

        void Start()
        {
            mLogicPos = new FixIntVector2(transform.localPosition);
            //创建碰撞体
            foreach (var item in boxTransArr)
            {
                var boxCollider =
                    PhysicsWorld2D.Instance.GenerateFixIntBoxFormUnityCollider(
                        item.GetComponent<BoxCollider2D>(), false);
                //胶囊体碰撞体
                boxCollider.OnCollisionEnter2DAction += (target) => { SetColor(boxCollider, Color.red); };
                //监听碰撞退出回调
                boxCollider.OnCollisionExit2DAction += (target) => { SetColor(boxCollider, Color.white); };
                mWallColliderList.Add(boxCollider);
            }
            //创建玩家碰撞体
            mPlayerCollider = PhysicsWorld2D.Instance.GenerateFixIntBoxFormUnityCollider(playerTrans.GetComponent<BoxCollider2D>(),false);
            mPlayerCollider.SetUseAdjustPos(true);
            //缓存碰撞体
            mColliderList.Add(mPlayerCollider);
            
            //胶囊体碰撞体
            mPlayerCollider.OnCollisionEnter2DAction += (target) => { SetColor(mPlayerCollider, Color.red); };
            //监听碰撞退出回调
            mPlayerCollider.OnCollisionExit2DAction += (target) => { SetColor(mPlayerCollider, Color.white); };
            
        }

        private void SetColor(FixIntCollider2D collider2D, Color color)
        {
            collider2D.RenderObj.GetComponent<Image>().color = color;
        }

        private void FixedUpdate()
        {
            // 获取输入
            FixInt moveHorizontal = (Keyboard.current.dKey.isPressed ? 1 : (Keyboard.current.aKey.isPressed ? -1 : 0)); // A/D 或 左右箭头
            FixInt moveVertical = (Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0)); // W/S 或 上下箭头

            // 移动
            FixIntVector2 movement = new FixIntVector2(moveHorizontal,  moveVertical);
            mLogicPos += movement.normalized * moveSpeed * Time.fixedDeltaTime;
            SyncColliderPos();
            
            //检测玩家是否碰撞
            for (int i = 0; i < mWallColliderList.Count; i++)
            {
                mPlayerCollider.DetectCollision(mWallColliderList[i]);
               
                if (mPlayerCollider.CanAdjust)
                {
                    mLogicPos += mPlayerCollider.AdjustPos;
                  
                }
            }
            playerTrans.localPosition = mLogicPos.ToVector2();
            SyncColliderPos();
        }

        private void SyncColliderPos()
        {
            foreach (var item in mColliderList)
            {
                //同步碰撞体位置 由于当前物体是通过手动拖拽进行移动，故使用物体的Position，应使用LogicPos逻辑位置
                item.SyncLogicPos(item.RenderObj.transform.localPosition);
                RectTransform itemRectTrans = item.RenderObj.transform as RectTransform;
                //同步碰撞体大小。使用示例：一般在碰撞体大小发生变化时进行同步
                item.SyncLogicSize(new FixIntVector2(itemRectTrans.sizeDelta) *
                                   new FixIntVector2(itemRectTrans.localScale));
                item.SyncLogicRadius(new FixInt(itemRectTrans.sizeDelta.x) / 2 * itemRectTrans.localScale.x);
            }
        }
    }
}