using System;
using System.Collections.Generic;

namespace Core.Config
{
    public class CDNConfigManager : BaseConfigManager<CDNConfigManager, CDNConfigGroup>
    {
        protected override CDNConfigGroup CreateDefaultConfigGroup()
        {
            return CrateConfigGroup("CDNConfigs");
        }

        protected override CDNConfigGroup CrateConfigGroup(string name)
        {
            return new CDNConfigGroup(name);
        }

        public void SetRootUrls(IEnumerable<string> rootUrls)
        {
            def.SetRootUrls(rootUrls);
        }

        public void AddManifestInfo(CDNFileInfo info)
        {
            def.AddManifestInfo(info);
        }
        
        public void AddManifestInfos(IEnumerable<CDNFileInfo> infos)
        {
            def.AddManifestInfos(infos);
        }

        public void RemoveManifestInfo(string path)
        {
            def.RemoveManifestInfo(path);
        }

        public void RemoveManifestInfos(IEnumerable<string> paths)
        {
            def.RemoveManifestInfos(paths);
        }

        public Dictionary<string, CDNFileInfo>.Enumerator GetFileEnumerator()
        {
            return def.GetFileEnumerator();
        }
        
        public void AddConfig(string path, Type type, byte[] bytes)
        {
            def.AddConfig(path, type, bytes);
        }

        public void LoadConfig(string path, Type type, LoadConfigAction completed)
        {
            def.LoadConfig(path, type, completed);
        }

        public void LoadConfig(Type type, LoadConfigAction completed)
        {
            def.LoadConfig(type, completed);
        }

        public void LoadConfigs(Dictionary<string, Type> configs, Action<bool> completed)
        {
            def.LoadConfigs(configs, completed);
        }

        public void LoadConfigs(IEnumerable<Type> types, Action<bool> completed)
        {
            def.LoadConfigs(types, completed);
        }
    }
}