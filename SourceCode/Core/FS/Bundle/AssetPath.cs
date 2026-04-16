using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Core.FS
{
    public static class AssetPath
    {
        public const string ASSETBUNDLE_EXTENSION = ".ab";


        public static string UpdateAssetsPath
        {
            get { return string.Format("{0}/Assets", Application.persistentDataPath); }
        }

        public static string InternalAssetsPath
        {
            get
            {
                if (Application.isEditor)
                {
                    return string.Format("{0}/ExtraAssets/AssetPackages/Assets", Directory.GetCurrentDirectory());
                }
                else
                {
                    return string.Format("{0}/Assets", Application.streamingAssetsPath);
                }
            }
        }

        public static string DownloadPath
        {
            get { return string.Format("{0}/Download", Application.persistentDataPath); }
        }

        public static string GetBundleName(string value)
        {
            byte[] buff = Encoding.UTF8.GetBytes(value + FSConfig.Instance.Flag);
            buff = MD5.Create().ComputeHash(buff);
            uint hash = BitConverter.ToUInt32(buff, 0);
            return hash + ASSETBUNDLE_EXTENSION;
        }

        public static string GetAddressManifestBundleName(string pkgName)
        {
            return GetBundleName(pkgName + "_" + AddressManifest.NAME);
        }
    }
}