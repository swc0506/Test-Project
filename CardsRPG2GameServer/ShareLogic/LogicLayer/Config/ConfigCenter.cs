using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ConfigCenter
{
    public static List<HeroData> HeroDataList { get; private set; }
    
    public static List<HeroData> EnemyDataList { get; private set; }

    public static void Init()
    {
        LoadHeroConfig();
        //SkillConfigCenter.Initialized();
    }

    public static void LoadHeroConfig()
    {
#if CLIENT_LOGIC
        TextAsset text = ResourcesManager.Instance.LoadAsset<TextAsset>("Config/Hero");
        HeroDataList = JsonConvert.DeserializeObject<List<HeroData>>(text.text);
        Debugger.Log("heroDataList.Count" + HeroDataList.Count);
#else
        string heroPath = AssetPathConfig.SERVER_CONFIG_PATH + "tbherodatacfg.json";
        string heroJson = File.ReadAllText(heroPath);
        HeroDataList = JsonConvert.DeserializeObject<List<HeroData>>(heroJson);
#endif
        Debugger.Log("heroDataList.Count" + HeroDataList.Count);
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