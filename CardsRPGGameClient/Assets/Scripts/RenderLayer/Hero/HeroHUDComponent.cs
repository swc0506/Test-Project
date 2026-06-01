using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeroHUDComponent : MonoBehaviour
{
    private HeroRender heroRender;
    public Slider hpSlider;
    public Slider animSlider;
    public Slider angerSlider;
    public Transform buffParent;

    public void Init(HeroRender render)
    {
        this.heroRender = render;
    }

    public void UpdateHpSlider(float value)
    {
        hpSlider.value = value;
        animSlider.DOValue(value, 0.5f).SetDelay(0.4f);
        if (value <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
