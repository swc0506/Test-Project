using System.Collections.Generic;
using My.Physics3D;
using UnityEngine;
using UnityEngine.UI;
using ZM.FixIntMath;

/*----------------------------------------------------------------------------
* Title: 帧同步定点数学碰撞库
*
* Author: 铸梦
*
* Date: 2025.02.19
*
* Description:可应用于状态同步或帧同步中，主要解决不同平台下float精度误差问题，保证不同平台在相同属于源的情况下，结果计算的一致性
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/
public class PhysicsWorld3DDemo : MonoBehaviour
{
    public GameObject boxA;
    public GameObject boxB;

    public GameObject sphereA;
    public GameObject sphereB;

    public GameObject cylinderA;
    public GameObject cylinderB;

    private int baseColorID;

    private readonly List<FixIntCollider3D> mColliderList = new List<FixIntCollider3D>();

    void Start()
    {
        Debug.Log("Start Collider Env");
        //生成Box碰撞体
        var boxColliderA = PhysicsWorld3D.Instance.GenerateFixIntBoxFormUnityCollider(boxA.GetComponent<BoxCollider>());
        var boxColliderB = PhysicsWorld3D.Instance.GenerateFixIntBoxFormUnityCollider(boxB.GetComponent<BoxCollider>());
        //生成圆球碰撞体
        var sphereColliderA =
            PhysicsWorld3D.Instance.GenerateFixIntSphereFormUnityCollider(sphereA.GetComponent<SphereCollider>());
        var sphereColliderB =
            PhysicsWorld3D.Instance.GenerateFixIntSphereFormUnityCollider(sphereB.GetComponent<SphereCollider>());
        //生成圆球碰撞体
        var cylinderColliderA =
            PhysicsWorld3D.Instance.GenerateFixIntCylinderFormUnityCollider(cylinderA.GetComponent<CapsuleCollider>());
        var cylinderColliderB =
            PhysicsWorld3D.Instance.GenerateFixIntCylinderFormUnityCollider(cylinderB.GetComponent<CapsuleCollider>());

        //缓存碰撞体用来更新位置和大小以及半径
        mColliderList.Add(boxColliderA);
        mColliderList.Add(boxColliderB);
        mColliderList.Add(sphereColliderA);
        mColliderList.Add(sphereColliderB);
        mColliderList.Add(cylinderColliderA);
        mColliderList.Add(cylinderColliderB);

        // 缓存属性ID
        baseColorID = Shader.PropertyToID("_BaseColor");

        //监听碰撞体进入回调
        boxColliderA.OnCollisionEnter3DAction += (target) => { SetColor(boxColliderA, Color.red); };
        boxColliderB.OnCollisionEnter3DAction += (target) => { SetColor(boxColliderB, Color.red); };
        sphereColliderA.OnCollisionEnter3DAction += (target) => { SetColor(sphereColliderA, Color.red); };
        sphereColliderB.OnCollisionEnter3DAction += (target) => { SetColor(sphereColliderB, Color.red); };
        cylinderColliderA.OnCollisionEnter3DAction += (target) => { SetColor(cylinderColliderA, Color.red); };
        cylinderColliderB.OnCollisionEnter3DAction += (target) => { SetColor(cylinderColliderB, Color.red); };

        //监听碰撞体退出回调
        boxColliderA.OnCollisionExit3DAction += (target) => { SetColor(boxColliderA, Color.white); };
        boxColliderB.OnCollisionExit3DAction += (target) => { SetColor(boxColliderB, Color.white); };
        sphereColliderA.OnCollisionExit3DAction += (target) => { SetColor(sphereColliderA, Color.white); };
        sphereColliderB.OnCollisionExit3DAction += (target) => { SetColor(sphereColliderB, Color.white); };
        cylinderColliderA.OnCollisionExit3DAction += (target) => { SetColor(cylinderColliderA, Color.white); };
        cylinderColliderB.OnCollisionExit3DAction += (target) => { SetColor(cylinderColliderB, Color.white); };
    }

    private void SetColor(FixIntCollider3D collider, Color color)
    {
        string result = color == Color.red ? "碰撞进入" : "碰撞离开";
        Debug.Log($"碰撞体{result} name:{collider.RenderObj.name}");
        collider.RenderObj.GetComponent<MeshRenderer>().material.SetColor(baseColorID, color);
    }

    void Update()
    {
        SyncColliderData();
        //模拟逻辑帧更新
        PhysicsWorld3D.Instance.OnLogicFrameUpdate();
    }

    /// <summary>
    /// 同步碰撞体数据
    /// </summary>
    private void SyncColliderData()
    {
        foreach (var item in mColliderList)
        {
            //只需要在碰撞体的半径或者大小发生变化的更新即可。这里每帧更新只是一个教学使用演示。
            item.SyncLogicPos(new FixIntVector3(item.RenderObj.transform.position));
            //更新碰撞体大小和半径
            item.SyncLogicSize(new FixIntVector3(item.RenderObj.transform.localScale));
            //更新碰撞体的半径
            item.SyncLogicRadius(new FixInt(item.RenderObj.transform.localScale.x / 2));
        }
    }

    public void OnApplicationQuit()
    {
        foreach (var item in mColliderList)
        {
            item.OnRelease();
        }

        mColliderList.Clear();
    }
}