/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/3/14 8:50:10
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using ZMGC.Battle;
using System.Collections.Generic;
using FixMath;
using ZM.AssetFrameWork;

public class BattleWindow : WindowBase
{
    public BattleWindowDataComponent dataCompt;

    private HeroLogic mHeroLogic;

    //技能按钮节点列表
    private List<Transform> mSkillItemRootList;

    //技能列表
    private List<SkillItem> mSkillItemList = new List<SkillItem>();

    //上一次显示血条的时间
    private float mLastShowBloodTime = 0;

    //当前显示的血条
    private MonsterBloodItem mCurBloodItem;

    #region 声明周期函数

    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<BattleWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
        mSkillItemRootList = new List<Transform>();
        for (int i = 0; i < dataCompt.SKillRootTransform.childCount; i++)
        {
            mSkillItemRootList.Add(dataCompt.SKillRootTransform.GetChild(i));
        }
    }

    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        mHeroLogic = BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>().HeroLogic;

        //获取角色技能id数组
        int[] heroSkillidArr = BattleWorld.GetExitsDataMgr<HeroDataMgr>().GetHeroSkillIdArr(mHeroLogic.Heroid);
        //遍历角色技能数组，生成对应的技能按钮
        for (int i = 0; i < heroSkillidArr.Length; i++)
        {
            GameObject skillItemObj =
                ZM.AssetFrameWork.ZMAssetsFrame.Instantiate(AssetPathConfig.GAME_PREFABS + "Item/SkillItem",
                    mSkillItemRootList[i]);
            SkillItem item = skillItemObj.GetComponent<SkillItem>();
            mSkillItemList.Add(item);
            item.SetItemSkillData(mHeroLogic.GetSKill(heroSkillidArr[i]), mHeroLogic);
            skillItemObj.transform.localPosition = Vector3.zero;
            skillItemObj.transform.rotation = Quaternion.identity;
            skillItemObj.transform.localScale = Vector3.one;
        }
    }

    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
    }

    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region API Function

    /// <summary>
    /// 显示怪物血条
    /// </summary>
    /// <param name="monsterCfg">怪物配置</param>
    /// <param name="insid">对象Instanceid</param>
    /// <param name="curHp">怪物当前的血量</param>
    /// <param name="damageHp">伤害量</param>
    public void ShowMonsterDamage(MonsterCfg monsterCfg, int insid, FixInt curHp, FixInt damageHp)
    {
        //同一个怪物的血条显示
        if (mCurBloodItem != null && mCurBloodItem.curShowMonsterInsid == insid)
        {
            mLastShowBloodTime = Time.realtimeSinceStartup;
            mCurBloodItem.Damage(-damageHp.RawInt);
            return;
        }

        //血条显示冷却
        if (monsterCfg.type != MonsterType.Boss && Time.realtimeSinceStartup - mLastShowBloodTime < 0.5f)
        {
            return;
        }

        //回收血条对象到对象池
        if (mCurBloodItem != null)
        {
            ZMAssetsFrame.Release(mCurBloodItem.gameObject);
            mCurBloodItem = null;
        }

        //生成对应的血条
        string bloodName = monsterCfg.type == MonsterType.Boss ? "BossBlood" : "MonsterBlood";
        GameObject itemObj = ZMAssetsFrame.Instantiate(AssetPathConfig.GAME_PREFABS + "DamageItem/" + bloodName,
            dataCompt.BloodRootTransform, Vector3.zero, Vector3.one, Quaternion.identity);
        mCurBloodItem = itemObj.GetComponent<MonsterBloodItem>();
        mCurBloodItem.InitBloodData(monsterCfg, curHp.RawInt, insid);
        mCurBloodItem.Damage(-damageHp.RawInt);
        //记录上次血条显示的时间
        mLastShowBloodTime = Time.realtimeSinceStartup;
    }

    #endregion

    #region UI组件事件

    public void OnNormalAttackButtonClick()
    {
        mHeroLogic.ReleaseNormalAttack();
    }

    #endregion
}