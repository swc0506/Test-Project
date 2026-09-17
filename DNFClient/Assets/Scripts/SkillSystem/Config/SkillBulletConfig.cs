using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class SkillBulletConfig  
{
    [AssetList,LabelText("特效"),PreviewField(70,ObjectFieldAlignment.Left)]
    public GameObject bulletPrefab;
    [LabelText("智能锁定寻敌(常用于普通子弹，智能调整子弹发射角度射向前方敌人)")]
    public bool interlligentAttack;
    [LabelText("触发帧")]
    public int triggerFrame;
    [LabelText("是否循环创建"),BoxGroup("循环创建参数")]
    public bool isLoopCreate;
    [LabelText("循环间隔ms 毫秒"),ShowIf("isLoopCreate"), BoxGroup("循环创建参数")]
    public int loopIntervalMs;
    [LabelText("最小随机位置波动范围"), ShowIf("isLoopCreate"), BoxGroup("循环创建参数")]
    public Vector3 minrandomRangeVect3;
    [LabelText("最大随机位置波动范围"), ShowIf("isLoopCreate"), BoxGroup("循环创建参数")]
    public Vector3 maxRandomRangeVect3;
    [LabelText("移动速度")]
    public float moveSpeed;
    [LabelText("存活时间（毫秒）")]
    public int survivalTimeMsg;
    [LabelText("重力加速度")]
    public Vector2 gravitySpeed;
    [LabelText("发射位置偏移")]
    public Vector3 offset;
    [LabelText("发射角度")]
    public Vector3 angle;
    [LabelText("是否击中销毁")]
    public bool isHitDestory = true;
    [LabelText("击中特效"),PreviewField(70,ObjectFieldAlignment.Left)]
    public GameObject hitEffect;
    [LabelText("击中特效存活时间")]
    public int hitEffectSurvivalTimems = 3000;
    [LabelText("击中音效")]
    public AudioClip hitAudio;
    [ToggleGroup("isAttachDamage","是否附加伤害")]
    public bool isAttachDamage = false;
    [ToggleGroup("isAttachDamage", "是否附加伤害")]
    public SkillDamageConfig damageCfg;
}
