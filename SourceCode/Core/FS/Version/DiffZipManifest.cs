using System.Collections.Generic;
using UnityEngine;

namespace Core.FS
{
    public class DiffZipManifest
    {
        public const string NAME = "DiffZipManifest.json";

        private readonly string defName;
        private readonly Dictionary<string, AssetFileInfo> infoMap = new Dictionary<string, AssetFileInfo>();

        public DiffZipManifest(string defName)
        {
            this.defName = defName;
        }

        public DiffZipManifest(string defName, string content)
        {
            this.defName = defName;
            if (!string.IsNullOrEmpty(content))
            {
                Dictionary<string, AssetFileInfo> map = JsonUtils.ToObject<Dictionary<string, AssetFileInfo>>(content);
                foreach (var item in map)
                {
                    infoMap.Add(item.Key, item.Value);
                }
            }
        }

        public void AddFileInfo(string name, AssetFileInfo info)
        {
            infoMap.Add(name, info);
        }

        public bool GetFileInfo(string name, out AssetFileInfo info)
        {
            if (infoMap.TryGetValue(name, out info))
            {
                return true;
            }

            if (infoMap.TryGetValue(defName, out info))
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return JsonUtils.ToJson(infoMap);
        }
    }
}