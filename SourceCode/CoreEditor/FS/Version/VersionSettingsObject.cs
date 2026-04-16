using System;
using System.Collections.Generic;
using Core.FS;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoreEditor.FS
{
    public class VersionSettingsObject : SettingsObject
    {
        public List<string> extraProjectsAsset;
        public DiffType diffType;
        public int rangeCount;
        public List<PackageInfo> packages;

        protected override void OnInitial()
        {
            extraProjectsAsset = new List<string>();
            packages = new List<PackageInfo>();
            rangeCount = 80;
        }

        public static VersionSettingsObject Get()
        {
            return Load<VersionSettingsObject>("Version");
        }

        public ChannelInfo FindChannelInfo(string platformName, string channelName)
        {
            if (string.IsNullOrEmpty(platformName))
            {
                Debug.LogWarningFormat("PackageInfo platform is null");
                return null;
            }

            PackageInfo pkgInfo = null;
            foreach (var item in packages)
            {
                if (item.platform == platformName)
                {
                    pkgInfo = item;
                    break;
                }
            }

            if (null == pkgInfo)
            {
                Debug.LogWarningFormat("PackageInfo platform:{0} dont exist", platformName);
                return null;
            }

            foreach (var info in pkgInfo.channels)
            {
                if (info.channel == channelName)
                {
                    return info;
                }
            }

            Debug.LogWarningFormat("AppInfo platform:{0} channel:{1} dont exist", platformName, channelName);
            return null;
        }
    }

    [Serializable]
    public class PackageInfo
    {
        public string platform;
        public List<ChannelInfo> channels;
    }

    [Serializable]
    public class ChannelInfo
    {
        public string channel;
        public VersionNum appVersion;
        public VersionNum assetVersion;

        public List<string> remoteUrls;
        public List<VersionNum> assetVersions;

        public ChannelInfo(string channel)
        {
            this.channel = channel;
            this.appVersion = new VersionNum(1, 0, 0);
            this.assetVersion = new VersionNum(1, 0, 0);
            
            remoteUrls = new List<string>();
            assetVersions = new List<VersionNum>();
        }
    }
}