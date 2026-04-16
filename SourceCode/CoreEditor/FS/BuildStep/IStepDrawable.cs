namespace CoreEditor.FS
{
    /// <summary>
    /// 打包步骤图形绘制
    /// </summary>
    public interface IStepDrawable
    {
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="pkg">当前打的资源包</param>
        void Draw(AssetPackage pkg);
    }
}