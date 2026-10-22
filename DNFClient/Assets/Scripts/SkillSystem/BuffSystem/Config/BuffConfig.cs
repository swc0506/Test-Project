using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff配置", menuName = "Buff配置", order = 0)]
[System.Serializable]
public class BuffConfig : ScriptableObject
{
    [LabelText("buff图标"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel("Buff图标")]
    public Sprite buffIcon; //buff图标 

    [LabelText("buffID")] public int buffid; //Buff唯一id
    [LabelText("Buff名称")] public string buffName;
    [LabelText("延迟触发时间")] public int buffDelay; //延迟buff  1 2 3 秒之后触发buff对应的效果 例如造成伤害 生命回复
    [LabelText("触发间隔")] public int buffIntervalms; //Buff 每过n毫秒重复一次该值

    [LabelText("Buff持续时间(0表示一次 -1表示buff时间无限 直到战斗结束)")]
    public int buffDurationms; //0表示一次 -1表示buff时间无限 直到战斗结束  >0Buff的实际生效时间

    [LabelText("Buff类型")] public BuffType buffType; //表示当前buff是什么buff 例如击退 浮空
    [LabelText("附加目标")] public BuffAttachType attachType;
    [LabelText("Buff位置")] public BuffPosType buffPosType;
    [LabelText("Buff伤害类型")] public DamageType damageType;
    [LabelText("Buff伤害值")] public int damageRate;
    [LabelText("Buff 数值列表")] public List<BuffParams> buffParamsList;
    [LabelText("抓取数据")] public TargetGrabData targetGrabData; //抓取数据，用于把目标抓取到某一目标位置


    [LabelText("buff触发音效"), TitleGroup("特效配置", "所有的特效数据会在Buff释放时Buff生效时播放")]
    public AudioClip buffAudio;

    [LabelText("Buff特效配置"), TitleGroup("特效配置", "所有的特效数据会在Buff释放时Buff生效时播放")]
    public BuffEffectConfig effectConfig;

    [LabelText("Buff受击特效"), TitleGroup("特效配置", "所有的特效数据会在Buff释放时Buff生效时播放"), OnValueChanged("GetObjectPath")]
    public GameObject buffHitEffectObj;

    [ReadOnly] public string buffHitEffectObjPath;

    [LabelText("Buff触发动画"), TitleGroup("特效配置", "所有的特效数据会在Buff释放时Buff生效时播放")]
    public ObjectAnimationState buffTriggerAnim = ObjectAnimationState.None; //buff触发的动画

    [LabelText("伤害/目标数据")] public TargetConfig targetConfig; //buff伤害配置/目标数据

    [Title("Buff描述"), HideLabel, MultiLineProperty(5)]
    public string buffDes;

#if UNITY_EDITOR
    public void GetObjectPath(GameObject obj)
    {
        buffHitEffectObjPath = UnityEditor.AssetDatabase.GetAssetPath(obj);
        Debug.Log("skillHitEffectPath:" + buffHitEffectObjPath);
    }

    public void SaveAsset()
    {
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}

[System.Serializable, TabGroup("目标数据")]
public class TargetConfig
{
    [LabelText("是否开启")] public bool isOpen = false;
    [LabelText("触发目标"), ShowIf("isOpen")] public TargetType targetType; //触发目标

    [LabelText("伤害配置数据"), ShowIf("isOpen")]
    public SkillDamageConfig damageCfg; //伤害配置数据
}

//Eidtor 1.技能核心
//

/// <summary>
/// 表示当前buff触发时需要播放的动画
/// </summary>
public enum ObjectAnimationState
{
    [LabelText("无")] None,
    [LabelText("受击")] BeHit,
    [LabelText("僵直")] Stiff,
}

[System.Serializable]
public class BuffEffectConfig
{
    [LabelText("特效资源"), OnValueChanged("GetObjectPath")]
    public GameObject effect;

    [ReadOnly] public string effectPath;
    [LabelText("特效附加类型")] public EffectAttachType attachType;
    [LabelText("特效位置类型")] public BuffEffectPosType effectPosType;

#if UNITY_EDITOR
    public void GetObjectPath(GameObject obj)
    {
        effectPath = UnityEditor.AssetDatabase.GetAssetPath(obj);
        Debug.Log("effectPath:" + effectPath);
    }
#endif
}

[LabelText("特效位置类型")]
public enum BuffEffectPosType
{
    [LabelText("无")] None,
    [LabelText("跟随目标")] targetFollow,
    [LabelText("目标位置")] TargetPos,
}

[LabelText("特效附加类型")]
public enum EffectAttachType
{
    [LabelText("无")] None,
    [LabelText("中心")] Conter,
    [LabelText("手部")] Hand,
}

[System.Serializable]
public class TargetGrabData
{
    [LabelText("抓取移动目标位置")] public Vector3 garbMoveTargetPos;
    [LabelText("移动到抓取位置所需时间")] public int moveTimems; //表示要抓取的目标移动到抓取目标位置需要的时间
}

[System.Serializable]
public class BuffParams
{
    [LabelText("数值"), PropertyTooltip("例如：导致的伤害来说明的距离")]
    public float value;

    [LabelText("数值说明")] public string des;
}

[LabelText("位置类型")]
public enum BuffPosType
{
    [LabelText("无")] None,
    [LabelText("跟随目标")] FollowTarget, //跟随目标
    [LabelText("指定目标位置")] HitTargetPos, //添加到目标位置
    [LabelText("施法者位置")] ReleaserPos, //UI摇杆获取位置
    [LabelText("UI输入位置")] UIInputPos, //UI摇杆获取位置
}

[LabelText("附加目标类型")]
public enum BuffAttachType
{
    [LabelText("无")] None,
    [LabelText("施法者")] Creator, //添加到施法者身上
    [LabelText("施法目标")] Target, //添加到施法目标身上
    [LabelText("施法者位置")] Creator_Pos, //添加到施法者所在的位置
    [LabelText("施法目标位置")] Target_Pos, //添加到施法目标所在的位置
    [LabelText("引导位置")] Guide_Pos, //技能是在施法者通过技能摇杆选中的位置
}

[LabelText("buff类型")]
public enum BuffType
{
    [LabelText("无")] None = 0,
    [LabelText("击退")] Repel,
    [LabelText("浮空")] Floating,
    [LabelText("僵直")] Stiff,
    [LabelText("群体血量修复")] HP_Modify_Group,

    [LabelText("移动速度修复")] MoveSpeed_Modify_Single,

    [LabelText("抓取")] Grab,
    [LabelText("忽略重力")] IgnoreGravity,
    [LabelText("允许移动")] AllowMove,
    [LabelText("不允许转向")] NotAllowDir,
}