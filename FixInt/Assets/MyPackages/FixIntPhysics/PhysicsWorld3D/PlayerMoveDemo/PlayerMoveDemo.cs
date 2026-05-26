using System.Collections.Generic;
using My.Physics3D;
using UnityEngine;
using UnityEngine.InputSystem;
using ZM.FixIntMath;

namespace My.FixIntPhysics3D
{
    public class PlayerMoveDemo : MonoBehaviour
    {
        public Transform[] boxTransArr;

        private FixIntVector3 mLogicPos;
        public FixInt moveSpeed=4;
        public Transform playerTrans;
        private FixintCylinderCollider3D mPlayerCollider;
        
        private readonly List<FixIntCollider3D> mWallColliderList = new List<FixIntCollider3D>();
        private int baseColorID;

        void Start()
        {
            mLogicPos = new FixIntVector3(transform.position);
            //创建碰撞体
            foreach (var item in boxTransArr)
            {
                var boxCollider = PhysicsWorld3D.Instance.GenerateFixIntBoxFormUnityCollider(item.GetComponent<BoxCollider>(),false);
                //胶囊体碰撞体
                boxCollider.OnCollisionEnter3DAction += (target) => { SetColor(boxCollider, Color.red); };
                //监听碰撞退出回调
                boxCollider.OnCollisionExit3DAction += (target) => { SetColor(boxCollider, Color.white); };
                mWallColliderList.Add(boxCollider);
            }
            //创建玩家碰撞体
            mPlayerCollider = PhysicsWorld3D.Instance.GenerateFixIntCylinderFormUnityCollider(playerTrans.GetComponent<CapsuleCollider>(),false);
            mPlayerCollider.SetUseAdjustPos(true);
            
            //胶囊体碰撞体
            mPlayerCollider.OnCollisionEnter3DAction += (target) => { SetColor(mPlayerCollider, Color.red); };
            //监听碰撞退出回调
            mPlayerCollider.OnCollisionExit3DAction += (target) => { SetColor(mPlayerCollider, Color.white); };
            
            baseColorID = Shader.PropertyToID("_BaseColor");
        }

        private void SetColor(FixIntCollider3D collider, Color color)
        {
            collider.RenderObj.GetComponent<MeshRenderer>().material.SetColor(baseColorID, color);
        }


        private void FixedUpdate()
        {
            // 获取输入
            FixInt moveHorizontal = (Keyboard.current.dKey.isPressed ? 1 : (Keyboard.current.aKey.isPressed ? -1 : 0));
            FixInt moveVertical = (Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0));

            // 移动
            FixIntVector3 movement = new FixIntVector3(moveHorizontal, 0.0f, moveVertical);
            mLogicPos += movement.normalized * moveSpeed * Time.deltaTime;
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
            playerTrans.position = mLogicPos.ToVector3();
            SyncColliderPos();
        }

        private void SyncColliderPos()
        {
            //计算逻辑位置，由于当前物体是通过手动拖拽进行移动，故使用物体的Position
            mPlayerCollider.SyncLogicPos(new FixIntVector3(mPlayerCollider.RenderObj.transform.position));
        }

        private void OnApplicationQuit()
        {
            mPlayerCollider.OnRelease();
        }
        
    }
}