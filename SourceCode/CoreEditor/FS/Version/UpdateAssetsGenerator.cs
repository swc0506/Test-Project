using Core;

namespace CoreEditor.FS
{
    public class UpdateAssetsGenerator
    {
        public static string Generate(string platform, string channel)
        {
            string assetDir = AssetBundleUtils.VersionAssetsTempPath(channel);
            //prepare assets
            PackageAssetsHandler.EncryptPackageAssetBundles(assetDir);
            //compare diff
            VersionDiffPackage diffPackage = new VersionDiffPackage(assetDir, platform, channel);
            diffPackage.GenerateDiff();
            //delete temp
            FileUtils.DeleteDirectory(assetDir);

            return diffPackage.DiffPath;
        }

        public static string Generate(string channel)
        {
            string platform = BuildPlatformPath.GetBuildPlatform();
            return Generate(platform, channel);
        }
    }
}