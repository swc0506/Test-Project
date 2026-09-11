/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/9/10 18:26:57
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;
using ZMGC.Hall;

namespace ZM.UI
{
    public class LevelReplayWindow : WindowBase
    {
        public LevelReplayWindowDataComponent dataCompt;
        private LevelDataMgr mLevelDataLayer;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            dataCompt = gameObject.GetComponent<LevelReplayWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
            mLevelDataLayer = HallWorld.GetExitsDataMgr<LevelDataMgr>();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
            HallWorld.GetExitsMsgMgr<LevelMsgMgr>().SendGetReplayDataListRequest();
            UIEventControl.AddEvent(UIEventEnum.ReplayDataListShow, OnReplayDataListShow);
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
            UIEventControl.RemoveEvent(UIEventEnum.ReplayDataListShow, OnReplayDataListShow);
            dataCompt.ReplayZMUIListView.OnRelease();
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        private void OnReplayDataListShow(object data)
        {
            RefreshViewList();
        }

        /// <summary>
        /// 刷新无限滚动列表
        /// </summary>
        private void RefreshViewList()
        {
            dataCompt.ReplayZMUIListView.RefreshListView(true, mLevelDataLayer.ReplayDataList.Count,
                OnGetListDataCallBack);
        }

        private object OnGetListDataCallBack(int index)
        {
            return mLevelDataLayer.ReplayDataList[index];
        }

        #endregion

        #region UI组件事件

        public void OnCloseButtonClick()
        {
            HideWindow();
        }

        #endregion
    }
}