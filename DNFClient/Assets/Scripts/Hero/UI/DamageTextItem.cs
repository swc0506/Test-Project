using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class DamageTextItem : MonoBehaviour
{
    public Text text;
    void Start()
    {
        //ShowDamageText(9999);
    }
  
    public void ShowDamageText(int damageValue,RenderObject target)
    {
        //BattleWindow window = UIModule.Instance.GetWindow<BattleWindow>();
        //transform.SetParent(window.transform);
        transform.localScale = Vector3.one;
        //transform.position = PosConvertUtility.World3DPosToCanvasWorldPos(target.transform.position, window.transform as RectTransform, UIModule.Instance.mUICamera);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y+20,0);
        transform.localScale = Vector3.one * 2f;
        transform.DOScale(1, 0.3f).OnComplete(() =>
         {
            
         });
        text.DOFade(0,0.3f). SetDelay(0.2f);
        transform.DOMoveY(transform.position.y + 1, 0.2f).OnComplete(() =>
        {
            GameObject.Destroy(gameObject);
        }).SetDelay(0.2f);
        text.text = damageValue.ToString();
    }
}
