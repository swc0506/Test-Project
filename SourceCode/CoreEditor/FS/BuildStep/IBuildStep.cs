namespace CoreEditor.FS
{
    /// <summary>
    /// 打包指令步骤
    /// </summary>
    public interface IBuildStep
    {
        /// <summary>
        /// 执行步骤
        /// </summary>
        /// <param name="pkg"></param>
        void Execute(AssetPackage pkg);
    }
}