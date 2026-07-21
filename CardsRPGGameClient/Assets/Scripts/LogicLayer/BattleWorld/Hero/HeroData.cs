using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroData
{
    public HeroData()
    {

    }
    public int id;
    public int seatId;//位置 座位 id
    
    public int[] skillIdArr;//技能数组
    public int hp;//声明值
    public int atk;//攻击力
    public int def;//防御力
    public int agl;//敏捷
    public int atkRage;//攻击怒气值
    public int takeDamageRage; //受击怒气值
    public int maxRage;//最大怒气
    // public BattleHeroDataPb ToBattleHeroData()
    // {
    //     BattleHeroDataPb heroDataPb = new BattleHeroDataPb();
    //     heroDataPb.id = id;
    //     heroDataPb.seatId = seatId;
    //     heroDataPb.skillIdArr = skillIdArr;
    //     heroDataPb.hp = hp;
    //     heroDataPb.atk = atk;
    //     heroDataPb.def = def;
    //     heroDataPb.agl = agl;
    //     heroDataPb.atkRange = atkRange;
    //     heroDataPb.takeDamageRange = takeDamageRange;
    //     heroDataPb.maxRage = maxRage;
    //     return heroDataPb;
    // }
}
