using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    public Transform skillAnimTrans;
    public Text skillNameText;
    public Image heroIconImage;

    public void PlayAnim(SkillConfig skill, int heroId)
    {
        skillNameText.text = skill.skillName;
        heroIconImage.sprite = ResourcesManager.Instance.LoadAsset<Sprite>("Texture/" + heroId);
        skillAnimTrans.localScale = Vector3.one;
        skillAnimTrans.transform.localPosition = new Vector3(340, 0, 0);

        skillAnimTrans.DOLocalMoveX(0, 0.1f).OnComplete(() =>
        {
            skillAnimTrans.DOLocalMoveY(10, 0.5f).SetLoops(-1, LoopType.Yoyo);
        });
        
        skillAnimTrans.DOLocalMoveX(340, 0.1f).SetDelay(1.5f);
    }
}
