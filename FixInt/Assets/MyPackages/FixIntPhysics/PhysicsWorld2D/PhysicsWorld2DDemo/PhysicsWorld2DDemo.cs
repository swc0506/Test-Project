using System;
using System.Collections.Generic;
using My.Physics2D;
using UnityEngine;
using UnityEngine.UI;
using ZM.FixIntMath;

public class PhysicsWorld2DDemo : MonoBehaviour
{
    public GameObject boxA;
    public GameObject boxB;
    public GameObject sphereA;
    public GameObject sphereB;

    private readonly List<FixIntCollider2D> mCollider2DList = new List<FixIntCollider2D>();

    private void Start()
    {
        var boxColliderA =
            PhysicsWorld2D.Instance.GenerateFixIntBoxFormUnityCollider(boxA.GetComponent<BoxCollider2D>());
        var boxColliderB =
            PhysicsWorld2D.Instance.GenerateFixIntBoxFormUnityCollider(boxB.GetComponent<BoxCollider2D>());
        var sphereColliderA =
            PhysicsWorld2D.Instance.GenerateFixIntSphereFormUnityCollider(sphereA.GetComponent<CircleCollider2D>());
        var sphereColliderB =
            PhysicsWorld2D.Instance.GenerateFixIntSphereFormUnityCollider(sphereB.GetComponent<CircleCollider2D>());

        mCollider2DList.Add(boxColliderA);
        mCollider2DList.Add(boxColliderB);
        mCollider2DList.Add(sphereColliderA);
        mCollider2DList.Add(sphereColliderB);

        //监听碰撞体进入回调
        boxColliderA.OnCollisionEnter2DAction += (target) => { SetImageColor(boxColliderA, Color.red); };
        boxColliderB.OnCollisionEnter2DAction += (target) => { SetImageColor(boxColliderB, Color.red); };
        sphereColliderA.OnCollisionEnter2DAction += (target) => { SetImageColor(sphereColliderA, Color.red); };
        sphereColliderB.OnCollisionEnter2DAction += (target) => { SetImageColor(sphereColliderB, Color.red); };

        //监听碰撞体退出回调
        boxColliderA.OnCollisionExit2DAction += (target) => { SetImageColor(boxColliderA, Color.white); };
        boxColliderB.OnCollisionExit2DAction += (target) => { SetImageColor(boxColliderB, Color.white); };
        sphereColliderA.OnCollisionExit2DAction += (target) => { SetImageColor(sphereColliderA, Color.white); };
        sphereColliderB.OnCollisionExit2DAction += (target) => { SetImageColor(sphereColliderB, Color.white); };
    }

    void Update()
    {
        SyncColliderData();
        PhysicsWorld2D.Instance.OnLogicFrameUpdate();
    }

    private void SetImageColor(FixIntCollider2D collider, Color color)
    {
        collider.RenderObj.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// 同步碰撞器数据
    /// </summary>
    private void SyncColliderData()
    {
        foreach (var item in mCollider2DList)
        {
            item.SyncLogicPos(item.RenderObj.transform.localPosition);
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
        foreach (var item in mCollider2DList)
        {
            item.OnRelease();
        }

        mCollider2DList.Clear();
    }
}