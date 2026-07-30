using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using ZM.ZMAsset;

public class ConfigCenter
{
    public static List<HeroData> HeroDataList { get; private set; }
    
    public static List<HeroData> EnemyDataList { get; private set; }
    
    public static List<LevelData> LevelDataList { get; private set; }

    public static void Init()
    {
        LoadHeroConfig();
        LoadLevelConfig();
        //SkillConfigCenter.Initialized();
    }

    public static void LoadHeroConfig()
    {
#if CLIENT_LOGIC
        TextAsset text = ZMAsset.LoadTextAsset(AssetsPathConfig.HALL_DATA_PATH + "tbherodatacfg.json");
        HeroDataList = JsonConvert.DeserializeObject<List<HeroData>>(text.text);
        Debugger.Log("heroDataList.Count" + HeroDataList.Count);
#else
        string heroPath = AssetPathConfig.SERVER_CONFIG_PATH + "tbherodatacfg.json";
        string heroJson = File.ReadAllText(heroPath);
        HeroDataList = JsonConvert.DeserializeObject<List<HeroData>>(heroJson);
#endif
        Debugger.Log("heroDataList.Count" + HeroDataList.Count);
    }
    
    public static void LoadLevelConfig()
    {
#if CLIENT_LOGIC
        TextAsset text = ZMAsset.LoadTextAsset(AssetsPathConfig.HALL_DATA_PATH + "tblevelconfig.json");
        LevelDataList = JsonConvert.DeserializeObject<List<LevelData>>(text.text);
        Debugger.Log("LevelDataList.Count" + LevelDataList.Count);
#else
        string levelPath = AssetPathConfig.SERVER_CONFIG_PATH + "tblevelconfig.json";
        string levelJson = File.ReadAllText(levelPath);
        LevelDataList = JsonConvert.DeserializeObject<List<LevelData>>(levelJson);
#endif
        Debugger.Log("LevelDataList.Count" + LevelDataList.Count);
    }

    public static HeroData GetHeroData(int heroId)
    {
        foreach (var heroData in HeroDataList)
        {
            if (heroData.id == heroId)
            {
                return heroData;
            }
        }

        return null;
    }
    
    public static LevelData GetLevelData(int levelId)
    {
        foreach (var levelData in LevelDataList)
        {
            if (levelData.levelID == levelId)
            {
                return levelData;
            }
        }
        return null;
    }
}