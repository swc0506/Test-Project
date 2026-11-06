using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlackScreenType
{
    HideAndShow,
    Hide,
    Show,
}

//黑屏，用一张全黑图片覆盖屏幕，调整透明度使用curve。
public class BlackScreen : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;//覆盖屏幕的一张全黑图片，我选择挂在Camera下面，这样做如果相机是移动的就很方便
    [Header("渐入渐现曲线")]
    public AnimationCurve curveFadeOut; //在Inspector上调整自己喜欢的曲线
    [Header("渐隐曲线")]
    public AnimationCurve curveHide;
    [Header("渐现曲线")]
    public AnimationCurve curveShow;
    [Range(0.5f, 2f)] public float speed = 1f; //控制渐入渐出的速度

    private BlackScreenType mBlackScreenType;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        UIEventControl.AddEvent(UIEventEnum.BlackScreen, OnBlackScreenEvent);
    }
    /// <summary>
    /// 黑屏事件
    /// </summary>
    /// <param name="data"></param>
    private void OnBlackScreenEvent(object data)
    {
      
        mBlackScreenType = (BlackScreenType)data;
        spriteRenderer.gameObject.gameObject.SetActive(true);
        switch (mBlackScreenType)
        {
            case BlackScreenType.HideAndShow:
                StartCoroutine(Black(curveFadeOut));
                break;
            case BlackScreenType.Hide:
                StartCoroutine(Black(curveHide));
                break;
            case BlackScreenType.Show:
              
                StartCoroutine(Black(curveShow));
                break;
        }
        spriteRenderer.gameObject.SetActive(true);
    }
    //开启自动播放黑屏
    private void OnEnable()
    {
        //StartCoroutine(Black(curveFadeOut));
    }

    Color tmpColor; //用于传递颜色的变量
    /// <summary>
    /// 渐入渐现
    /// </summary>
    /// <returns></returns>
    public IEnumerator Black(AnimationCurve curve)
    {
        float timer = 0f;
        tmpColor = spriteRenderer.color;
        do
        {
            timer += Time.deltaTime;
            SetColorAlpha(curve.Evaluate(timer * speed));
            yield return null;

        }
        while ((mBlackScreenType == BlackScreenType.Hide && tmpColor.a > 0.01f) || (mBlackScreenType == BlackScreenType.Show &&  tmpColor.a < 1)
        || (mBlackScreenType == BlackScreenType.HideAndShow &&  tmpColor.a > 0.01f));
        

        if (mBlackScreenType == BlackScreenType.Hide|| mBlackScreenType== BlackScreenType.HideAndShow)
        {
            spriteRenderer.gameObject.SetActive(false);
        }
     
    }

    //通过调整图片的透明度实现渐入渐出
    void SetColorAlpha(float a)
    {
        tmpColor.a = a;
        spriteRenderer.color = tmpColor;
    }
    public void OnDestroy()
    {
        UIEventControl.RemoveEvent(UIEventEnum.BlackScreen, OnBlackScreenEvent);
    }
}