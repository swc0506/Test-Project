/*----------------------------------------------------------------------------
* Title: 帧同步定点数
*
* Author: 铸梦
*
* Date: 2024.12.29
*
* Description:可应用于状态同步或帧同步中，主要解决不同平台下float精度误差问题，保证不同平台在相同属于源的情况下，结果计算的一致性
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/

using System;
using UnityEngine;
using ZM.FixIntMath;

public class FixIntDemo : MonoBehaviour
{
    // 在第一帧更新之前调用 Start
    void Start()
    {

        #region Abs 绝对值
        //Debug.Log($"--------------------------Abs--------------------------------");
        //Debug.Log("Unity Abs -888:  " + Math.Abs(-888));
        //Debug.Log("FixIntMath Abs -888:    " + FixIntMath.Abs(-888));
        //Debug.Log("Unity Abs -6.66f:    " + Math.Abs(-6.66f));
        //Debug.Log("FixIntMath Abs -6.66f   :" + FixIntMath.Abs(-6.66f));
        //Debug.Log($"--------------------------Abs End--------------------------------");
        #endregion

        #region Min 最小取值
        //Debug.Log($"--------------------------Min--------------------------------");
        //Debug.Log("Unity Abs -8.88:  " + Math.Min(-8.88, 1));
        //Debug.Log("FixIntMath Abs -8.88:    " + FixIntMath.Min(-8.88, 1));
        //Debug.Log("Unity Abs 1000:    " + Math.Min(1000, 0));
        //Debug.Log("FixIntMath Abs 1000   :" + FixIntMath.Min(1000, 0));
        //Debug.Log($"--------------------------Min End--------------------------------");
        #endregion

        #region Max 最大取值
        //Debug.Log($"--------------------------Min--------------------------------");
        //Debug.Log("Unity Abs -8.88:  " + Math.Max(-8.88, 1));
        //Debug.Log("FixIntMath Abs -8.88:    " + FixIntMath.Max(-8.88, 1));
        //Debug.Log("Unity Abs 1000:    " + Math.Max(1000, 0));
        //Debug.Log("FixIntMath Abs 1000   :" + FixIntMath.Max(1000, 0));
        //Debug.Log($"--------------------------Min End--------------------------------");
        #endregion

        #region Range 随机数
        //Debug.Log($"--------------------------Range--------------------------------");
        //Debug.Log("Unity Range 1-11:  " + UnityEngine.Random.Range(1, 11));
        //Debug.Log("FixIntMath Range 1-11:    " + FixIntMath.Range(new System.Random(), 1, 11));
        //Debug.Log("Unity Range 0-1000:    " + UnityEngine.Random.Range(0, 1000));
        //Debug.Log("FixIntMath Range 0-1000   :" + FixIntMath.Range(new System.Random(), 0, 1000));
        //Debug.Log($"--------------------------Range End--------------------------------");
        #endregion

        #region Clamp 取值范围
        //Debug.Log($"--------------------------Clamp--------------------------------");
        //Debug.Log("Unity Math  3.6, 0, 10:  " + Math.Clamp(3.6, 0, 10));
        //Debug.Log("FixIntMath  3.6, 0, 10:    " + FixIntMath.Clamp(3.6, 0, 10));
        //Debug.Log("Unity Math -3.6, 0, 10:    " + Math.Clamp(-3.6, 0, 10));
        //Debug.Log("FixIntMath -3.6, 0, 10   :" + FixIntMath.Clamp(-3.6, 0, 10));
        //Debug.Log($"--------------------------Clamp End--------------------------------");
        #endregion

        #region Floor 向下取整
        //Debug.Log($"--------------------------Floor--------------------------------");
        //Debug.Log("Unity Math  3.4:  " + Mathf.Floor(4.01f));
        //Debug.Log("FixIntMath  3.4:    " + FixIntMath.Floor(4.01));
        //Debug.Log("Unity Math  3.99:    " + Math.Floor(3.99));
        //Debug.Log("FixIntMath  3.99   :" + FixIntMath.Floor(3.99));
        //Debug.Log($"--------------------------Floor End--------------------------------");
        #endregion

        #region Ceiling 向上取整
        //Debug.Log($"--------------------------Ceiling--------------------------------");
        //Debug.Log("Unity Math  3.1:  " + Math.Ceiling(4.01));
        //Debug.Log("FixIntMath  3.1:    " + FixIntMath.Ceiling(4.01));
        //Debug.Log("Unity Math  3.99:    " + Math.Ceiling(3.99));
        //Debug.Log("FixIntMath  3.99   :" + FixIntMath.Ceiling(3.99));
        //Debug.Log($"--------------------------Ceiling End--------------------------------");
        #endregion

        #region Sqrt 平方根
        //Debug.Log($"--------------------------Sqrt--------------------------------");
        //Debug.Log("Unity Math  8:  " + Math.Sqrt(8));
        //Debug.Log("FixIntMath  8:    " + FixIntMath.Sqrt(8));
        //Debug.Log("Unity Math  1024:    " + Math.Sqrt(1024));
        //Debug.Log("FixIntMath  1024:   :" + FixIntMath.Sqrt(1024));
        //Debug.Log($"--------------------------Sqrt End--------------------------------");
        #endregion

        #region Pow 幂运算
        //Debug.Log($"--------------------------Pow--------------------------------");
        //Debug.Log("Unity Math  2,2:  " + Math.Pow(2, 2));
        //Debug.Log("FixIntMath  2,2:    " + FixIntMath.Pow(2, 2));
        //Debug.Log("Unity Math  2,10:    " + Math.Pow(2, 10));
        //Debug.Log("FixIntMath  2,10:   :" + FixIntMath.Pow(2, 10));
        //Debug.Log($"--------------------------Pow End--------------------------------");
        #endregion

        #region Pow 幂运算
        //Debug.Log($"--------------------------Pow--------------------------------");
        //Debug.Log("Unity Math  2,2:  " + Math.Pow(2, 2));
        //Debug.Log("FixIntMath  2,2:    " + FixIntMath.Pow(2, 2));
        //Debug.Log("Unity Math  2,10:    " + Math.Pow(2, 10));
        //Debug.Log("FixIntMath  2,10:   :" + FixIntMath.Pow(2, 10));
        //Debug.Log($"--------------------------Pow End--------------------------------");
        #endregion

        #region Cos 余弦函数
        //Debug.Log($"--------------------------Cos--------------------------------");
        //Debug.Log("Unity Math 0:  " + Math.Cos(0));
        //Debug.Log("FixIntMath 0:    " + FixIntMath.Cos(0));
        //Debug.Log("Unity Math 0.5:    " + Math.Cos(0.5f));
        //Debug.Log("FixIntMath 0.5   :" + FixIntMath.Cos(0.5f));
        //Debug.Log($"--------------------------Cos End--------------------------------");
        #endregion

        #region Acos 反余弦函数
        //Debug.Log($"-------------------------Acos---------------------------------");
        //Debug.Log("Unity Math 0.2:    " + Math.Acos(0.2f));
        //Debug.Log("FixIntMath 0.2   :" + FixIntMath.Acos(0.2f));

        //Debug.Log("Unity Math 0.5:    " + Math.Acos(0.5f));
        //Debug.Log("FixIntMath 0.5:  " + FixIntMath.Acos(0.5f));
        //Debug.Log($"--------------------------Acos End--------------------------------");
        #endregion

        #region Sin 正弦函数
        //Debug.Log($"--------------------------Sin--------------------------------");
        //Debug.Log("Unity Math 1:  " + Math.Sin(1));
        //Debug.Log("FixIntMath 1:    " + FixIntMath.Sin(1));

        //Debug.Log("Unity Math 0.5:    " + Math.Sin(0.5f));
        //Debug.Log("FixIntMath 0.5   :" + FixIntMath.Sin(0.5f));
        //Debug.Log($"--------------------------Sin End--------------------------------");
        #endregion

        #region Atan2 四象限反正切函数
        //Debug.Log($"--------------------------Atan2--------------------------------");
        //Debug.Log("Unity Atan2 :  " + Math.Atan2(2, 1));

        //Debug.Log("Fixint Atan2:  " + FixIntMath.Atan2(2, 1));
        //Debug.Log($"--------------------------Atan2 End--------------------------------");
        #endregion
    }


}
