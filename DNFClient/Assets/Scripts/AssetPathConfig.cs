using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPathConfig  
{
    //资源加载路径总结点
    public const string GAMEDATA = "Assets/GameData/";

    public const string GAME = GAMEDATA+ "Game/";
    //游戏内预制体路径
    public const string GAME_PREFABS = GAME + "Prefabs/";

    public const string GAME_PREFABS_HERO = GAME_PREFABS + "Hero/";
    public const string GAME_PREFABS_MONSTER = GAME_PREFABS + "Monster/";
    public const string HALL = GAMEDATA+"Hall/";

    public const string SKILL_DATA_PATH = GAME + "SkillSystem/SkillData/";
    public const string BUFF_DATA_PATH = GAME + "SkillSystem/BuffData/";
}
