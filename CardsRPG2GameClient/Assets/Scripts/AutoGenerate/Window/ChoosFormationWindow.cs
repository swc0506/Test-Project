/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/7/31 11:48:33
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ZMGC.Hall;

namespace ZM.UI
{
    public class ChoosFormationWindow : WindowBase
    {
        public ChoosFormationWindowDataComponent dataCompt;
        private GameObject cloneObj;
        private GameObject battleRoot;
        private BattleRoot3D root3D;
        
        private List<GameObject> enemyList = new List<GameObject>();

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            FullScreenWindow = true;
            mDisableAnim = true;
            dataCompt = gameObject.GetComponent<ChoosFormationWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            RefreshViewList();
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
            ReleaseAllObj();
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
            dataCompt?.GoBattleHeroZMUIIGridView?.OnRelease();
        }

        #endregion

        #region API Function

        public void Init(List<int> enemyIDs)
        {
            LoadMap();
            LoadBattleRoot();
            LoadEnemy(enemyIDs);
        }

        private void LoadEnemy(List<int> enemyIDs)
        {
            for (int i = 0; i < enemyIDs.Count; i++)
            {
                var heroData =  ConfigCenter.GetHeroData(enemyIDs[i]);
                Transform seatTrans = root3D.rightSeatTransArr[i];
                GameObject obj = ZMAsset.ZMAsset.InstantiateObject(
                    $"{AssetsPathConfig.HALL_PREFABS_PATH}BattleRoles/role_{heroData.name}", seatTrans);
                enemyList.Add(obj);
            }
        }

        private void RefreshViewList()
        {
            dataCompt.NoHeroGameObject.SetVisible(HallWorld.UserData.HeroIdList.Count == 0);
            dataCompt.GoBattleHeroZMUIIGridView.RefreshListView(true, HallWorld.UserData.HeroIdList.Count, (dataIndex) =>
            {
                return ConfigCenter.GetHeroData(HallWorld.UserData.HeroIdList[dataIndex]);
            });
        }

        private void LoadMap()
        {
            cloneObj = ZMAsset.ZMAsset.InstantiateObject($"{AssetsPathConfig.HALL_PREFABS_PATH}Battle/Map3", null);
        }

        private void LoadBattleRoot()
        {
            battleRoot =
                ZMAsset.ZMAsset.InstantiateObject($"{AssetsPathConfig.HALL_PREFABS_PATH}Battle/3DBattleRoot", null);
            root3D = battleRoot.GetComponent<BattleRoot3D>();
        }

        private void ReleaseAllObj()
        {
            if (cloneObj != null)
            {
                ZMAsset.ZMAsset.Release(cloneObj);
                cloneObj = null;
            }

            if (battleRoot != null)
            {
                ZMAsset.ZMAsset.Release(battleRoot);
                battleRoot = null;
            }

            foreach (var enemy in enemyList)
            {
                ZMAsset.ZMAsset.Release(enemy);
            }
            enemyList.Clear();
        }

        #endregion

        #region UI组件事件

        public void OnStartFightButtonClick()
        {
        }

        public void OnCloseButtonClick()
        {
            HideWindow();
        }

        #endregion
    }
}