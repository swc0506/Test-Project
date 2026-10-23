using UnityEngine;
using System.Collections.Generic;
using FixMath;

/// <summary>
/// 贝塞尔工具类
/// </summary>
public static class BezierUtils
{
    /// <summary>
    /// 线性贝塞尔曲线
    /// </summary>
    public static FixIntVector3 BezierCurve(FixIntVector3 p0, FixIntVector3 p1, FixInt t)
    {
        FixIntVector3 B = FixIntVector3.zero;
        B = (1 - t) * p0 + t * p1;
        return B;
    }

    /// <summary>
    /// 二阶贝塞尔曲线
    /// </summary>
    public static FixIntVector3 BezierCurve(FixIntVector3 p0, FixIntVector3 p1, FixIntVector3 p2, FixInt t)
    {
        FixIntVector3 B = FixIntVector3.zero;
        FixInt t1 = (1 - t) * (1 - t);
        FixInt t2 = 2 * t * (1 - t);
        FixInt t3 = t * t;
        B = t1 * p0 + t2 * p1 + t3 * p2;
        return B;
    }

    /// <summary>
    /// 三阶贝塞尔曲线
    /// </summary>
    public static FixIntVector3 BezierCurve(FixIntVector3 p0, FixIntVector3 p1, FixIntVector3 p2, FixIntVector3 p3, FixInt t)
    {
        FixIntVector3 B = FixIntVector3.zero;
        FixInt t1 = (1 - t) * (1 - t) * (1 - t);
        FixInt t2 = 3 * t * (1 - t) * (1 - t);
        FixInt t3 = 3 * t * t * (1 - t);
        FixInt t4 = t * t * t;
        B = t1 * p0 + t2 * p1 + t3 * p2 + t4 * p3;
        return B;
    }

    /// <summary>
    /// n阶贝塞尔曲线
    /// </summary>
    public static FixIntVector3 BezierCurve(List<FixIntVector3> pointList, FixInt t)
    {
        FixIntVector3 B = FixIntVector3.zero;
        if (pointList == null)
        {
            return B;
        }
        if (pointList.Count < 2)
        {
            return pointList[0];
        }

        List<FixIntVector3> tempPointList = new List<FixIntVector3>();
        for (int i = 0; i < pointList.Count - 1; i++)
        {
            FixIntVector3 tempPoint = BezierCurve(pointList[i], pointList[i + 1], t);
            tempPointList.Add(tempPoint);
        }
        return BezierCurve(tempPointList, t);
    }
}