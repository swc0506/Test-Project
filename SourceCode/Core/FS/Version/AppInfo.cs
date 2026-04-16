using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.FS
{
    public class AppBaseInfo
    {
        public string channel;
        public List<string> remoteUrls;
        public string appVersion;
        public string assetVersion;
    }

    public class AppInfo : AppBaseInfo
    {
        private static AppInfo instance;

        public static AppInfo Instance
        {
            get { return instance; }
        }

        public static void SetInstance(AppInfo instance)
        {
            AppInfo.instance = instance;
        }


        public const string NAME = "AppInfo.json";
        public const string PATH = "Assets/Res/App/" + NAME;

        public VersionNum appVer;
        public VersionNum assetVer;
        public VersionNum updateAssetVer;

        public VersionNum CurrAssetVer
        {
            get { return updateAssetVer == VersionNum.zero ? assetVer : updateAssetVer; }
        }

        public void Parse()
        {
            appVer = new VersionNum(appVersion);
            assetVer = new VersionNum(assetVersion);
        }
    }
}