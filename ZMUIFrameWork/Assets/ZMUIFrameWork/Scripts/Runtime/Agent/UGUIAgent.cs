using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UGUIAgent
{
    public static void SetVisible(this GameObject obj, bool visible)
    {
        obj.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

    public static void SetVisible(this Transform trans, bool visible)
    {
        trans.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this Button btn, bool visible)
    {
        btn.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this Slider slider, bool visible)
    {
        slider.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this Text text, bool visible)
    {
        text.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this Toggle toggle, bool visible)
    {
        toggle.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this InputField field, bool visible)
    {
        field.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this RawImage rawImage, bool visible)
    {
        rawImage.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisible(this ScrollRect rect, bool visible)
    {
        rect.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
}
