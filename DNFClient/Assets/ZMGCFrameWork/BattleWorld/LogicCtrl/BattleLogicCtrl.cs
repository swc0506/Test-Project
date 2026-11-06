/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/3/16 13:08:23
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using FixMath;
using System.Collections.Generic;
using ZMGC.Hall;

namespace ZMGC.Battle
{
    public class BattleLogicCtrl : ILogicBehaviour
    {
        private HeroLogicCtrl mHeroCtrl;
        private MonsterLogicCtrl mMonsterCtrl;
        private BattleDataMgr mBattleDataLayer;
        private BattleMsgMgr mBattleMsgLayer;
        private BattleWorld mBattleWorld;
        public void OnCreate()
        {
            UnityEngine.Debug.Log("BattleLogicCtrl OnCreate");
            mHeroCtrl = BattleWorld.GetExitsLogicCtrl<HeroLogicCtrl>();
            mMonsterCtrl = BattleWorld.GetExitsLogicCtrl<MonsterLogicCtrl>();
            mBattleDataLayer = BattleWorld.GetExitsDataMgr<BattleDataMgr>();
            mBattleWorld = WorldManager.DefaultGameWorld as BattleWorld;
            mBattleMsgLayer = BattleWorld.GetExitsMsgMgr<BattleMsgMgr>();
        }

        /// <summary>
        /// 退出地下城返回大厅
        /// </summary>
        public async void ExitDungeon()
        {
            ExitDungeonResponse response = await mBattleMsgLayer.SendExitDungeonRequest(mBattleDataLayer.battleId, BattleWorld.UserData.AccountId);
            if (response.ErrorCode == 0)
            {
                //退出地下城成功
                WorldManager.DestroyWorld<BattleWorld>();
            }
        }
        public void LocalBattleEnd()
        {
            mBattleMsgLayer.SendBattleEndMessage(mBattleDataLayer.battleId, BattleWorld.UserData.AccountId);
            //处理本地战斗结束的逻辑 TODO
        }
        /// <summary>
        /// 战斗结束服务端通知
        /// </summary>
        public void OnBattleEndByServer()
        {
            //弹出战斗奖励的弹窗了 
            UIEventControl.DispensEvent(UIEventEnum.ShowBattleReward);
        }
        /// <summary>
        /// 领取结算奖励通知
        /// </summary>
        /// <param name="index"></param>
        public void OnReceiveCradReward(int index)
        {
            UIEventControl.DispensEvent(UIEventEnum.ReceiveCardReward, index);
        }
        /// <summary>
        /// 移动帧数据输入
        /// </summary>
        /// <param name="inputDir"></param>
        public void MoveFrameDataInput(FixIntVector3 inputDir)
        {
            SendFrameOperateData(OperateType.InputMove, inputDir, 0, FixIntVector3.zero);
        }
        /// <summary>
        /// 释放技能输入
        /// </summary>
        /// <param name="skillid">技能id</param>
        /// <param name="skillPos">技能引导位置</param>
        public void ReleaseSkillInput(int skillid, OperateSkillType skillType = OperateSkillType.ClickSkill, FixIntVector3 skillPos = default(FixIntVector3))
        {
            SendFrameOperateData(OperateType.ReleaseSkill, FixIntVector3.zero, skillid, skillPos, skillType);
        }
        /// <summary>
        /// 发送帧操作数据到服务端
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="inputDir">输入方向</param>
        /// <param name="skillid">释放的技能id</param>
        /// <param name="skillPos">技能引导的位置</param>
        /// <param name="skillType">技能类型</param>
        private void SendFrameOperateData(OperateType operateType, FixIntVector3 inputDir, int skillid, FixIntVector3 skillPos, OperateSkillType skillType = OperateSkillType.None)
        {
            //战斗未开始或已结束
            if (mBattleDataLayer.BattleStateEnum != BattleStateEnum.Start) return;
            //创建操作帧数据结构体
            FrameOperateData frameData = new FrameOperateData();
            frameData.operateType = (int)operateType;
            frameData.playerid = BattleWorld.UserData.AccountId;
            switch (operateType)
            {
                case OperateType.NoOperate:
                    break;
                case OperateType.InputMove:
                    frameData.inputDir = new CSFixIntVector3() { x = inputDir.x.IntValue, y = inputDir.y.IntValue, z = inputDir.z.IntValue };
                    break;
                case OperateType.ReleaseSkill:
                    frameData.skillid = skillid;
                    frameData.skillType = (int)skillType;
                    frameData.skillPos = new CSFixIntVector3() { x = skillPos.x.IntValue, y = skillPos.y.IntValue, z = skillPos.z.IntValue };
                    break;
            }
            //收集帧操作数据
            mBattleDataLayer.frameOperateDataList.Add(frameData);
            //把当前客户端帧操作消息发送给服务端
            mBattleMsgLayer.SendFrameOperateEventMessage(mBattleDataLayer.battleId, mBattleDataLayer.frameOperateDataList);
            //清理帧操作数据列表
            mBattleDataLayer.frameOperateDataList.Clear();
        }

        /// <summary>
        /// 通过服务端调用的逻辑帧更新接口
        /// </summary>
        public void OnLogicFrameUpdateByServer(FrameOperateEventMessage_G2C message)
        {
            //战斗状态记录和切换
            mBattleDataLayer.BattleStateEnum = BattleStateEnum.Start;
            mBattleDataLayer.battleId = message.battleid;
            //逻辑帧缓存
            LogicFrameConfig.LogicFrameid = message.logicFrameid;

            //更新玩家的输入
            for (int i = 0; i < message.frameDataList.Count; i++)
            {
                FrameOperateData frameData = message.frameDataList[i];
                HeroLogic heroLogic = mHeroCtrl.GetHeroLogicByPlayerid(frameData.playerid);
                heroLogic.InputFrameOperateData(frameData);
            }

            //调用逻辑帧更新接口
            mBattleWorld.OnLogicFrameUpdate();
        }

        /// <summary>
        /// 根据当前对象类型获取攻击的目标
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<LogicActor> GetEnemyList(LogicObjectType objectType)
        {
            List<LogicActor> enemyList = new List<LogicActor>();
            if (objectType == LogicObjectType.Hero)
            {
                foreach (var item in mMonsterCtrl.monsterList)
                {
                    enemyList.Add(item);
                }
            }
            else if (objectType == LogicObjectType.Monster)
            {
                foreach (var item in mHeroCtrl.heroLogicList)
                {
                    enemyList.Add(item);
                }
                //enemyList.Add(mHeroCtrl.HeroLogic);
            }
            return enemyList;
        }



        public void OnDestroy()
        {

        }

    }
}
