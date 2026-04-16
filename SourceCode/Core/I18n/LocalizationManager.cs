using System;
using System.Collections.Generic;
using Core.Config;
using Core.FS;
using UnityEngine;

namespace Core.I18N
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        private const string PREFS_KEY = "LocalizationManager.Language";

        private readonly ConfigGroup lan = new ConfigGroup();

        public IEnumerable<LanguageInfo> Languages { get; private set; }
        public LanguageInfo Language { get; private set; }


        private Func<string> languageFunc;
        private Func<string> fallbackFunc;

        private Action<bool> completed;

        private Action changedEvent;

        public event Action ChangedEvent
        {
            add { changedEvent += value; }
            remove { changedEvent -= value; }
        }

        private readonly Dictionary<string, string> valueMap = new Dictionary<string, string>();


        public void SetAssetPackage(AssetPackage assetPkg, string prefixPath = null)
        {
            lan.SetAssetPackage(assetPkg, prefixPath);
        }

        public void SetAssetPackageName(string pkgName, string prefixPath = null)
        {
            lan.SetAssetPackageName(pkgName, prefixPath);
        }

        public void RegisterLanguages(IEnumerable<LanguageInfo> languages)
        {
            this.Languages = languages;
        }

        #region Language

        public void SetLanguageFunc(Func<string> func)
        {
            this.languageFunc = func;
        }

        public void SetFallbackFunc(Func<string> func)
        {
            this.fallbackFunc = func;
        }

        private bool FindByName(string name, out LanguageInfo info)
        {
            foreach (var item in Languages)
            {
                if (item.Name == name)
                {
                    info = item;
                    return true;
                }
            }

            info = LanguageInfo.Empty;
            return false;
        }

        public string GetDefaultLanguage()
        {
            string lan = LocalStorage.GetString(PREFS_KEY);
            if (string.IsNullOrEmpty(lan))
            {
                if (null != languageFunc)
                {
                    lan = languageFunc.Invoke();
                }
                else
                {
                    lan = GetLanguageFromSystem();
                }
            }

            if (string.IsNullOrEmpty(lan) && null != fallbackFunc)
            {
                lan = fallbackFunc?.Invoke();
            }

            if (string.IsNullOrEmpty(lan))
            {
                lan = "en";
            }

            return lan;
        }

        private string GetLanguageFromSystem()
        {
            SystemLanguage systemLanguage = Application.systemLanguage;
            switch (systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    return "zh-CN";
                case SystemLanguage.ChineseTraditional:
                    return "zh-TW";
                case SystemLanguage.English:
                    return "en";
                case SystemLanguage.Russian:
                    return "ru";
                case SystemLanguage.Vietnamese:
                    return "vn";
                case SystemLanguage.Korean:
                    return "kr";
                case SystemLanguage.Japanese:
                    return "jp";
                case SystemLanguage.French:
                    return "fr";
                case SystemLanguage.Portuguese:
                    return "pt";
            }

            return null;
        }

        public bool SetLanguage(string language)
        {
            if (!string.IsNullOrEmpty(language) && Language.Name != language && FindByName(language, out var info))
            {
                Language = info;
                lan.UnloadConfig(language);
                lan.LoadConfig(language, typeof(LanguagePackage));
                OnChangeLanguage();
                return true;
            }

            return false;
        }

        public bool SetLanguage()
        {
            return SetLanguage(GetDefaultLanguage());
        }

        public bool SetLanguageAsync(string language, Action<bool> completed)
        {
            if (!string.IsNullOrEmpty(language) && Language.Name != language && FindByName(language, out var info))
            {
                Language = info;
                lan.UnloadConfig(language);

                this.completed = completed;
                lan.LoadConfigAsync(language, typeof(LanguagePackage), OnLoadCompleted);
                return true;
            }

            completed?.Invoke(false);
            return false;
        }

        private void OnLoadCompleted(string name, Type type, bool result)
        {
            OnChangeLanguage();
            completed?.Invoke(result);
        }

        public bool SetLanguageAsync(Action<bool> completed)
        {
            return SetLanguageAsync(GetDefaultLanguage(), completed);
        }

        private void OnChangeLanguage()
        {
            LocalStorage.SetString(PREFS_KEY, Language.Name);
            valueMap.Clear();
            changedEvent?.Invoke();
        }

        #endregion

        #region Text

        private bool TryFindValue(string key, out string value)
        {
            if (valueMap.Count == 0)
            {
                if (lan.TryGetConfig<LanguagePackage>(Language.Name, out var config))
                {
                    config.ForEach((ikey, iValue) => { valueMap[ikey] = iValue; });
                }
            }

            return valueMap.TryGetValue(key, out value);
        }

        public bool TryGetText(string key, out string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return TryFindValue(key, out value);
            }

            value = null;
            return false;
        }

        public string GetText(string key, params object[] args)
        {
            if (TryGetText(key, out var value))
            {
                if (null != args && args.Length > 0)
                {
                    return string.Format(value, args);
                }
                else
                {
                    return value;
                }
            }

            return key;
        }

        #endregion
    }
}