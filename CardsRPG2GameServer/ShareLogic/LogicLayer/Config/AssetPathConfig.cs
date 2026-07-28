using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetPathConfig
{
    public const string PREFABS = "Prefabs/";
    public const string HERO = PREFABS + "Hero/";
    public const string HUD = PREFABS + "HUD/";
    public const string SKILL_EFFECT = PREFABS + "SkillEffect/";
    public const string BUFF_EFFECT = PREFABS + "BuffEffect/";
    public const string SKILL_CONFIG = "Skill/";
    public const string BUFF_CONFIG = "Buff/";

    public const string GAME_DATA_PATH = "Assets/GameData/";
    public const string GAME_ITEM_PATH = GAME_DATA_PATH + "GameItem/";
    
    public const string HALL_PATH = GAME_DATA_PATH + "HallWorld/";
    public const string HALL_PREFABS_PATH = HALL_PATH + "Prefabs/";
    public const string HALL_DYNAMICITEM_PATH = HALL_PREFABS_PATH + "DynamicItem/";
    public const string HALL_HOTFIXDLL_PATH = HALL_PATH + "HotFixDll/";
    public const string HALL_EFFECT_PATH = HALL_PATH + "Effects/";
    
    public const string HALL_TEXTURE_PATH = HALL_PATH + "Textures/";
    public const string HALL_DATA_PATH = HALL_PATH + "CfgData/";
    
    public static string SERVER_CONFIG_PATH {get { return AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\ShareLogic\LogicLayer\Config\"; }}
}
