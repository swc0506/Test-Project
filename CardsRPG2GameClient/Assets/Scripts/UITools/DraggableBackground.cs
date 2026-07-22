/*----------------------------------------------------------------------------
* Title: 帧同步定点数学库
*
* Author: 铸梦
*
* Date: 2025.02.20
*
* Description:基于定点数实现的一套AABB定点数学物理碰撞库，可用于客户端和服务端。
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 案例地址：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBackground : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private float minX, maxX;
    private float canvasWidth;
    public RectTransform targetTrans;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        
        // 计算边界
        Canvas canvas = GetComponentInParent<Canvas>();
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        
        // 背景图宽度的一半减去屏幕宽度的一半
        float backgroundHalfWidth = rectTransform.rect.width / 2;
        float screenHalfWidth = canvasWidth / 2;
        
        minX = originalPosition.x - (backgroundHalfWidth - screenHalfWidth);
        maxX = originalPosition.x + (backgroundHalfWidth - screenHalfWidth);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = rectTransform.anchoredPosition + new Vector2(eventData.delta.x, 0);
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        targetTrans.anchoredPosition = rectTransform.anchoredPosition = newPos;
    }
}