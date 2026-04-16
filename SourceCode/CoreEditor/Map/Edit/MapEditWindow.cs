#if MAP
using System.Collections.Generic;
using System.IO;
using Core.Map;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Map
{
    public class MapEditWindow : EditWindow<MapGridPainter>
    {
        private const string EDIT_SCENE_UNIT_FOLDOUT_KEY = "Map.EDIT_SCENE_UNIT_FOLDOUT_KEY";
        private const string SELECT_EDIT_THEME_KEY = "Map.SELECT_EDIT_THEME_KEY";

        private MapConfigObject config;
        private bool editClassifyFoldout;
        private Vector2 drawPartPos;

        private string inputThemeName;
        private string[] themeNames;
        private int selectThemeIndex;
        private ThemeInfo selectTheme;

        private string inputGroupName;
        private List<Vector2> drawGroupsPos;
        private Object addObject;
        private PartType selectPartType;
        private string currGroupName;
        private string currPartPath;

        [MenuItem("Tools/Map/Map Window", false, 5081)]
        public static void ShowWindow()
        {
            MapEditWindow window = GetWindow<MapEditWindow>("Map Window");
            window.minSize = new Vector2(400, 300);
        }

        protected override void OnEnable()
        {
            editType = "Map";
            base.OnEnable();
            config = MapConfigObject.Get();
            editClassifyFoldout = EditorPrefs.GetBool(EDIT_SCENE_UNIT_FOLDOUT_KEY, true);
            inputThemeName = config.themes.Count == 0 ? "Common" : string.Empty;
            drawGroupsPos = new List<Vector2>();
            selectPartType = PartType.Ground;
            CollectThemeNames();
        }

        protected override void OnDrawEdit()
        {
        }

        protected override void OnDrawGUI()
        {
            EditorGUI.BeginChangeCheck();
            editClassifyFoldout = EditorGUILayout.Foldout(editClassifyFoldout, "Edit Map Parts");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(EDIT_SCENE_UNIT_FOLDOUT_KEY, editClassifyFoldout);
            }

            if (editClassifyFoldout)
            {
                DrawPartTheme();
            }

            if (null != selectTheme)
            {
                EditorGUILayout.Space(5);
                drawPartPos = EditorGUILayout.BeginScrollView(drawPartPos,
                    GUILayout.MinHeight(200), GUILayout.MaxHeight(600));
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawGroupsInfo();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }

        protected override void OnEditGrid(Vector3 pos)
        {
            if (string.IsNullOrEmpty(currPartPath))
            {
                ShowNotification(new GUIContent("Please select part first!"));
                return;
            }

            MapNode node = painter.MapGrid.GetNode(pos);
            if (null != node)
            {
                if (!isRetreat)
                {
                    if (selectPartType == PartType.Ground)
                    {
                        if (null == node.ground)
                        {
                            node.ground = new MapPart(node, selectPartType);
                        }

                        node.ground.SetPath(currPartPath, currGroupName);
                    }
                    else if (selectPartType == PartType.Surface)
                    {
                        if (null == node.surface)
                        {
                            node.surface = new MapPart(node, selectPartType);
                        }

                        node.surface.SetPath(currPartPath, currGroupName);
                    }
                }
                else
                {
                    if (selectPartType == PartType.Ground)
                    {
                        node.ground?.SetPath(null, null);
                        node.ground = null;
                    }
                    else if (selectPartType == PartType.Surface)
                    {
                        node.surface?.SetPath(null, null);
                        node.surface = null;
                    }
                }
            }
        }

        protected override void OnClickGrid(Vector3 pos)
        {
            if (null != painter)
            {
                MapNode node = painter.MapGrid.GetNode(pos);
                if (null != node)
                {
                    MapPart mapPart = null;
                    if (selectPartType == PartType.Ground)
                    {
                        mapPart = node.ground;
                    }
                    else if (selectPartType == PartType.Surface)
                    {
                        mapPart = node.surface;
                    }

                    if (null != mapPart && null != mapPart.Part)
                    {
                        Selection.activeObject = mapPart.Part;
                        UnityEditor.Tools.current = Tool.Rotate;
                    }
                }
            }
        }

        private void DrawPartTheme()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            {
                inputThemeName = EditorGUILayout.TextField("Create Theme:", inputThemeName);
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(inputThemeName));
                if (GUILayout.Button("+", GUILayout.Width(40)))
                {
                    TryCreateTheme();
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                selectThemeIndex = EditorGUILayout.Popup("Select Theme:", selectThemeIndex, themeNames);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(SELECT_EDIT_THEME_KEY, themeNames[selectThemeIndex]);
                    SetSelectTheme();
                }

                EditorGUI.BeginDisabledGroup(null == selectTheme);
                {
                    if (GUILayout.Button("-", GUILayout.Width(40)))
                    {
                        if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this theme?", "Yes",
                            "No"))
                        {
                            config.themes.RemoveAt(selectThemeIndex);
                            config.Save();
                            CollectThemeNames();
                        }
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }

            selectPartType = (PartType) EditorGUILayout.EnumPopup("Select Part Type:", selectPartType);

            EditorGUILayout.EndVertical();
        }

        private ThemeInfo FindTheme(string themeName)
        {
            foreach (var item in config.themes)
            {
                if (item.name == themeName)
                {
                    return item;
                }
            }

            return null;
        }

        private void TryCreateTheme()
        {
            inputThemeName = inputThemeName.Trim();
            if (null != FindTheme(inputThemeName))
            {
                string tip = string.Format("Theme name of:{0} already exist", inputThemeName);
                ShowNotification(new GUIContent(tip));
                return;
            }

            ThemeInfo info = new ThemeInfo();
            info.name = inputThemeName;
            info.groups = new List<GroupInfo>();
            config.themes.Add(info);
            config.Save();
            inputThemeName = string.Empty;
            GUI.FocusControl(null);
            CollectThemeNames();
            ShowNotification(new GUIContent("Theme create success"));
        }

        private void CollectThemeNames()
        {
            string selectThemeName = EditorPrefs.GetString(SELECT_EDIT_THEME_KEY);
            selectThemeIndex = 0;
            themeNames = new string[config.themes.Count];
            for (int i = 0; i < themeNames.Length; i++)
            {
                themeNames[i] = config.themes[i].name;
                if (selectThemeName == themeNames[i])
                {
                    selectThemeIndex = i;
                }
            }

            SetSelectTheme();
        }

        private void SetSelectTheme()
        {
            if (selectThemeIndex >= 0 && selectThemeIndex < config.themes.Count)
            {
                selectTheme = config.themes[selectThemeIndex];
                for (int i = 0; i < selectTheme.groups.Count; i++)
                {
                    drawGroupsPos.Add(Vector2.zero);
                }
            }
            else
            {
                selectTheme = null;
                drawGroupsPos.Clear();
            }
        }

        private GroupInfo FindGroup(string groupName)
        {
            foreach (var item in selectTheme.groups)
            {
                if (item.name == groupName)
                {
                    return item;
                }
            }

            return null;
        }

        private void TryCreateGroup()
        {
            inputGroupName = inputGroupName.Trim();
            if (null != FindGroup(inputGroupName))
            {
                string tip = string.Format("Group name of:{0} already exist", inputGroupName);
                ShowNotification(new GUIContent(tip));
                return;
            }

            GroupInfo info = new GroupInfo();
            info.name = inputGroupName;
            info.partType = (int) selectPartType;
            info.partPaths = new List<string>();
            selectTheme.groups.Add(info);
            drawGroupsPos.Add(Vector2.zero);

            config.Save();
            inputGroupName = string.Empty;
            GUI.FocusControl(null);
            ShowNotification(new GUIContent("Group create success"));
        }

        private void DrawGroupsInfo()
        {
            EditorGUILayout.BeginHorizontal();
            {
                inputGroupName = EditorGUILayout.TextField("Create Group:", inputGroupName);
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(inputGroupName));
                if (GUILayout.Button("+", GUILayout.Width(40)))
                {
                    TryCreateGroup();
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            for (int i = 0; i < selectTheme.groups.Count; i++)
            {
                if (selectTheme.groups[i].partType == (int) selectPartType)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    bool res = DrawGroupInfo(i);
                    EditorGUILayout.EndVertical();
                    if (!res)
                    {
                        break;
                    }

                    EditorGUILayout.Space(5);
                }
            }
        }

        private bool DrawGroupInfo(int i)
        {
            GroupInfo item = selectTheme.groups[i];
            item.foldout = EditorGUILayout.Foldout(item.foldout, item.name);
            if (!item.foldout)
            {
                return true;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this group?", "Yes",
                        "No"))
                    {
                        selectTheme.groups.RemoveAt(i);
                        drawGroupsPos.RemoveAt(i);
                        config.Save();
                        return false;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(6);
                EditorGUI.BeginChangeCheck();
                addObject = EditorGUILayout.ObjectField(addObject, typeof(GameObject), false,
                    GUILayout.ExpandWidth(true), GUILayout.Height(45));
                if (EditorGUI.EndChangeCheck())
                {
                    addObject = null;
                    bool refreshParts = false;
                    foreach (var go in Selection.gameObjects)
                    {
                        string path = AssetDatabase.GetAssetPath(go);
                        string guid = AssetDatabase.AssetPathToGUID(path);
                        if (TryAddGroupPart(item, guid))
                        {
                            refreshParts = true;
                        }
                    }

                    if (refreshParts)
                    {
                        config.Save();
                        return false;
                    }
                }

                GUILayout.Space(6);
                EditorGUILayout.EndHorizontal();
            }

            if (item.partPaths.Count > 0)
            {
                drawGroupsPos[i] = EditorGUILayout.BeginScrollView(drawGroupsPos[i], GUILayout.Height(108));
                DrawGroupParts(item);
                EditorGUILayout.EndScrollView();
            }

            return true;
        }

        private bool TryAddGroupPart(GroupInfo item, string guid)
        {
            if (item.partPaths.Contains(guid))
            {
                ShowNotification(new GUIContent("This Part Already Contains"));
                return false;
            }

            item.partPaths.Add(guid);
            return true;
        }

        private void DrawGroupParts(GroupInfo item)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < item.partPaths.Count; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(item.partPaths[i]);
                Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (null == asset)
                {
                    continue;
                }

                Texture texture = AssetPreview.GetAssetPreview(asset);
                string name = Path.GetFileNameWithoutExtension(path);
                GUIContent guiContent = new GUIContent(texture, path);
                bool isCurr = item.name == currGroupName && path == currPartPath;
                GUI.color = isCurr ? Color.cyan : Color.white;

                EditorGUILayout.BeginVertical(GUILayout.Width(70));
                {
                    var centeredStyle = GUI.skin.label;
                    centeredStyle.alignment = TextAnchor.UpperCenter;
                    GUILayout.Label(name, centeredStyle);
                    if (GUILayout.Button(guiContent, GUILayout.Width(70), GUILayout.Height(70)))
                    {
                        SetSelectPart(item.name, path);
                    }

                    EditorGUILayout.EndVertical();
                }

                if (isCurr && GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this part?", "Yes",
                        "No"))
                    {
                        item.partPaths.RemoveAt(i);
                        int length = item.partPaths.Count;
                        if (length > 0)
                        {
                            int newIndex = i == length ? 0 : i;
                            string newPath = AssetDatabase.GUIDToAssetPath(item.partPaths[newIndex]);
                            SetSelectPart(item.name, newPath);
                        }
                        else
                        {
                            SetSelectPart(null, null);
                        }

                        break;
                    }
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void SetSelectPart(string groupName, string path)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                currGroupName = groupName;
                currPartPath = path;
            }
            else
            {
                currGroupName = null;
                currPartPath = null;
            }
        }
    }
}
#endif