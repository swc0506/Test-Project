using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class ConfigCenter
{
    public static List<HeroData> HeroDataList { get; private set; }
    
    public static List<HeroData> EnemyDataList { get; private set; }

    public static void Init()
    {
        LoadHeroConfig();
        EnemyDataList = new List<HeroData>();
        for (int i = 0; i < 5; i++)
        {
            HeroData heroData = GetHeroData(500 + i + 1);
            heroData.seatId = i;
            EnemyDataList.Add(heroData);
        }
    }

    public static void LoadHeroConfig()
    {
#if CLIENT_LOGIC
        TextAsset text = ResourcesManager.Instance.LoadAsset<TextAsset>("Config/Hero");
        HeroDataList = JsonConvert.DeserializeObject<List<HeroData>>(text.text);
        Debugger.Log("HeroDataList.Count" + HeroDataList.Count);
#else
        string heroPath = AssetPathConfig.SERVER_CONFIG_PATH + "Hero.json";
        string heroJson = File.ReadAllText(heroPath);
        HeroDataList = JsonConvert.DeserializeObject<List<HeroData>>(heroJson);
#endif
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
}