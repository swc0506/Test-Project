using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreEditor.FS
{
    public partial class FSWindow
    {
        private class ConfigWindow : BaseWindow
        {
            private const string SELECT_EDIT_PACKAGE_KEY = "FS.SELECT_EDIT_PACKAGE_KEY";
            
            public override string Title
            {
                get { return "Config Assets"; }
            }

            private FSSettingsObject settings;
            private bool changedConfig;
          
            private string inputPkgName;
            private string[] pkgNames;
            private int selectPkgIndex;
            private AssetPackage selectPkg;

            private ReorderableList groupList;
            private string[] ruleNames;
            private string[] ruleDescribes;
            private BuildGroup selectGroup;
            private Rect dragRect;
            private DragListener dragListener;


            public override void Initial(TabWindow context)
            {
                base.Initial(context);
                groupList = new ReorderableList(null, typeof(BuildGroup));
                groupList.drawHeaderCallback = DrawOrderHead;
                groupList.drawElementCallback = DrawOrderElement;
                groupList.onAddCallback = OnOrderAddElement;
                groupList.onRemoveCallback = OnOrderRemoveElement;
                groupList.onSelectCallback = OnOrderSelectElement;
                groupList.onChangedCallback = OnOrderChangedElement;

                var ruleMap = AttributeUtils.GetTypeAttributeInfo<IBundleRule, BundleRuleAttribute>();
                var iter = ruleMap.GetEnumerator();
                ruleNames = new string[ruleMap.Count];
                ruleDescribes = new string[ruleMap.Count];
                int index = 0;
                while (iter.MoveNext())
                {
                    ruleNames[index] = iter.Current.Value.Name;
                    ruleDescribes[index] = iter.Current.Value.Describe;
                    index++;
                }

                dragListener = new DragListener();
                dragListener.onDragCallback = OnDragAssets;
                dragListener.onDragRectDrawCallback = OnDragRectDrawAssets;
            }

            public override void Enable()
            {
                base.Enable();
                settings = GetContext<FSWindow>().settings;
                inputPkgName = settings.packages.Count == 0 ? "AssetBundles" : string.Empty;
                CollectPackageNames();
            }

            public override void Disable()
            {
                base.Disable();
                if (changedConfig)
                {
                    settings.Save();
                    changedConfig = false;
                }
            }

            public override void DrawGUI()
            {
                base.DrawGUI();
                DrawCreateSelectPackage();
                GUILayout.Space(5);
                if (null != selectPkg)
                {
                    DrawBuildGroup();
                }
            }

            private void CollectPackageNames()
            {
                string selectPkgName = EditorPrefs.GetString(SELECT_EDIT_PACKAGE_KEY);
                selectPkgIndex = 0;
                pkgNames = new string[settings.packages.Count];
                for (int i = 0; i < pkgNames.Length; i++)
                {
                    pkgNames[i] = settings.packages[i].name;
                    if (selectPkgName == pkgNames[i])
                    {
                        selectPkgIndex = i;
                    }
                }

                SetSelectPackage();
            }

            private void SetSelectPackage()
            {
                if (selectPkgIndex >= 0 && selectPkgIndex < settings.packages.Count)
                {
                    selectPkg = settings.packages[selectPkgIndex];
                    groupList.list = selectPkg.groups;
                }
                else
                {
                    selectPkg = null;
                    groupList.list = null;
                }

                groupList.elementHeight = EditorGUIUtility.singleLineHeight;
                SetSelectGroupIndex(-1);
            }

            private void DrawCreateSelectPackage()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                {
                    inputPkgName = EditorGUILayout.TextField("Create Asset Package:", inputPkgName);
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(inputPkgName));
                    if (GUILayout.Button("+", GUILayout.Width(40)))
                    {
                        TryCreatePackage();
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    selectPkgIndex = EditorGUILayout.Popup("Select Asset Package:", selectPkgIndex, pkgNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetString(SELECT_EDIT_PACKAGE_KEY, pkgNames[selectPkgIndex]);
                        SetSelectPackage();
                    }

                    EditorGUI.BeginDisabledGroup(null == selectPkg);
                    {
                        if (GUILayout.Button("-", GUILayout.Width(40)))
                        {
                            if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this asset package?", "Yes",
                                "No"))
                            {
                                settings.packages.RemoveAt(selectPkgIndex);
                                settings.Save();
                                CollectPackageNames();
                            }
                        }

                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            private void TryCreatePackage()
            {
                inputPkgName = inputPkgName.Trim();
                if (null != GetContext<FSWindow>().FindPackage(inputPkgName))
                {
                    string tip = string.Format("Asset package name of:{0} already exist", inputPkgName);
                    GetContext<FSWindow>().ShowNotification(new GUIContent(tip));
                    return;
                }

                AssetPackage pkg = new AssetPackage();
                pkg.name = inputPkgName;
                settings.packages.Add(pkg);
                settings.Save();
                inputPkgName = string.Empty;
                GUI.FocusControl(null);
                CollectPackageNames();
                GetContext<FSWindow>().ShowNotification(new GUIContent("Build package create success"));
            }

            private void DrawBuildGroup()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    groupList.DoLayoutList();
                    string tip = null == selectGroup
                        ? string.Format("<size=14><b>Tip:please select build group config assets</b></size>")
                        : string.Format(
                            "<size=14><b>Tip:current select group is:{0},build assets order will by group index</b></size>",
                            selectGroup.name);
                    EditorGUILayout.LabelField(tip);
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);
                if (null != selectGroup)
                {
                    dragRect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(200));
                    dragListener.DrawRect(dragRect);
                    EditorGUILayout.EndVertical();
                }
            }

            private BuildGroup FindGroup(string name)
            {
                bool isNullOrEmpty = string.IsNullOrEmpty(name);
                foreach (var item in selectPkg.groups)
                {
                    if (isNullOrEmpty && string.IsNullOrEmpty(item.name))
                    {
                        return item;
                    }

                    if (item.name == name)
                    {
                        return item;
                    }
                }

                return null;
            }

            private void DrawOrderHead(Rect rect)
            {
                EditorGUI.LabelField(rect, "Build Groups");
            }

            private int FindRuleIndex(string rule)
            {
                for (int i = 0; i < ruleNames.Length; i++)
                {
                    if (ruleNames[i] == rule)
                    {
                        return i;
                    }
                }

                return -1;
            }

            private void SetSelectGroupIndex(int index)
            {
                groupList.index = index;
                RefreshSelectGroupIndex();
            }

            private void RefreshSelectGroupIndex()
            {
                if (null != groupList && groupList.index >= 0 && selectPkg.groups.Count > 0)
                {
                    selectGroup = selectPkg.groups[groupList.index];
                }
                else
                {
                    groupList.elementHeight = EditorGUIUtility.singleLineHeight;
                    selectGroup = null;
                }
            }

            private void DrawOrderElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                BuildGroup group = selectPkg.groups[index];
                rect.height = EditorGUIUtility.singleLineHeight;
                var groupRect = rect;
                groupRect.y += 5;
                groupRect.width = 100;
                EditorGUI.LabelField(groupRect, "Group Name:");
                groupRect.x += groupRect.width;
                groupRect.width = rect.width - groupRect.width;
                EditorGUI.BeginChangeCheck();
                string tempName = group.name;
                tempName = EditorGUI.TextField(groupRect, group.name);
                if (EditorGUI.EndChangeCheck())
                {
                    if (null == FindGroup(tempName))
                    {
                        group.name = tempName;
                        changedConfig = true;
                    }
                }

                var ruleRect = rect;
                ruleRect.y = groupRect.yMax + 5;
                ruleRect.width = 100;
                EditorGUI.LabelField(ruleRect, "Bundle Rule:");
                ruleRect.x += ruleRect.width;
                ruleRect.width = rect.width - ruleRect.width;
                EditorGUI.BeginChangeCheck();
                int ruleIndex = FindRuleIndex(group.rule);
                ruleIndex = EditorGUI.Popup(ruleRect, ruleIndex, ruleNames);
                if (EditorGUI.EndChangeCheck())
                {
                    group.rule = ruleNames[ruleIndex];
                    changedConfig = true;
                }

                var boxRect = rect;
                boxRect.y = ruleRect.yMax + 5;
                boxRect.height = rect.height * 2;
                if (ruleIndex >= 0 && ruleIndex < ruleDescribes.Length)
                {
                    EditorGUI.HelpBox(boxRect, ruleDescribes[ruleIndex], MessageType.Info);
                }
                else
                {
                    string tip = string.Format("Group rule name of:{0} is missing", group.rule);
                    EditorGUI.HelpBox(boxRect, tip, MessageType.Error);
                }

                groupList.elementHeight = boxRect.yMax - rect.yMin + 5;
            }

            private void OnOrderAddElement(ReorderableList list)
            {
                if (null == FindGroup(null))
                {
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                    int lastIndex = list.count - 1;
                    selectPkg.groups[lastIndex].rule = ruleNames.Length > 0 ? ruleNames[0] : null;
                    selectPkg.groups[lastIndex].enable = true;
                    SetSelectGroupIndex(lastIndex);
                    settings.Save();
                }
                else
                {
                    string tip = "Please set empty group name first";
                    GetContext<FSWindow>().ShowNotification(new GUIContent(tip));
                }
            }

            private void OnOrderRemoveElement(ReorderableList list)
            {
                if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this build group?", "Yes", "No"))
                {
                    bool isLast = list.index >= list.count - 1;
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    if (isLast)
                    {
                        SetSelectGroupIndex(list.count - 1);
                    }

                    settings.Save();
                }
            }

            private void OnOrderSelectElement(ReorderableList list)
            {
                RefreshSelectGroupIndex();
            }

            private void OnOrderChangedElement(ReorderableList list)
            {
                changedConfig = true;
            }

            private void OnDragAssets(Object[] assets)
            {
                int length = assets.Length;
                for (int i = 0; i < length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(assets[i]);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    if (!selectGroup.assetsGUID.Contains(guid))
                    {
                        selectGroup.assetsGUID.Add(guid);
                        settings.Save();
                    }
                }
            }

            private void OnDragRectDrawAssets()
            {
                GUIContent gc = new GUIContent();
                GUILayoutOption width = GUILayout.Width(20);
                GUILayoutOption height = GUILayout.Height(20);
                foreach (var item in selectGroup.assetsGUID)
                {
                    GUILayout.BeginHorizontal();
                    string path = AssetDatabase.GUIDToAssetPath(item);
                    Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                    if (null == asset)
                    {
                        GUI.contentColor = Color.gray;
                        gc.text = path + "  [this asset is missing]";
                    }
                    else
                    {
                        GUI.contentColor = Color.white;
                        gc.text = path;
                    }

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(EditorGUIUtils.GetAssetTypeIcon(asset), width, height);
                        if (GUILayout.Button(gc, EditorStyles.textField))
                        {
                            Selection.activeObject = asset;
                        }

                        GUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("-", GUILayout.Width(40)))
                    {
                        if (null == asset ||
                            EditorUtility.DisplayDialog("Tip", "Are you sure delete this assets?", "Yes", "No"))
                        {
                            selectGroup.assetsGUID.Remove(item);
                            settings.Save();
                            break;
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}