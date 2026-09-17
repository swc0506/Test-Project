using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class UIEventListener : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Action<PointerEventData> OnDrag;
    public Action<PointerEventData> OnPress;
    public Action<PointerEventData> OnUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("  OnPointerDown:");
        OnPress?.Invoke(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp:");
        OnUp?.Invoke(eventData);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        OnDrag?.Invoke(eventData);
    }
}
