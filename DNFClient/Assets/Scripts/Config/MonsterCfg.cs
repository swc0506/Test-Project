using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MonsterType
{ 
    Normal=1,
    Elite=2,
    Boss=5,
}
public class MonsterCfg  
{
    public int id;//怪物id
    public string name;//怪物名称
    public MonsterType type;//怪物类型 1.普通怪物 2.精英怪物 5.Boss
    public string monsterDes;//怪物描述
    public int[] skillidArr;//技能数组
    public int hp;//血量
    public int mp;//法力值
    public int ap;//物理攻击力
    public int ad;//物理防御
    public int adDef;//物理防御百分比
    public int apDef;//法术防御百分比
    public int pct;//物理暴击率
    public int mct;//法术暴击率
    public float adPctRate;//物理暴击伤害倍率
    public float apMctRate;//法术暴击伤害倍率
    public int str;//力量
    public int sta;//耐力
    public int Int;//智力
    public int spi;//精神
    public int agl;//敏捷值
    public int atkRange; //攻击距离，用于区别远程怪物和近战怪物的攻击距离
    public int searchDisRange;//搜寻距离 用于出生后首次搜寻目标进行进行追击
}
