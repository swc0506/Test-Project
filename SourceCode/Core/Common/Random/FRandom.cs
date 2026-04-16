namespace Core
{
    public interface FRandom
    {
        void SetSeed(int seed);

        /// <summary>
        /// 随机获取一个数字
        /// </summary>
        /// <returns></returns>
        double Next();

        /// <summary>
        /// 随机一个整数 [min,max)
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        int NextInt(int min, int max);

        /// <summary>
        /// 随机命中
        /// </summary>
        /// <param name="probability">概率</param>
        /// <param name="factor">系数/百分比/千分比/万分比等</param>
        /// <returns></returns>
        bool NextBool(int probability, int factor);
    }
}