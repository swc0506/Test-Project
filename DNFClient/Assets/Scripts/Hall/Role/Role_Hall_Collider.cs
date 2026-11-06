using FixIntPhysics;
using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMGC.Hall;

public partial class Role_Hall : MonoBehaviour
{
    private FixIntBoxCollider mBoxCollider;

    private Map mMap;

    private Vector3 boxConter;

    private FixIntVector3 mColliderPos;
    /// <summary>
    /// 初始化角色碰撞体
    /// </summary>
    public void InitCollider(Map map)
    {
        mMap = map;
        if (mBoxCollider == null)
        {
            BoxColliderGizmo editorCollider = GetComponent<BoxColliderGizmo>();
            editorCollider.enabled = false;
            boxConter = editorCollider.mConter;
            //创建碰撞体
            mBoxCollider = new FixIntBoxCollider(editorCollider.mSize, editorCollider.mConter);
            mBoxCollider.SetBoxData(editorCollider.mConter, editorCollider.mSize);
        }
        mBoxCollider.OnCollisionEnterAction = OnFixintBoxCollisionEnter;
        mMap.AddEntryColliderEvent(mBoxCollider);
    }
    /// <summary>
    /// 更新碰撞体位置
    /// </summary>
    public void UpdateCollider()
    {
        if (mBoxCollider != null) 
        { 
            Vector3 boxPosition = transform.position + boxConter;
            mColliderPos.x= boxPosition.x;
            mColliderPos.y= boxPosition.y; 
            mColliderPos.z= boxPosition.z;
            mBoxCollider.UpdateColliderInfo(mColliderPos, mBoxCollider.Size);
        }
    }
    /// <summary>
    /// 碰撞进入
    /// </summary>
    public async void OnFixintBoxCollisionEnter(ColliderBehaviour targetCollider, MonoBehaviour targetMono)
    {
        Debuger.Log("发生碰撞 gameObject Name:"+gameObject.name+" target Name:"+targetMono.name);
        MapEntry entryCfg= targetMono.GetComponent<MapEntry>();
        ActiveMove(false);
        if (entryCfg.mapType == MapType.Dungeons)
        {
            HallWorld.GetExitsMsgMgr<DungeonSelectMsgMgr>().SendEnterDungeonsMessage(HallWorld.GetExitsDataMgr<TeamDataMgr>().TeamID);
        }
        else
        {
            await HallWorld.GetExitsLogicCtrl<MapLogicCtrl>().LoadMapAsync(entryCfg.mapType, entryCfg.doorType);
        }
       
    }
    /// <summary>
    /// 
    /// </summary>
    public void ReleaseCollider()
    {
        mMap.RemoveEntryColliderEvent(mBoxCollider);
        mBoxCollider.OnCollisionEnterAction =null;
    }
}
