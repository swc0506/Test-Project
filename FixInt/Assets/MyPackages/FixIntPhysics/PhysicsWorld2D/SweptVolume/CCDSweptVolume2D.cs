using My.Physics2D;
using UnityEngine;
using UnityEngine.UI;
using ZM.FixIntMath;

public class CCDSweptVolume2D
{
    /// <summary>
    /// 连续碰撞包围盒
    /// </summary>
    private readonly FixIntBoxCollider2D mSweepBoxCollider;

    /// <summary>
    /// 连续碰撞检测
    /// </summary>
    /// <param name="boxCollider2D"></param>
    /// <param name="curPos"></param>
    /// <param name="lastPos"></param>
    public CCDSweptVolume2D(FixIntBoxCollider2D boxCollider2D, FixIntVector2 curFramePos, FixIntVector2 lastFramePos)
    {
        //连续碰撞检测从上一帧结束位置到当前帧数所在的位置生成一个新的碰撞体积，用于检测高速穿透的问题
        FixIntVector2 size = CalculateSweptVolume(lastFramePos, curFramePos, boxCollider2D.Size / 2);
        FixIntVector2 midPos = lastFramePos + (curFramePos - lastFramePos) / 2;
        mSweepBoxCollider = new FixIntBoxCollider2D(midPos, boxCollider2D.Center, size);
        //测试代码
        GameObject tempObj = new GameObject("CcdSweptVolume");
        tempObj.AddComponent<Image>();
        tempObj.transform.SetParent(GameObject.Find("DemoCanvas/Panel").transform);
        tempObj.transform.localPosition = midPos.ToVector2();
        tempObj.GetComponent<RectTransform>().sizeDelta = size.ToVector2();
        tempObj.transform.SetSiblingIndex(2);

        // mSweepBoxCollider = new FixIntBoxCollider2D(lastFramePos,boxCollider2D.Center,boxCollider2D.Size);
        // //测试代码
        // GameObject tempObj = new GameObject("CcdSweptVolume");
        // tempObj.AddComponent<Image>();
        // tempObj.transform.SetParent(GameObject.Find("DemoCanvas/Panel").transform);
        // tempObj.transform.localPosition = lastFramePos.ToVector2();
        // tempObj.GetComponent<RectTransform>().sizeDelta = boxCollider2D.Size.ToVector2();
        // tempObj.transform.SetSiblingIndex(2);
    }

    /// <summary>
    /// 检测碰撞
    /// </summary>
    /// <param name="collider2D"></param>
    /// <returns></returns>
    public bool DetectCollider(FixIntCollider2D collider2D)
    {
        return mSweepBoxCollider.DetectCollision(collider2D);
    }

    /// <summary>
    /// 计算碰撞体积
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    //计算扫描体积
    private FixIntVector2 CalculateSweptVolume(FixIntVector2 start, FixIntVector2 end, FixIntVector2 size)
    {
        //计算x和y最小值
        FixIntVector2 min = new FixIntVector2(FixIntMath.Min(start.x, end.x), FixIntMath.Min(start.y, end.y)) - size;
        //计算x和y最大值
        FixIntVector2 max = new FixIntVector2(FixIntMath.Max(start.x, end.x), FixIntMath.Max(start.y, end.y)) + size;
        //计算Box宽度和高度
        return new FixIntVector2(max.x - min.x, max.y - min.y);
    }
}