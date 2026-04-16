#if I18N
using Core.I18n;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.I18N
{
    internal static class LanguagePackageHelper
    {
        public static LanguagePackage Load(string languageName)
        {
            if (!string.IsNullOrEmpty(languageName))
            {
                string path = string.Format("Assets/{0}/{1}.json", I18NSettingsObject.Get().il8nPath, languageName);
                TextAsset jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (null != jsonFile && !string.IsNullOrEmpty(jsonFile.text))
                {
                    return JsonUtility.FromJson<LanguagePackage>(jsonFile.text);
                }
            }

            return null;
        }

        public static LanguagePackage LoadCurrent()
        {
            string selectCultureName = EditorPrefs.GetString(I18nWindow.SELECT_EDIT_CULTURE_KEY);
            return Load(selectCultureName);
        }

        public static void Save(LanguagePackage pkg)
        {
            string json = JsonUtility.ToJson(pkg, true);
            string savePath = string.Format("{0}/{1}/{2}.json", Application.dataPath, I18NSettingsObject.Get().il8nPath,
                pkg.name);
            Core.FileUtils.CreateFile(savePath, json);
        }
    }
}
#endif