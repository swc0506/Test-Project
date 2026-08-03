/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2026/7/31 17:59:55
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ZMGC.Hall
{
    public class ChooseFormationDataMgr : IDataBehaviour
    {
        public Dictionary<int, int> HeroSeatDict {get; private set;}
        
        public void OnCreate()
        {
            HeroSeatDict = new Dictionary<int, int>()
            {
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },

            };
        }

        public void OnDestroy()
        {
        }
        
        /// <summary>
        ///  添加英雄到指定位置
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="heroId"></param>
        public void AddHeroToSeat(int seat, int heroId)
        {
            HeroSeatDict[seat] = heroId;
        }
        
        public void RemoveHeroFromSeat(int seat)
        {
            HeroSeatDict[seat] = 0;
        }

        /// <summary>
        ///  交换两个位置的英雄
        /// </summary>
        /// <param name="seat1"></param>
        /// <param name="seat2"></param>
        public void SwitchHeroToSeatDic(int seat1, int seat2)
        {
            int heroId1 = HeroSeatDict[seat1];
            int heroId2 = HeroSeatDict[seat2];
            HeroSeatDict[seat1] = heroId2;
            HeroSeatDict[seat2] = heroId1;
            HeroSeatDicToString();
        }

        /// <summary>
        ///  获取空闲位置数量
        /// </summary>
        /// <returns></returns>
        public int GetNullSeatCount()
        {
            int nullCount = 0;
            foreach (var keyValuePair in HeroSeatDict)
            {
                if (keyValuePair.Value == 0)
                {
                    nullCount++;
                }
            }
            return nullCount;
        }

        public void ClearHeroSeatDic()
        {
            for (var index = 0; index < HeroSeatDict.Count; index++)
            { 
                HeroSeatDict[index] = 0;
            }
        }

        public void HeroSeatDicToString()
        {
            string heroSeatDicStr = "";
            foreach (var item in HeroSeatDict)
            {
                heroSeatDicStr += item.Key.ToString() + ":" + item.Value.ToString() + "|";
            }
            Debugger.Log($"HeroSeat:{heroSeatDicStr}");
        }
    }
}