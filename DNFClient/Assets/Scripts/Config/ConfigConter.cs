using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

public class ConfigCenter:Singleton<ConfigCenter>
{ 
    //英雄配置字典
    private Dictionary<int, HeroDataCfg> mHeroDtaCfgDic;
    //怪物配置字典
    private Dictionary<int, MonsterCfg> mMonsterCfgDic;
    public void InitGameCfg()
    {
        LoadMonsterCfg();
        LoadHeroCfg();
    }

    #region 配置加载
    private void LoadMonsterCfg()
    {
        mMonsterCfgDic = new Dictionary<int, MonsterCfg>();
        //加载配置文件
         TextAsset textAsset = ZMAssetsFrame.LoadTextAsset(AssetPathConfig.GAME_DATA_PATH+ "MonsterCfg.json");
        if (textAsset==null)
        {
            Debug.LogError("LoadMonsterCfg Failed, textAsset is Null !");
            return;
        }
        //反序列化Json
        List<MonsterCfg> monsterCfgList =JsonConvert.DeserializeObject<List<MonsterCfg>>(textAsset.text);
        foreach (var item in monsterCfgList)
        {
            mMonsterCfgDic.Add(item.id,item);
        }
        Debug.Log("LoadMonsterCfg Success, Count:"+mMonsterCfgDic.Count);
    }
    private void LoadHeroCfg()
    {
        mHeroDtaCfgDic = new Dictionary<int, HeroDataCfg>();
        //加载配置文件
        TextAsset textAsset = ZMAssetsFrame.LoadTextAsset(AssetPathConfig.GAME_DATA_PATH + "HeroDataCfg.json");
        if (textAsset == null)
        {
            Debug.LogError("LoadHeroCfg Failed, textAsset is Null !");
            return;
        }
        //反序列化Json
        List<HeroDataCfg> heroCfgList = JsonConvert.DeserializeObject<List<HeroDataCfg>>(textAsset.text);
        foreach (var item in heroCfgList)
        {
            mHeroDtaCfgDic.Add(item.id, item);
        }
        Debug.Log("LoadHeroCfg Success, Count:" + mHeroDtaCfgDic.Count);
    }
    #endregion

    #region 配置获取
    /// <summary>
    /// 通过怪物id获取怪物配置数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MonsterCfg GetMonsterCfgById(int id)
    {
        MonsterCfg monsterCfg = null;
        mMonsterCfgDic.TryGetValue( id,out monsterCfg);
        return monsterCfg;
    }
    /// <summary>
    /// 通过怪物id获取英雄配置数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public HeroDataCfg GetHeroCfgById(int id)
    {
        HeroDataCfg heroCfg = null;
        mHeroDtaCfgDic.TryGetValue(id, out heroCfg);
        return heroCfg;
    }
    #endregion
}
