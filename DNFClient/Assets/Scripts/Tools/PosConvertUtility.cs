
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

/// <summary>
/// 屏幕和3D坐标及UGUI坐标的转换
/// </summary>
public class PosConvertUtility
{
    public static Vector3 Get3dConterPos()
    {
        return new Vector3(Camera.main.transform.position.x, 0, 0);
    }
    public static Vector3 GetMapConterPos()
    {
        return new Vector3(Camera.main.transform.position.x, 0, 4);
    }
    public static Vector3 ScreenPosToWorld3DPos(Vector2 vector2)
    {
        return Camera.main.ScreenToWorldPoint(vector2);
    }
    /// <summary>
    /// 世界坐标转为屏幕坐标
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static Vector3 WorldToScreenPos(Vector3 worldPos)
    {
        return Camera.main.WorldToScreenPoint(worldPos);
    }
    /// <summary>
    /// 世界坐标转为Canvas世界坐标
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static Vector3 World3DPosToCanvasWorldPos(Vector3 worldPos, RectTransform canvasRect, Camera uiCamera)
    {
        Vector3 pos;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPos, uiCamera, out pos);
        return pos;
    }

    /// <summary>
    /// UI转世界坐标
    /// </summary>
    public static Vector3 UIToWorldPos(Vector3 uiPos, Camera uiCamera)
    {
        uiPos = uiCamera.WorldToScreenPoint(uiPos);
        uiPos.z = 0f;
        uiPos = Camera.main.ScreenToWorldPoint(uiPos);
        return uiPos;
    }
    /// <summary>
    /// 屏幕坐标转为UI世界坐标
    /// </summary>
    /// <param name="ScreenPos"></param>
    /// <param name="outPos"></param>
    /// <returns></returns>
    public static Vector3 ScreenPosToUIWorldPos(Vector2 ScreenPos, RectTransform canvasRect, Camera uiCamera)
    {
        Vector3 outPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, ScreenPos, uiCamera, out outPos);
        return outPos;
    }
    /// <summary>
    /// Canvas UI坐标转MainCamear世界坐标
    /// </summary>
    /// <param name="uiPos">UI坐标</param>
    public static Vector3 CanvasPosToMainCamerWorldPos(Vector3 uiPos, Canvas canvas)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, canvas.worldCamera.WorldToScreenPoint(uiPos), Camera.main, out worldPoint);
        return worldPoint;
    }
    /// <summary>
    /// Canvas UI坐标转为主3D摄像机的屏幕坐标
    /// </summary>
    /// <param name="uiPos">UI坐标</param>
    /// <returns></returns>
    public static Vector3 CanvasPosToScreenPos(Vector3 uiPos, Canvas canvas)
    {
        Vector3 ScreenPos = canvas.worldCamera.WorldToScreenPoint(uiPos);
        ScreenPos.z = 0;
        return ScreenPos;
    }


    /// <summary> 
    /// 从鼠标发射一条射线  
    /// </summary>
    public static void MousePosToRay(Action<RaycastHit> hitCallBack)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
        ScreenToRay(pos, hitCallBack);
    }
    /// <summary>
    /// 从屏幕位置发射一条射线
    /// </summary>
    /// <param name="ScreenPos">屏幕位置</param>
    public static void ScreenToRay(Vector3 ScreenPos, Action<RaycastHit> HitCallBack)
    {

        Vector2 uipos = Camera.main.WorldToViewportPoint(ScreenPos);
        Ray ray = Camera.main.ViewportPointToRay(uipos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
            HitCallBack?.Invoke(hitInfo);
            return;
        }
    }
}