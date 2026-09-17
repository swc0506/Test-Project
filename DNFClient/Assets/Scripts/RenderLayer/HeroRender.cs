using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixMath;
public class HeroRender : RenderObject
{
    private HeroLogic mHeroLogic;
    /// <summary>
    /// 角色动画管理器
    /// </summary>
    private Animation mAnim;
    /// <summary>
    /// 当前摇杆输入方向
    /// </summary>
    public Vector3 mInputDir;
    /// <summary>
    /// 左手节点
    /// </summary>
    public Transform Left_Hand_RootTrans;
    /// <summary>
    /// 右手节点
    /// </summary>
    public Transform Right_Hand_RootTrans;
    /// <summary>
    /// 技能引导特效对象
    /// </summary>
    private GameObject mSkillGuideEffectObj;

    public override void OnCreate()
    {
        base.OnCreate();
        mHeroLogic = logicObject as HeroLogic;
        JoystickUGUI.OnMoveCallBack += OnJoyStickMove;
        mAnim = transform.GetComponent<Animation>();
    }

    public override void OnRelease()
    {
        base.OnRelease();
        JoystickUGUI.OnMoveCallBack -= OnJoyStickMove;
    }
    /// <summary>
    /// 摇杆移动输入
    /// </summary>
    public void OnJoyStickMove(Vector3 inputDir)
    {
        mInputDir = inputDir;
        //逻辑方向
        FixIntVector3 logicDir = FixIntVector3.zero;

        if (inputDir != Vector3.zero)
        {
            logicDir.x = inputDir.x;
            logicDir.y = inputDir.y;
            logicDir.z = inputDir.z;
        }
        //向英雄逻辑层直接输入操作帧事件 没有服务端情况下的测试代码
        mHeroLogic.InputLoigcFrameEvent(logicDir);
    }

    public override void Update()
    {
        base.Update();
        //判断有没有技能在释放中，如果有技能在释放中，是不允许播放移动和待机动画的
        if (mHeroLogic.releaseingSkillList.Count == 0)
        {
            //判断摇杆输入是否有值输入，如果没有有，播放待机动画，如果有值，播放移动动画
            if (mInputDir.x == 0 && mInputDir.z == 0)
            {
                PlayAnim("Anim_Idle02");
            }
            else
            {
                PlayAnim("Anim_Run");
            }
        }

    }
    /// <summary>
    /// 播放角色动画
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnim(string animName)
    {
        mAnim.CrossFade(animName, 0.2f);
    }
    /// <summary>
    /// 通过动画文件播放动画
    /// </summary>
    /// <param name="clip"></param>
    public override void PlayAnim(AnimationClip clip)
    {
        base.PlayAnim(clip);

        if (mAnim.GetClip(clip.name) == null)
        {
            mAnim.AddClip(clip, clip.name);
        }
        mAnim.clip = clip;
        PlayAnim(clip.name);
    }
    /// <summary>
    /// 获取父节点
    /// </summary>
    /// <param name="parentType"></param>
    /// <returns></returns>
    public override Transform GetTransParent(TransParentType parentType)
    {
        if (parentType == TransParentType.LeftHand)
        {
            return Left_Hand_RootTrans;
        }
        else if (parentType == TransParentType.RightHand)
        {
            return Right_Hand_RootTrans;
        }
        return null;
    }


    public void InitSkillGuide(int skillid)
    {
        if (mSkillGuideEffectObj == null)
        {
            Skill skill = mHeroLogic.GetSKill(skillid);
            mSkillGuideEffectObj = GameObject.Instantiate(skill.SKillCfg.skillGuideObj);
            mSkillGuideEffectObj.transform.localScale = Vector3.one;
        }
    }
    /// <summary>
    /// 更新技能引导
    /// </summary>
    /// <param name="sKillGuideType">技能引导类型</param>
    /// <param name="skillid">技能id</param>
    /// <param name="isPreass">手指是否按下</param>
    /// <param name="pos">摇杆的位置</param>
    /// <param name="skillRange">技能引导范围</param>
    public void UpdateSkillGuide(SKillGuideType sKillGuideType, int skillid, bool isPreass, Vector3 pos, float skillRange)
    {
        //初始化引导特效
        InitSkillGuide(skillid);
        //更新引导特效位置
        switch (sKillGuideType)
        {
            case SKillGuideType.Click:
                break;
            case SKillGuideType.LongPress:
                break;
            case SKillGuideType.Position:
                Vector3 skillGuidePos= transform.position + pos;
                //限制当前位置的z轴 不能超过地图
                skillGuidePos = new Vector3(skillGuidePos.x,0,Mathf.Clamp(skillGuidePos.z,-1,8.6f));
                mSkillGuideEffectObj.transform.localPosition = skillGuidePos;
                break;
            case SKillGuideType.Dirction:
                break;
        }
    }
    public void OnGuideRelease()
    {
        if (mSkillGuideEffectObj!=null)
        {
            GameObject.Destroy(mSkillGuideEffectObj);
        }
    }
}
