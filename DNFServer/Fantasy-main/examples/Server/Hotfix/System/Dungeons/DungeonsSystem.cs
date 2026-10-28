using Fantasy;
using Hotfix.Helper;
using Hotfix.System.Dungeons;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.System.Dungeons
{
    public class DungeonDestroySystem : DestroySystem<Dungeon>
    {
        protected override void Destroy(Dungeon self)
        {
            if (self.timerid != 0)
            {
                //取消逻辑帧更新计时器
                Log.Info("释放地下城 取消逻辑帧更新计时器："+self.timerid);
                self.Scene.TimerComponent.Core.Remove(ref self.timerid);
            }

            //释放玩家数据
            foreach (var item in self.playerDic)
            {
                item.Value.Dispose();
            }
            //释放玩家字典
            self.playerDic.Clear();
            self.timerid = 0;
            self.logicFrameid = 0;
            self.battleState = BattleStateEnum.None;
            Log.Info("释放地下城");
        }
    }

    public static class DungeonsSystem
    {
        /// <summary>
        /// 帧操作数据列表
        /// </summary>
        public static List<FrameOperateData> mFrameOperateDatList = new List<FrameOperateData>();

        public static void BattleStart(this Dungeon self)
        {
            //战斗开始
            self.battleState = BattleStateEnum.Start;
            //开启逻辑帧更新 开启一个线程，以0.066秒推送一个逻辑帧 
            //开启一个计时器
            self.timerid = self.Scene.TimerComponent.Core.RepeatedTimer(CommonConfig.LogicFrameIntervalms, self.LogicFrameUpdate);
        }

        /// <summary>
        /// 逻辑帧更新 推送逻辑帧更新和收到的客户端上一帧操作
        /// </summary>
        /// <param name="self"></param>
        public static void LogicFrameUpdate(this Dungeon self)
        {
            try
            {
                self.logicFrameid++;
                //给地下城中的所有的玩家，推送逻辑帧更新通知
                FrameOperateEventMessage_G2C message = new FrameOperateEventMessage_G2C();
                message.battleid = self.battleid;
                message.logicFrameid = self.logicFrameid;
                message.frameDataList = new List<FrameOperateData>();
                //收集所有客户端当前帧的操作
                lock (mFrameOperateDatList)
                {
                    foreach (var item in mFrameOperateDatList)
                    {
                        message.frameDataList.Add(item);
                    }
                    //清理缓存的上一帧玩家操作数据
                    mFrameOperateDatList.Clear();
                }
                Log.Info("给地下城内的所有玩家广播帧同步消息和数据:"+self.playerDic.Count);
                //给地下城内的所有玩家广播帧同步消息和数据
                foreach (var palyer in self.playerDic)
                {
                    if (!palyer.Value.session.IsDisposed)
                    {
                        //推送逻辑帧消息
                        palyer.Value.session.Send(message);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("逻辑帧更新异常，请及时检查：" + e);
            }
        }
        /// <summary>
        /// 同步玩家帧操作数据
        /// </summary>
        /// <param name="battleid"></param>
        /// <param name="frameDataList"></param>
        public static void SyncPlayerFrameData(this Dungeon self,long battleid,List<FrameOperateData> frameDataList)
        {
            lock (mFrameOperateDatList)
            {
                foreach (var data in frameDataList)
                {
                    mFrameOperateDatList.Add(data);
                }
            }
        }
    }
}
