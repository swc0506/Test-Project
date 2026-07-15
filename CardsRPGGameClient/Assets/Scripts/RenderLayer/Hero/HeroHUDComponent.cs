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
    public List<BuffIconItem> buffIconItems = new List<BuffIconItem>();

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

    public void UpdateAngerSlider(float value)
    {
        angerSlider.value = value;
        angerSlider.gameObject.SetActive(value != 0);
    }
    
    public void AddBuffIcon(BuffConfig buffConfig)
    {
        BuffIconItem buffIconItem = ResourcesManager.Instance.LoadObject<BuffIconItem>(AssetPathConfig.HUD + "BuffIconItem", buffParent);
        buffIconItem.buffIcon.sprite = buffConfig.buffIcon;
        buffIconItems.Add(buffIconItem);
    }
    
    public void RemoveBuffIcon(Sprite sprite)
    {
        for (int i = 0; i < buffIconItems.Count; i++)
        {
            if (buffIconItems[i].buffIcon.sprite == sprite)
            {
                Destroy(buffIconItems[i].gameObject);
                buffIconItems.Remove(buffIconItems[i]);
                break;
            }
        }
    }

    public void Release()
    {
        for (int i = 0; i < buffIconItems.Count; i++)
        {
            Destroy(buffIconItems[i].gameObject);
        }
        buffIconItems.Clear();
        
        if (gameObject != null)
            Destroy(gameObject);
    }
}
