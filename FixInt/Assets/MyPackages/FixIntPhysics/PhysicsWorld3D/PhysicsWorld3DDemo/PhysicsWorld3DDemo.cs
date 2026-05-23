using System.Collections.Generic;
using My.Physics3D;
using UnityEngine;
using UnityEngine.UI;
using ZM.FixIntMath;

public class PhysicsWorld3DDemo : MonoBehaviour
{
    public GameObject boxA;
    public GameObject boxB;
    public GameObject sphereA;
    public GameObject sphereB;

    private readonly List<FixIntCollider3D> mCollider3DList = new List<FixIntCollider3D>();

    private void Start()
    {
        var boxColliderA =
            PhysicsWorld3D.Instance.GenerateFixIntBoxFormUnityCollider(boxA.GetComponent<BoxCollider>());
        var boxColliderB =
            PhysicsWorld3D.Instance.GenerateFixIntBoxFormUnityCollider(boxB.GetComponent<BoxCollider>());
        var sphereColliderA =
            PhysicsWorld3D.Instance.GenerateFixIntSphereFormUnityCollider(sphereA.GetComponent<SphereCollider>());
        var sphereColliderB =
            PhysicsWorld3D.Instance.GenerateFixIntSphereFormUnityCollider(sphereB.GetComponent<SphereCollider>());

        mCollider3DList.Add(boxColliderA);
        mCollider3DList.Add(boxColliderB);
        mCollider3DList.Add(sphereColliderA);
        mCollider3DList.Add(sphereColliderB);

        //监听碰撞体进入回调
        boxColliderA.OnCollisionEnter3DAction += (target) => { SetImageColor(boxColliderA, Color.red); };
        boxColliderB.OnCollisionEnter3DAction += (target) => { SetImageColor(boxColliderB, Color.red); };
        sphereColliderA.OnCollisionEnter3DAction += (target) => { SetImageColor(sphereColliderA, Color.red); };
        sphereColliderB.OnCollisionEnter3DAction += (target) => { SetImageColor(sphereColliderB, Color.red); };

        //监听碰撞体退出回调
        boxColliderA.OnCollisionExit3DAction += (target) => { SetImageColor(boxColliderA, Color.white); };
        boxColliderB.OnCollisionExit3DAction += (target) => { SetImageColor(boxColliderB, Color.white); };
        sphereColliderA.OnCollisionExit3DAction += (target) => { SetImageColor(sphereColliderA, Color.white); };
        sphereColliderB.OnCollisionExit3DAction += (target) => { SetImageColor(sphereColliderB, Color.white); };
    }

    void Update()
    {
        SyncColliderData();
        PhysicsWorld3D.Instance.OnLogicFrameUpdate();
    }

    private void SetImageColor(FixIntCollider3D collider, Color color)
    {
        collider.RenderObj.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// 同步碰撞器数据
    /// </summary>
    private void SyncColliderData()
    {
        foreach (var item in mCollider3DList)
        {
            item.SyncLogicPos(new FixIntVector3(item.RenderObj.transform.localPosition));
            //只需要在碰撞体的半径或者大小发生变化的更新即可。这里每帧更新只是一个教学使用演示。
            //var itemRectTrans=item.RenderObj.transform as RectTransform; 
            //更新碰撞体大小和半径
            //item.SyncLogicSize(new FixIntVector2(itemRectTrans.sizeDelta)*new FixIntVector2(itemRectTrans.localScale));
            //更新碰撞体的半径
            //item.SyncLogicRadius((new FixInt(itemRectTrans.sizeDelta.x)/2)*new FixInt(itemRectTrans.localScale.x));
        }
    }

    public void OnDestroy()
    {
        foreach (var item in mCollider3DList)
        {
            item.OnRelease();
        }

        mCollider3DList.Clear();
    }
}