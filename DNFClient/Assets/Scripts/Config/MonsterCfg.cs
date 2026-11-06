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
    public int adDef;//敏捷值
    public int apDef;//敏捷值
    public int pct;//敏捷值
    public int mct;//敏捷值
    public float adPctRate;//敏捷值
    public float apMctRate;//敏捷值
    public int str;//敏捷值
    public int sta;//敏捷值
    public int Int;//敏捷值
    public int spi;//敏捷值
    public int agl;//敏捷值
    public int atkRange; //攻击距离，用于区别远程怪物和近战怪物的攻击距离
    public int searchDisRange;//搜寻距离 用于出生后首次搜寻目标进行进行追击
}
