#if I18N
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.I18N
{
    public sealed partial class I18nWindow : TabWindow
    {
        private CultureInfo[] cultureInfos;
        private I18NSettingsObject config;
        private string[] pkgFileNames;

        [MenuItem("Tools/I18n Window &l",false, 5051)]
        public static void ShowWindow()
        {
            OpenWindow<I18nWindow>("I18N Window");
        }

        protected override string SelectTabIndexKey
        {
            get { return "I18n.Select_Tab_Index_Key"; }
        }

        protected override void OnInitial()
        {
            windows = new BaseWindow[]
            {
                new CreateWindow(),
                new EditWindow(),
                new TranslateWindow(),
            };
            cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            config = I18NSettingsObject.Get();
            FindTextPackages();
        }

        protected override void OnSelectTabChanged()
        {
            base.OnSelectTabChanged();
            GUI.FocusControl(null);
        }

        private bool Il8nPathNull
        {
            get { return string.IsNullOrEmpty(config.il8nPath); }
        }

        protected override void OnDrawGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    string displayPath = Il8nPathNull ? string.Empty : "Assets/" + config.il8nPath;
                    EditorGUILayout.TextField("Il8n Save Directory Is:", displayPath);
                    if (GUILayout.Button("Select Directory", GUILayout.MaxWidth(120)))
                    {
                        SelectSaveDirectory();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (Il8nPathNull)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Il8n Save Directory Is Null", MessageType.Error);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    config.useTextLocalized = EditorGUILayout.Toggle("Use TextLocalized:", config.useTextLocalized);
                    if (EditorGUI.EndChangeCheck())
                    {
                        config.Save();
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void SelectSaveDirectory()
        {
            string dataPath = Application.dataPath;
            string srcPath = dataPath + "/" + config.il8nPath;
            string newPath = EditorUtility.OpenFolderPanel("Il8n Save Directory", srcPath, "");
            if (!string.IsNullOrEmpty(newPath) && newPath != srcPath)
            {
                int index = newPath.IndexOf(dataPath);
                if (index >= 0)
                {
                    config.il8nPath = newPath.Substring(index + dataPath.Length + 1);
                    config.Save();
                    FindTextPackages();
                }
                else
                {
                    ShowNotification(new GUIContent(string.Format("il8nPath need below:{0}", dataPath)));
                }
            }
        }

        private void FindTextPackages()
        {
            List<string> pkg = new List<string>();
            if (!string.IsNullOrEmpty(config.il8nPath))
            {
                for (int i = 0; i < cultureInfos.Length; i++)
                {
                    string path = string.Format("Assets/{0}/{1}.json", config.il8nPath, cultureInfos[i].Name);
                    if (Core.FileUtils.ExistsFile(path))
                    {
                        pkg.Add(cultureInfos[i].Name);
                    }
                }
            }

            pkgFileNames = pkg.ToArray();
        }
    }
}
#endif