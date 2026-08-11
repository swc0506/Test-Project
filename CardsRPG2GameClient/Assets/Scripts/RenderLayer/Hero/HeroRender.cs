using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LogicLayer;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using ZM.UI;
using ZM.ZMAsset;

public enum HeroAniState
{
    Attack,
    Hurt,
    Injured,
    Run,
    Skill_01,
    Skill_02,
    Stand,
}

public class HeroRender : RenderObject
{
    public HeroData HeroData { get; private set; }
    public HeroTeamEnum TeamEnum { get; private set; }

    private SkeletonAnimation mAnimator;
    private HUDWindow hudWindow;
    private HeroHUDComponent mHUDComp;
    private Transform hudParent;
    private float mLastPlayAnimTime;
    private Vector2 hudOffset = new Vector2(0, 260f);

    public void SetHeroData(HeroData data, HeroTeamEnum teamEnum)
    {
        HeroData = data;
        TeamEnum = teamEnum;
        Initialize();
    }

    private void Initialize()
    {
        mAnimator = transform.GetComponent<SkeletonAnimation>();
        hudWindow = UIModule.Instance.GetWindow<HUDWindow>();
        hudParent = hudWindow.UIContent;
        mHUDComp =
            ZMAsset.InstantiateObject(
                AssetPathConfig.HUD + "HPObject" + TeamEnum.ToString(), hudParent).GetComponent<HeroHUDComponent>();
        mHUDComp.Init(this);
    }

    public override void Update()
    {
        base.Update();
        UpdateHeroHUD();
    }

    private void UpdateHeroHUD()
    {
        if (mHUDComp != null && LogicObj != null && hudParent != null)
        {
            mHUDComp.transform.localPosition = World3DToCanvasPos(transform.position) + hudOffset;
        }
    }

    public void PlayAnim(string animName)
    {
        //mAnimator.SetTrigger(animName);
        mAnimator.AnimationState.SetAnimation(0, animName, false).Complete += (trackEntry) =>
        {
            if (LogicObj.objectState == LogicObjectState.Survival)
            {
                mAnimator.AnimationState.SetAnimation(0, nameof(HeroAniState.Stand), true);
            }
        };
    }

    public void SetAnimState(AnimState state)
    {
        //mAnimator.speed = state == AnimState.StopAnim ? 0 : 1;
        mAnimator.timeScale = state == AnimState.StopAnim ? 0 : 1;
    }

    /// <summary>
    /// 更新HP
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hpPercent"></param>
    /// <param name="buffConfig"></param>
    public void UpdateHp_HUD(int damage, float hpPercent, BuffConfig buffConfig = null)
    {
        Vector2 pos = World3DToCanvasPos(transform.position);
        if (damage != 0)
        {
            GameObject damageText = ResourcesManager.Instance.LoadObject(
                AssetPathConfig.HUD + (damage > 0 ? "DamageText" : "RestoreHPText"), hudParent, restScale: true);
            damageText.transform.localPosition = new Vector2(pos.x, pos.y + 40);

            damageText.GetComponent<Text>().text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
            damageText.transform.DOLocalMoveY(damageText.transform.localPosition.y + 100, 1f);
            damageText.GetComponent<CanvasGroup>().DOFade(0, .05f).SetDelay(1.2f);
            Destroy(damageText, 3f);

            mHUDComp.UpdateHpSlider(hpPercent);
        }

        if (buffConfig != null)
        {
            BuffTextItem textItem =
                ResourcesManager.Instance.LoadObject<BuffTextItem>(AssetPathConfig.HUD + "DeBuffItemText", hudParent);
            textItem.transform.localPosition = new Vector3(pos.x, pos.y);
            textItem.transform.localScale = Vector3.one;
            if (mLastPlayAnimTime == 0 || Time.realtimeSinceStartup - mLastPlayAnimTime > 0.2f)
            {
                textItem.PlayBuffDamageAnim(buffConfig);
            }
            else
            {
                LogicTimerManager.Instance.DelayCall(300, () => { textItem.PlayBuffDamageAnim(buffConfig); });
            }

            mLastPlayAnimTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// 更新怒气值
    /// </summary>
    /// <param name="rate"></param>
    public void UpdateAnger_HUD(float rate)
    {
        if (mHUDComp != null)
            mHUDComp.UpdateAngerSlider(rate);
    }

    public void AddBuffIcon(BuffConfig buffConfig)
    {
        if (mHUDComp != null)
            mHUDComp.AddBuffIcon(buffConfig);
    }

    public void RemoveBuffIcon(Sprite sprite)
    {
        if (mHUDComp != null)
            mHUDComp.RemoveBuffIcon(sprite);
    }

    /// <summary>
    /// 世界3d坐标转化位UGUI本地坐标
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private Vector2 World3DToCanvasPos(Vector3 targetPos)
    {
        Vector3 screenPos =
            RectTransformUtility.WorldToScreenPoint(BattleWorldManager.BattleWorld.Root3D.battleCamera, targetPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(hudWindow.transform as RectTransform,
            screenPos,
            UIModule.Instance.Camera, out var uGuiLocalPos);
        return uGuiLocalPos;
    }

    public void HeroDeath()
    {
        PlayAnim(nameof(HeroAniState.Hurt));
        mHUDComp.gameObject.SetActive(false);
    }

    public override void OnRelease()
    {
        // 不能用?.运算符，因为它不走Unity重载的==判断，无法识别已被Unity销毁的对象
        if (mHUDComp != null)
        {
            mHUDComp.Release();
            mHUDComp = null;
        }

        base.OnRelease();
    }
}