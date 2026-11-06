/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2024/3/16 13:08:51
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
using Fantasy;
using System.Collections.Generic;

namespace ZMGC.Battle
{
    public enum OperateSkillType
    {
        None,//未配置
        ClickSkill,//默认点击触发的技能
        GuideSkill,//引导技能
        StockPileTriggerSkill,//蓄力技能
    }
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperateType
    {
        NoOperate,//没有操作
        InputMove,//输入移动
        ReleaseSkill,//释放技能
    }
    public enum BattleStateEnum
    {
        None,
        Start,
        End,
    }
    /// <summary>
    /// 主要负责游戏模块的数据储存
    /// </summary>
    public class BattleDataMgr : IDataBehaviour
    {
        public long battleId;
        public BattleStateEnum BattleStateEnum { get; set; }

        /// <summary>
        /// 帧操作数据
        /// </summary>
        public List<FrameOperateData> frameOperateDataList=new List<FrameOperateData>();
        public void OnCreate()
        {

        }

        public void OnDestroy()
        {

        }

    }
}
