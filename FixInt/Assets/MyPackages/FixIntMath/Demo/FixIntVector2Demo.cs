/*----------------------------------------------------------------------------
* Title: #Title#
*
* Author: 铸梦
*
* Date: #CreateTime#
*
* Description:
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/
using System;
using UnityEngine;

public class FixIntVector2Demo : MonoBehaviour
{
    // 在第一帧更新之前调用 Start
    void Start()
    {

        #region 加减乘除运算
        //Debug.Log($"--------------------------相加运算--------------------------------");
        //Vector2 uv2 = new Vector2(0.5f, 0.5f) + new Vector2(1, 1);
        //Debug.Log($"Unity vector2:{uv2}");

        //FixIntVector2 fv2 = new FixIntVector2(0.5, 0.5) + new FixIntVector2(1, 1);
        //Debug.Log($"Fixint vector2:{fv2}");
        //Debug.Log($"--------------------------相加运算 End--------------------------------");

        //Debug.Log($"--------------------------相减运算--------------------------------");
        //uv2 = new Vector2(0.5f, 0.5f) - new Vector2(1, 1);
        //Debug.Log($"Unity vector2:{uv2}");

        //fv2 = new FixIntVector2(0.5, 0.5) - new FixIntVector2(1, 1);
        //Debug.Log($"Fixint vector2:{fv2}");
        //Debug.Log($"--------------------------相减运算 End--------------------------------");

        //Debug.Log($"--------------------------相乘运算--------------------------------");
        //uv2 = new Vector2(0.5f, 0.5f) * new Vector2(2, 2);
        //Debug.Log($"Unity vector2:{uv2}");

        //fv2 = new FixIntVector2(0.5, 0.5) * new FixIntVector2(2, 2);
        //Debug.Log($"Fixint vector2:{fv2}");
        //Debug.Log($"--------------------------相乘运算 End--------------------------------");

        //Debug.Log($"--------------------------相除运算--------------------------------");
        //uv2 = new Vector2(0.5f, 0.5f) / new Vector2(2, 2);
        //Debug.Log($"Unity vector2:{uv2}");

        //fv2 = new FixIntVector2(0.5, 0.5) / new FixIntVector2(2, 2);
        //Debug.Log($"Fixint vector2:{fv2}");
        //Debug.Log($"--------------------------相除运算 End--------------------------------");
        #endregion

        #region 角度获取运算 Angle
        //Debug.Log($"--------------------------角度获取运算 Angle--------------------------------");
        //float angleu = Vector2.Angle(new Vector2(0, 1), new Vector2(1, 0));
        //Debug.Log($"Unity angleu:{angleu}");

        //FixInt anglef = FixIntVector2.Angle(new FixIntVector2(0, 1), new FixIntVector2(1, 0));
        //Debug.Log($"FixintVector2 anglef:{anglef.RawInt}");
        //Debug.Log($"--------------------------角度获取运算 Angle End--------------------------------");
        #endregion

        #region 距离获取运算 Distance
        //Debug.Log($"--------------------------距离获取运算 Distance--------------------------------");
        //float distanceUv2 = Vector2.Distance(new Vector2(0, 1), new Vector2(1, 0));
        //Debug.Log($"Unity Vector2  distanceUv2: {distanceUv2}");

        //FixInt distanceFV2 = FixIntVector2.Distance(new FixIntVector2(0, 1), new FixIntVector2(1, 0));
        //Debug.Log($"Fixint Vector2 distanceFV2: {distanceFV2}");
        //Debug.Log($"--------------------------距离获取运算 Distance End--------------------------------");
        #endregion

        #region 点乘运算 Dot
        //Debug.Log($"-------------------------- 点乘运算 Dot --------------------------------");
        //float dotUv2 = Vector2.Dot(new Vector2(1.5f, 1), new Vector2(1.5f, 1));
        //Debug.Log($"Unity Vector2  dotUv2: {dotUv2}");

        //FixInt dotFv2 = FixIntVector2.Dot(new FixIntVector2(1.5f, 1), new FixIntVector2(1.5f, 1));
        //Debug.Log($"Fixint Vector2 dotFv2: {dotFv2}");
        //Debug.Log($"--------------------------点乘运算 Dot End--------------------------------");
        #endregion

        #region  插值运算 Lerp
        //Debug.Log($"-------------------------- 插值运算 Lerp --------------------------------");
        //Vector2 lerpUv2 = Vector2.Lerp(new Vector2(0, 0), new Vector2(15, 15), 0.5f);
        //Debug.Log($"Unity Vector2  lerpUv2: {lerpUv2}");

        //FixIntVector2 lerpFv2 = FixIntVector2.Lerp(new FixIntVector2(0, 0), new FixIntVector2(15, 15), 0.5f);
        //Debug.Log($"Fixint Vector2 lerpFv2: {lerpFv2}");
        //Debug.Log($"--------------------------插值运算 Lerp End--------------------------------");
        #endregion

    }

    #region  平移运算 MoveTowards
    //private float mAccRuntime;
    //private float mMoveSpeed = 2;
    //private void Update()
    //{
    //    mAccRuntime += Time.deltaTime;
    //    Vector2 lerpUv2 = Vector2.MoveTowards(transform.position, new Vector2(1000, 1000), mAccRuntime * mMoveSpeed);
    //    Debug.Log($"Unity Vector2  MoveTowards: {lerpUv2}");

    //    FixIntVector2 lerpFv2 = FixIntVector2.MoveTowards(transform.position, new FixIntVector2(1000, 1000), mAccRuntime * mMoveSpeed);
    //    Debug.Log($"Fixint Vector2 MoveTowards: {lerpFv2}");
    //    transform.position = lerpUv2;
    //}
    #endregion

    #region  插值函数 SmoothDamp 
    //private float mMoveSpeed = 1;

    //Vector2 targetPositionUv2 = new Vector2(1000, 1000);
    //Vector2 velocityUv2;

    //FixIntVector2 targetPositionFv2 = new FixIntVector2(1000, 1000);
    //FixIntVector2 velocityFv2;
    //private void Update()
    //{
    //    float timeDeltaTime = Time.deltaTime;

    //    Vector2 lerpUv2 = Vector2.SmoothDamp(transform.position, targetPositionUv2, ref velocityUv2, timeDeltaTime, 99999, timeDeltaTime);
    //    Debug.Log($"Unity Vector2  SmoothDamp: {lerpUv2}");

    //    FixIntVector2 lerpFv2 = FixIntVector2.SmoothDamp(transform.position, targetPositionFv2, ref velocityFv2, timeDeltaTime, timeDeltaTime);
    //    Debug.Log($"Fixint Vector2 SmoothDamp: {lerpFv2}");
    //    //transform.position = lerpUv2;
    //    transform.position = lerpFv2.ToVector2();
    //}
    #endregion
}
