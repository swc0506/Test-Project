using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataModel
{
    public const string key = "BattleDataKey";
    public int battleSite; //战斗随机种子
    public int battleId; //战斗id
    public List<HeroData> heroList;
    public List<HeroData> enemyList;
}
