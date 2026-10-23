using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

public class MonsterRedner : RenderObject
{
    private Animation mAnim;
    private string mCurAnimName;
    private int mMonsterid;
    private MonsterLogic mMonsterLogic;

    public override void OnCreate()
    {
        base.OnCreate();
        mAnim = transform.GetComponentInChildren<Animation>();
        mMonsterLogic = (logicObject as MonsterLogic);
        mMonsterid = mMonsterLogic.MonsterId;
    }

    public override void PlayAnim(string name)
    {
        base.PlayAnim(name);
        if (mAnim == null)
        {
            return;
        }

        //怪物死亡时 只能播放死亡动画
        if (logicObject.ObjectState == LogicObjectState.Death && !string.Equals(name, "Anim_Dead"))
        {
            return;
        }

        mCurAnimName = name;
        mAnim.Play(name);
    }
    
    public override string GetCurAnimName()
    {
        return mCurAnimName;
    }

    public override void OnRelease()
    {
        base.OnRelease();
    }

    public override void OnHit(GameObject effectHitObj, int survivalTimems, LogicObject source)
    {
        base.OnHit(effectHitObj, survivalTimems, source);
        
        //通过怪物配置文件，配置怪物的信息，如怪物的id、基础血量、攻击力、移动速度、受击音效、攻击音效等 Excel
        //加载怪物的时候读取配置，播放音效也是读配置的。
        //临时代码
        AudioClip audioClip = null;
        if (mMonsterid == 20001)//哥布林
        {
            audioClip = ZMAssetsFrame.LoadAudio(AssetPathConfig.GAME_AUIDO_PATH + "Gebulin/GoblinAttackC.wav");
        }
        else if (mMonsterid == 20005)//蜘蛛
        {
            audioClip = ZMAssetsFrame.LoadAudio(AssetPathConfig.GAME_AUIDO_PATH + "zhizu/NorthrendGhoulWound1.wav");
        }
        if (audioClip != null)
        {
            AudioController.GetInstance().PlaySoundByAudioClip(audioClip, false, 2);
        }
    }
}