#if I18N
using System.Collections.Generic;
using System.Globalization;
using Core.I18n;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;
using TextInfo = Core.I18n.TextInfo;

namespace CoreEditor.I18N
{
    public partial class I18nWindow
    {
        private class CreateWindow : BaseWindow
        {
            private string[] cultureNames;
            private int selectCultureIndex;

            private int copyFromIndex;
            private int copyToIndex;

            public override string Title
            {
                get { return "CreateLanguage"; }
            }

            public override void Enable()
            {
                base.Enable();
                selectCultureIndex = 0;
                var cultureInfos = GetContext<I18nWindow>().cultureInfos;
                cultureNames = new string[cultureInfos.Length];
                for (int i = 0; i < cultureNames.Length; i++)
                {
                    cultureNames[i] = cultureInfos[i].EnglishName;
                    if (cultureInfos[i].EnglishName == CultureInfo.CurrentCulture.EnglishName)
                    {
                        selectCultureIndex = i;
                    }
                }
            }

            public override void DrawGUI()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.HelpBox(
                        string.Format("System Current Culture:{0}",
                            GetContext<I18nWindow>().cultureInfos[selectCultureIndex].EnglishName), MessageType.Info);
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        selectCultureIndex = EditorGUILayout.Popup("Select Culture:", selectCultureIndex,
                            cultureNames, EditorStyles.toolbarDropDown);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(4);
                    EditorGUILayout.TextField("Language Package Name:",
                        GetContext<I18nWindow>().cultureInfos[selectCultureIndex].Name);

                    if (DrawGreyButton("Create", GetContext<I18nWindow>().Il8nPathNull))
                    {
                        TryCreatePackage();
                    }

                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(30);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Copy Language Package TextInfos:");
                    GUILayout.Space(1);
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        copyFromIndex = EditorGUILayout.Popup("From:", copyFromIndex, GetContext<I18nWindow>().pkgFileNames,
                            EditorStyles.toolbarPopup);
                        copyToIndex = EditorGUILayout.Popup("To:", copyToIndex, GetContext<I18nWindow>().pkgFileNames,
                            EditorStyles.toolbarPopup);
                        EditorGUILayout.EndHorizontal();
                    }
                    if (DrawGreyButton("Copy", copyFromIndex == copyToIndex))
                    {
                        CopyPackageTextInfo(GetContext<I18nWindow>().pkgFileNames[copyFromIndex], GetContext<I18nWindow>().pkgFileNames[copyToIndex]);
                    }

                    EditorGUILayout.EndVertical();
                }
            }

            private bool DrawGreyButton(string text, bool disable)
            {
                EditorGUI.BeginDisabledGroup(disable);
                {
                    GUILayout.Space(10);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button(text, GUILayout.MinHeight(50)))
                    {
                        return true;
                    }

                    GUI.backgroundColor = Color.white;
                    EditorGUI.EndDisabledGroup();
                }
                return false;
            }

            private void TryCreatePackage()
            {
                string fileName = GetContext<I18nWindow>().cultureInfos[selectCultureIndex].Name;
                string savePath = string.Format("{0}/{1}/{2}.json", Application.dataPath, GetContext<I18nWindow>().config.il8nPath,
                    fileName);
                if (FileUtils.ExistsFile(savePath))
                {
                    string tip = string.Format("{0}.json File Is Exist,Do You Want Overlay?", fileName);
                    if (EditorUtility.DisplayDialog("Confirm", tip, "Yes", "No"))
                    {
                        CreatePackageFile(savePath);
                    }
                }
                else
                {
                    CreatePackageFile(savePath);
                }
            }

            private void CreatePackageFile(string savePath)
            {
                LanguagePackage pkg = new LanguagePackage();
                pkg.name = GetContext<I18nWindow>().cultureInfos[selectCultureIndex].Name;
                pkg.englishName = GetContext<I18nWindow>().cultureInfos[selectCultureIndex].EnglishName;
                string json = JsonUtility.ToJson(pkg, true);
                FileUtils.CreateFile(savePath, json);
                GetContext<I18nWindow>().FindTextPackages();
            }

            private void CopyPackageTextInfo(string srcPath, string desPath)
            {
                LanguagePackage srcPkg = LanguagePackageHelper.Load(srcPath);
                LanguagePackage desPkg = LanguagePackageHelper.Load(desPath);
                LanguagePackage pkg = new LanguagePackage();
                pkg.name = desPkg.name;
                pkg.englishName = desPkg.englishName;
                pkg.texts = new List<TextInfo>();
                foreach (var item in srcPkg.texts)
                {
                    TextInfo info = item;
                    TryCopyTextInfo(desPkg, item.key, ref info);
                    pkg.texts.Add(info);
                }

                LanguagePackageHelper.Save(pkg);
            }

            private void TryCopyTextInfo(LanguagePackage pkg, string key, ref TextInfo textInfo)
            {
                foreach (var item in pkg.texts)
                {
                    if (item.key == key)
                    {
                        textInfo = item;
                    }
                }
            }
        }
    }
}
#endif