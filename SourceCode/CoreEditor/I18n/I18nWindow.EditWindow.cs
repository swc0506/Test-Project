#if I18N
using System.Collections.Generic;
using System.Globalization;
using Core.I18n;
using UnityEditor;
using UnityEngine;
using TextInfo = Core.I18n.TextInfo;

namespace CoreEditor.I18N
{
    public partial class I18nWindow
    {
        public const string SELECT_EDIT_CULTURE_KEY = "I18n.SELECT_EDIT_CULTURE_KEY";

        private class EditWindow : BaseWindow
        {
            private enum SearchType
            {
                Key,
                Value
            }

            private int selectCultureIndex;
            private LanguagePackage srcPkg;
            private LanguagePackage languagePkg;

            private string inputKey;
            private SearchType searchType;
            private string searchValue;
            private Vector2 scrollPos;
            private float totalViewHeight;

            public override string Title
            {
                get { return "EditLanguage"; }
            }

            public override void Enable()
            {
                base.Enable();
                selectCultureIndex = -1;
                int defCultureIndex = -1;
                string selectCultureName = EditorPrefs.GetString(SELECT_EDIT_CULTURE_KEY);
                var pkgFileNames = GetContext<I18nWindow>().pkgFileNames;
                for (int i = 0; i < pkgFileNames.Length; i++)
                {
                    if (selectCultureName == pkgFileNames[i])
                    {
                        selectCultureIndex = i;
                        break;
                    }

                    if (defCultureIndex == -1 && pkgFileNames[i] == CultureInfo.CurrentCulture.Name)
                    {
                        defCultureIndex = i;
                    }
                }

                if (selectCultureIndex == -1 && defCultureIndex >= 0)
                {
                    EditorPrefs.SetString(SELECT_EDIT_CULTURE_KEY, pkgFileNames[selectCultureIndex]);
                }

                LoadSelectPackage();
            }

            public override void DrawGUI()
            {
                if (selectCultureIndex == -1)
                {
                    EditorGUILayout.HelpBox("Language Packages File Don't Exist", MessageType.Error);
                    return;
                }

                DrawSelectCulture();
                GUILayout.Space(2);
                DrawCreateKey();
                GUILayout.Space(2);
                DrawTextInfos();
            }

            private void LoadSelectPackage()
            {
                if (selectCultureIndex >= 0)
                {
                    var pkgNames = GetContext<I18nWindow>().pkgFileNames;
                    srcPkg = LanguagePackageHelper.Load(pkgNames[selectCultureIndex]);
                    languagePkg = LanguagePackageHelper.Load(pkgNames[selectCultureIndex]);
                }
            }

            private void DrawSelectCulture()
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUI.BeginChangeCheck();
                    selectCultureIndex = EditorGUILayout.Popup("Select Culture:", selectCultureIndex,
                        GetContext<I18nWindow>().pkgFileNames, EditorStyles.toolbarPopup);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetString(SELECT_EDIT_CULTURE_KEY,
                            GetContext<I18nWindow>().pkgFileNames[selectCultureIndex]);
                        LoadSelectPackage();
                    }

                    EditorGUILayout.TextField(languagePkg.englishName, GUILayout.MaxWidth(160));
                    EditorGUILayout.EndHorizontal();
                }
            }

            private bool ContainsKey(string key)
            {
                foreach (var item in languagePkg.texts)
                {
                    if (item.key == key)
                    {
                        return true;
                    }
                }

                return false;
            }

            private void TryCreateKey(string key)
            {
                key = key.Trim();
                if (ContainsKey(key))
                {
                    string tip = string.Format("Language Package Already Exist:{0}", key);
                    GetContext<I18nWindow>().ShowNotification(new GUIContent(tip));
                    return;
                }

                TextInfo text = new TextInfo();
                text.key = key;
                languagePkg.texts.Add(text);
            }

            private void DrawCreateKey()
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    inputKey = EditorGUILayout.TextField("Create Key:", inputKey);
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(inputKey));
                    if (GUILayout.Button("+", GUILayout.Width(50)))
                    {
                        TryCreateKey(inputKey);
                        scrollPos.y = totalViewHeight;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }
            }

            private void DrawTextInfos()
            {
                GUILayout.Space(2);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                {
                    searchType =
                        (SearchType) EditorGUILayout.EnumPopup("Search Type:", searchType, GUILayout.MaxWidth(250));
                    searchValue = EditorGUILayout.TextField(searchValue, EditorStyles.toolbarTextField,
                        GUILayout.ExpandWidth(true));
                }
                EditorGUILayout.EndHorizontal();

                bool isChange = languagePkg.texts.Count != srcPkg.texts.Count;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                for (int i = 0; i < languagePkg.texts.Count; i++)
                {
                    TextInfo info = languagePkg.texts[i];
                    if (CheckFilter(info))
                    {
                        continue;
                    }

                    GUILayout.Space(2);
                    EditorGUILayout.BeginHorizontal();
                    {
                        info.key = EditorGUILayout.TextField(info.key);
                        GUILayout.FlexibleSpace();
                        info.value = EditorGUILayout.TextField(info.value);
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("-", GUILayout.Width(50)))
                        {
                            languagePkg.texts.RemoveAt(i);
                            break;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (!isChange)
                    {
                        if (info.key != srcPkg.texts[i].key || info.value != srcPkg.texts[i].value)
                        {
                            isChange = true;
                        }
                    }

                    languagePkg.texts[i] = info;
                }

                EditorGUILayout.EndScrollView();
                totalViewHeight = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUI.BeginDisabledGroup(!isChange);
                {
                    GUI.backgroundColor = !isChange ? Color.white : Color.green;
                    if (GUILayout.Button("Save", GUILayout.MinHeight(50)))
                    {
                        SavePackage();
                    }

                    GUI.backgroundColor = !isChange ? Color.white : Color.yellow;
                    if (GUILayout.Button("Revert", GUILayout.MinHeight(50)))
                    {
                        RevertLanguagePackage();
                    }

                    EditorGUI.EndDisabledGroup();
                    GUI.backgroundColor = Color.white;
                }
            }

            private bool CheckFilter(TextInfo info)
            {
                if (string.IsNullOrEmpty(searchValue))
                {
                    return false;
                }

                string val = searchType == SearchType.Key ? info.key : info.value;
                return !val.Contains(searchValue);
            }

            private void RevertLanguagePackage()
            {
                languagePkg.texts.Clear();
                languagePkg.texts.AddRange(srcPkg.texts);
            }

            private void SavePackage()
            {
                HashSet<string> mapSameKey = CheckSameKey();
                if (null != mapSameKey)
                {
                    string tip = "Save Fail,Because Same Key In Package:";
                    foreach (var item in mapSameKey)
                    {
                        tip += string.Format("[{0}]", item);
                    }

                    EditorUtility.DisplayDialog("Tip", tip, "Check");
                    return;
                }

                LanguagePackageHelper.Save(languagePkg);
                LoadSelectPackage();
            }

            private HashSet<string> CheckSameKey()
            {
                HashSet<string> mapSameKey = null;
                HashSet<string> mapKey = new HashSet<string>();
                foreach (var item in languagePkg.texts)
                {
                    if (!mapKey.Contains(item.key))
                    {
                        mapKey.Add(item.key);
                    }
                    else
                    {
                        if (null == mapSameKey)
                        {
                            mapSameKey = new HashSet<string>();
                        }

                        mapSameKey.Add(item.key);
                    }
                }

                return mapSameKey;
            }
        }
    }
}
#endif