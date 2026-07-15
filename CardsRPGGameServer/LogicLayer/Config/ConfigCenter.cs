using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ConfigCenter
{
    public static List<HeroData> heroDataList { get; private set; }

    public static void Init()
    {
        LoadHeroConfig();
    }

    public static void LoadHeroConfig()
    {
#if CLIENT_LOGIC
        TextAsset text = ResourcesManager.Instance.LoadAsset<TextAsset>("Config/Hero");
        heroDataList = JsonConvert.DeserializeObject<List<HeroData>>(text.text);
        Debugger.Log("heroDataList.Count" + heroDataList.Count);
#else
        string heroPath = AssetPathConfig.SERVER_CONFIG_PATH + "Hero.json";
        string heroJson = File.ReadAllText(heroPath);
        heroDataList = JsonConvert.DeserializeObject<List<HeroData>>(heroJson);
#endif
    }

    public static HeroData GetHeroData(int heroId)
    {
        foreach (var heroData in heroDataList)
        {
            if (heroData.id == heroId)
            {
                return heroData;
            }
        }

        return null;
    }
}