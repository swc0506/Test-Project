using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

#if CLIENT_LOGIC
[CreateAssetMenu(fileName = "BuffConfig", menuName = "BuffConfig", order = 0)]
public class BuffConfig : ScriptableObject
#else
public class BuffConfig
#endif
{
    [HideInInspector] public bool hideDamagePercentage = true;

    [LabelText("Buff图标"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel("Buff图标")]
    [JsonIgnore]
    public Sprite buffIcon; //buff图标

    [LabelText("BuffID")] public int buffId;
    [LabelText("Buff名称")] public string buffName;
    [LabelText("最大叠加层数")] public int maxStackingNum; //最大叠加层数
    [LabelText("持续时间")] public int buffDurationTimeMs; //buff持续时间
    [LabelText("持续回合")] public int buffDurationRound; //buff的持续回合
    [LabelText("触发间隔")] public int buffTriggerIntervalMs; //buff触发间隔
    [LabelText("触发概率")] public int buffTriggerProbability; //buff触发概率
    [LabelText("Buff类型")] public BuffType buffType;
    [LabelText("Buff状态")] public BuffState buffState;
    [LabelText("Buff触发方式")] public BuffTriggerType triggerType;

    [LabelText("Buff伤害类型"), OnValueChanged("DamageTypeChange")]
    public BuffDamageType damageType;

    [LabelText("伤害百分比"), ProgressBar(0, 500, 0.8f, 0), HideIf("hideDamagePercentage")]
    public int damagePercentage; //伤害百分比


    //渲染层数据
    [LabelText("Buff声效"), TitleGroup("Buff渲染", "所有英雄渲染数据会在Buff开始释放时触发")]
    [JsonIgnore] 
    public AudioClip buffAudio; //技能声效

    [LabelText("Buff特效"), TitleGroup("Buff渲染", "所有英雄渲染数据会在Buff开始释放时触发")]
    public string buffEffect; //技能特效

    [LabelText("Buff描述:"), MultiLineProperty(4), HideLabel]
    public string buffDes; //技能描述

    public void DamageTypeChange(BuffDamageType type)
    {
        Debugger.Log(type);
        hideDamagePercentage = type != BuffDamageType.AtkPercentage && type != BuffDamageType.HpPercentage;
    }
}

public enum BuffType
{
    [LabelText("伤害Buff")] DamageBuff, //伤害型buff
    [LabelText("增益Buff")] Buff, //增益buff
    [LabelText("减益Buff")] DeBuff, //减益buff
    [LabelText("控制Buff")] Control, //控制buff
}

public enum BuffState
{
    [LabelText("无配置")] None,
    [LabelText("百分比减伤")] PercentageReduceDamage, //百分比减伤
    [LabelText("伤害加深")] DamageDeepening, //伤害加深
    [LabelText("生命值回复增加")] HpRecoveryIncrease, //生命值回复增加
    [LabelText("生命值回复减少")] HpRecoveryReduce, //生命值回复减少
    [LabelText("灼烧")] Burn, //灼烧
    [LabelText("净化")] Purify, //净化
    [LabelText("冰冻")] Frozen, //冰冻
    [LabelText("攻击力增加")] AtkAdd, // 攻击力增加
    [LabelText("防御力增加")] DefAdd, // 防御力增加
}

public enum BuffTriggerType
{
    [LabelText("即时性 一次性伤害")] OneDamageRealTime, //即时性 一次性伤害
    [LabelText("即时性 多段伤害")] MultisegmentDamageRealTime, //即时性 多段伤害
    [LabelText("回合开始时伤害")] DamageRoundStart, //回合开始时伤害
    [LabelText("回合结束时伤害")] DamageRoundEnd, //回合结束时伤害
}

public enum BuffDamageType
{
    [LabelText("无配置")] None,
    [LabelText("普通伤害")] NormalAttackDamage,
    [LabelText("真实伤害")] RealDamage,
    [LabelText("攻击力百分比伤害")] AtkPercentage, //攻击力百分比伤害
    [LabelText("生命值百分比伤害")] HpPercentage, //生命值百分比伤害
    [LabelText("无伤害控制型")] NoneDamageControl, //无伤害控制型
}