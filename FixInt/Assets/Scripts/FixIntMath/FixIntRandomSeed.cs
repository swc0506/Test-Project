/*----------------------------------------------------------------------------
* Title: 定点数随机种子随机数
*
* Author: 铸梦
*
* Date: 2024.12.29
*
* Description:可应用于状态同步或帧同步中，主要解决不同平台下float精度误差问题，保证不同平台在相同属于源的情况下，结果计算的一致性
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/
namespace ZM.FixIntMath
{
    using System;
    /// <summary>
    /// 定点数随机种子随机数
    /// </summary>
    public class FixIntRandomSeed
    {
        /// <summary>
        /// 随机种子
        /// </summary>
        public int SeedId { get; private set; }
        /// <summary>
        /// 随机数生成器
        /// </summary>
        private Random mRandomGenerator;
        /// <summary>
        /// 定点数随机种子随即器
        /// </summary>
        /// <param name="seedId">随机种子</param>
        public FixIntRandomSeed(int seedId)
        {
            this.SeedId = seedId;
            this.mRandomGenerator = new Random(seedId);
        }
        /// <summary>
        /// 在最小值min和最大值max之间随机一个数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public int Range(int min, int max)
        {
            return mRandomGenerator.Next(min, max);
        }
        /// <summary>
        /// 在最小值min和最大值max之间随机一个数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public int Range(FixInt min, FixInt max)
        {
            return mRandomGenerator.Next(min.IntValue, max.IntValue) / FixInt.MUTIPLE;
        }
    }
}