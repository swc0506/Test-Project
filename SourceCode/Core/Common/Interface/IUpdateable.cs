namespace Core
{
    /// <summary>
    ///  更新
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime">流失的时间,单位秒</param>
        void Update(float deltaTime);
    }
}