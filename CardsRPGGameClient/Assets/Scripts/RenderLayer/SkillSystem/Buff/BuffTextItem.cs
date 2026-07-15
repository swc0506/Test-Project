using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuffTextItem : MonoBehaviour
{
    public Text text;
    public CanvasGroup canvasGroup;

    public void PlayBuffDamageAnim(BuffConfig buffConfig)
    {
        text.text = buffConfig.buffName + "伤害";
        float endY = transform.localPosition.y + 100;
        transform.DOLocalMoveY(endY, 1f);
        canvasGroup.DOFade(0,0.5f).SetDelay(1.2f).OnComplete(() => { Destroy(gameObject); });
    }
}
