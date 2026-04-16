using System;
using System.Collections.Generic;
using Core.FS;

namespace Core.Config
{
    public class ConfigManager : BaseConfigManager<ConfigManager, ConfigGroup>
    {
        protected override ConfigGroup CreateDefaultConfigGroup()
        {
            return CrateConfigGroup(string.Empty);
        }

        protected override ConfigGroup CrateConfigGroup(string name)
        {
            return new ConfigGroup();
        }

        public bool EnableLazyLoad
        {
            set { def.EnableLazyLoad = value; }
        }

        public void SetAssetPackage(AssetPackage assetPkg, string prefixPath = null)
        {
            def.SetAssetPackage(assetPkg, prefixPath);
        }

        public void SetAssetPackageName(string pkgName, string prefixPath = null)
        {
            def.SetAssetPackageName(pkgName, prefixPath);
        }

        public void SetFileLoadDir(string fileLoadDir)
        {
            def.SetFileLoadDir(fileLoadDir);
        }

        public void LoadConfig(string name, Type type)
        {
            def.LoadConfig(name, type);
        }

        public void LoadConfig(Type type)
        {
            def.LoadConfig(type);
        }

        public void LoadConfig<T>()
        {
            def.LoadConfig<T>();
        }

        public void LoadConfigs(Dictionary<string, Type> configs)
        {
            def.LoadConfigs(configs);
        }

        public void LoadConfigs(IEnumerable<Type> types)
        {
            def.LoadConfigs(types);
        }

        public void LoadConfigAsync(string name, Type type, LoadConfigAction completed)
        {
            def.LoadConfigAsync(name, type, completed);
        }

        public void LoadConfigsAsync(Dictionary<string, Type> configs, Action<bool> completed)
        {
            def.LoadConfigsAsync(configs, completed);
        }

        public void LoadConfigsAsync(IEnumerable<Type> types, Action<bool> completed)
        {
            def.LoadConfigsAsync(types, completed);
        }
    }
}