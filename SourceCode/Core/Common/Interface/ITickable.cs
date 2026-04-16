namespace Core
{
    public interface ITickable
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Tick();

        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();
    }
}