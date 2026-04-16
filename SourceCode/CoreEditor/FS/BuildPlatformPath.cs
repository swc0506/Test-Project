using UnityEditor;

public class BuildPlatformPath
{
    private static string KEY_STANDALONE = "Standalone";
    private static string KEY_64 = "64";

    public static string GetBuildPlatform(BuildTarget buildTarget)
    {
        string platformName = buildTarget.ToString();
        if (platformName.StartsWith(KEY_STANDALONE))
        {
            platformName = platformName.Substring(KEY_STANDALONE.Length);
        }

        if (platformName.EndsWith(KEY_64))
        {
            platformName = platformName.Remove(platformName.Length - KEY_64.Length);
        }
        return platformName;
    }

    public static string GetBuildPlatform()
    {
        return GetBuildPlatform(EditorUserBuildSettings.activeBuildTarget);
    }
}