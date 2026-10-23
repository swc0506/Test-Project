using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPathConfig  
{
    

    //资源根路径总结
    public const string GAMEDATA = "Assets/GameData/";

    public const string HALL = GAMEDATA + "Hall/";
    //大厅预制体文件夹
    public const string HALL_PREFABS = HALL + "Prefabs/";
    public const string HALL_PREFABS_ITEM_PATH = HALL_PREFABS + "Item/";
    public const string Hall_TEXTURES_PATH = HALL + "Textures/";
    public const string Hall_EFFECTS_PATH = HALL + "Effects/";

    public const string GAME = GAMEDATA+ "Game/";
    //游戏的预制体路径
    public const string GAME_PREFABS = GAME + "Prefabs/";

    public const string GAME_PREFABS_HERO = GAME_PREFABS + "Hero/";
    public const string GAME_PREFABS_MONSTER = GAME_PREFABS + "Monster/";
  

    public const string SKILL_DATA_PATH = GAME + "SkillSystem/SkillData/";
    public const string BUFF_DATA_PATH = GAME + "SkillSystem/BuffData/";
    public const string GAME_AUIDO_PATH = GAME + "Sound/";

    public const string GAME_DATA_PATH = GAME+"Data/";
    public const string GAME_TEXTURES_PATH = GAME + "Textures/";
}