using System;
using System.Collections.Generic;
using System.IO;
using Core.FS;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using FileUtils = Core.FileUtils;

namespace CoreEditor.FS
{
    public partial class FSWindow
    {
        private class VersionWindow : BaseWindow
        {
            private const string EDIT_PACKAGE_FOLDOUT_KEY = "FS.EDIT_PACKAGE_FOLDOUT_KEY";
            private const string VIEW_ASSET_FOLDOUT_KEY = "FS.VIEW_ASSET_FOLDOUT_KEY";

            public override string Title
            {
                get { return "Update Assets"; }
            }

            private VersionSettingsObject settings;
            private string activePlatformName;

            private SerializedObject settingsSo;
            private ReorderableList extraProjectsList;

            private bool editPackageFoldout;
            private Vector2 drawPkgPos;
            private string[] platformNames;
            private int selectPlatformIndex;
            private PackageInfo selectPkgInfo;
            private string inputChannelName;
            private string[] selectPkgChannelNames;
            private int selectChannelIndex;
            private ChannelInfo selectChannelInfo;

            private ReorderableList remoteUrlsList;

            private bool viewAssetFoldout;
            private Vector2 drawAssetPos;
            private PackageInfo activePkgInfo;
            private string[] activeChannelNames;
            private int activeChannelIndex;
            private ChannelInfo activeChannelInfo;

            public override void Enable()
            {
                base.Enable();
                settings = VersionSettingsObject.Get();
                activePlatformName = BuildPlatformPath.GetBuildPlatform(EditorUserBuildSettings.activeBuildTarget);

                settingsSo = new SerializedObject(settings);
                extraProjectsList = new ReorderableList(settingsSo, settingsSo.FindProperty("extraProjectsAsset"));
                extraProjectsList.drawHeaderCallback = DrawOrderHead;
                extraProjectsList.drawElementCallback = DrawOrderElement;
                platformNames = GetContext<FSWindow>().CollectBuildPlatforms();

                activePkgInfo = FindPackageInfo(activePlatformName);
                activeChannelNames = FindChannelNames(activePkgInfo, out activeChannelIndex, out activeChannelInfo);

                selectPlatformIndex = Array.IndexOf(platformNames, activePlatformName);
                editPackageFoldout = EditorPrefs.GetBool(EDIT_PACKAGE_FOLDOUT_KEY, true);
                viewAssetFoldout = EditorPrefs.GetBool(VIEW_ASSET_FOLDOUT_KEY, true);

                RefreshSelectPackageInfo();
            }

            private void DrawOrderHead(Rect rect)
            {
                EditorGUI.LabelField(rect, "Extra Projects Asset");
            }

            private void DrawOrderElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                var element = extraProjectsList.serializedProperty.GetArrayElementAtIndex(index);
                string value = element.stringValue;
                rect.height = EditorGUIUtility.singleLineHeight;
                element.stringValue = GUI.TextField(rect, value);
            }

            private PackageInfo FindPackageInfo(string platformName)
            {
                foreach (var item in settings.packages)
                {
                    if (item.platform == platformName)
                    {
                        return item;
                    }
                }

                return null;
            }

            private string[] FindChannelNames(PackageInfo packageInfo, out int channelIndex,
                out ChannelInfo channelInfo)
            {
                if (null == packageInfo)
                {
                    channelIndex = 0;
                    channelInfo = null;
                    return null;
                }

                string[] channelNames = new string[packageInfo.channels.Count];
                for (int i = 0; i < channelNames.Length; i++)
                {
                    channelNames[i] = packageInfo.channels[i].channel;
                }

                if (channelNames.Length > 0)
                {
                    channelIndex = channelNames.Length - 1;
                    channelInfo = packageInfo.channels[channelIndex];
                }
                else
                {
                    channelIndex = -1;
                    channelInfo = null;
                }

                return channelNames;
            }

            private void RefreshSelectPackageInfo()
            {
                string selectPlatformName = platformNames[selectPlatformIndex];
                selectPkgInfo = FindPackageInfo(selectPlatformName);
                if (null == selectPkgInfo)
                {
                    selectPkgInfo = new PackageInfo();
                    selectPkgInfo.platform = selectPlatformName;
                    selectPkgInfo.channels = new List<ChannelInfo>();
                    settings.packages.Add(selectPkgInfo);
                    settings.Save();
                }

                selectPkgChannelNames =
                    FindChannelNames(selectPkgInfo, out selectChannelIndex, out selectChannelInfo);

                activePkgInfo = FindPackageInfo(activePlatformName);
                SetSelectChannel();
            }

            public override void DrawGUI()
            {
                base.DrawGUI();

                settingsSo.Update();
                extraProjectsList.DoLayoutList();

                EditorGUI.BeginChangeCheck();

                EditorGUI.BeginChangeCheck();
                editPackageFoldout = EditorGUILayout.Foldout(editPackageFoldout, "Edit Package Info");
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(EDIT_PACKAGE_FOLDOUT_KEY, editPackageFoldout);
                }

                if (editPackageFoldout)
                {
                    drawPkgPos = EditorGUILayout.BeginScrollView(drawPkgPos, GUILayout.MaxHeight(280));
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    DrawPackageInfo();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.Space(5);
                DrawDiffType();

                EditorGUILayout.Space(5);
                if (null != activeChannelInfo)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    DrawGenerateDiffPackage();

                    EditorGUI.BeginChangeCheck();
                    viewAssetFoldout = EditorGUILayout.Foldout(viewAssetFoldout, "Asset Versions");
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetBool(VIEW_ASSET_FOLDOUT_KEY, viewAssetFoldout);
                    }

                    if (viewAssetFoldout)
                    {
                        EditorGUILayout.Space(5);
                        drawAssetPos = EditorGUILayout.BeginScrollView(drawAssetPos);
                        DrawAssetVersions();
                        EditorGUILayout.EndScrollView();
                    }

                    EditorGUILayout.EndVertical();
                }

                settingsSo.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                {
                    settings.Save();
                }
            }

            private void DrawPackageInfo()
            {
                EditorGUI.BeginChangeCheck();
                selectPlatformIndex =
                    EditorGUILayout.Popup("Select Platform:", selectPlatformIndex, platformNames);
                if (EditorGUI.EndChangeCheck())
                {
                    RefreshSelectPackageInfo();
                }

                EditorGUILayout.Space(4);
                DrawAddChannel();

                if (null != selectPkgChannelNames && selectPkgChannelNames.Length > 0)
                {
                    EditorGUILayout.Space(2);
                    DrawEditChannel();
                }

                if (null != selectChannelInfo)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Create AppInfo File", GUILayout.Height(30)))
                        {
                            PackageAssetsHandler.CreateAppInfoFile(selectPkgInfo.platform, selectChannelInfo.channel);
                        }

                        GUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space(2);
                }
            }

            private void DrawAddChannel()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Add Channel:");
                EditorGUILayout.BeginHorizontal();
                {
                    inputChannelName = EditorGUILayout.TextField("Input Channel Name:", inputChannelName);
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(inputChannelName));
                    {
                        if (GUILayout.Button("+", GUILayout.Width(40)))
                        {
                            TryAddChannel();
                        }

                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            private void TryAddChannel()
            {
                inputChannelName = inputChannelName.Trim();
                if (null != FindChannelInfo(inputChannelName))
                {
                    string tip = string.Format("Channel name of:{0} already exist", inputChannelName);
                    GetContext<FSWindow>().ShowNotification(new GUIContent(tip));
                    return;
                }

                ChannelInfo channelInfo = new ChannelInfo(inputChannelName);
                selectPkgInfo.channels.Add(channelInfo);
                inputChannelName = string.Empty;

                selectPkgChannelNames = FindChannelNames(selectPkgInfo, out selectChannelIndex, out selectChannelInfo);
                RefreshActivePackageChannel();

                GUI.FocusControl(null);
                settings.Save();
                GetContext<FSWindow>().ShowNotification(new GUIContent("Channel create success"));
            }

            private void RefreshActivePackageChannel()
            {
                if (null == selectPkgInfo || null == activePkgInfo)
                {
                    return;
                }

                if (selectPkgInfo.platform == activePkgInfo.platform)
                {
                    activeChannelNames =
                        FindChannelNames(activePkgInfo, out activeChannelIndex, out activeChannelInfo);
                }
            }

            private ChannelInfo FindChannelInfo(string channelName)
            {
                foreach (var item in selectPkgInfo.channels)
                {
                    if (item.channel == channelName)
                    {
                        return item;
                    }
                }

                return null;
            }

            private void DrawEditChannel()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Edit Channel Info:");
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    selectChannelIndex =
                        EditorGUILayout.Popup("Select Channel:", selectChannelIndex, selectPkgChannelNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetSelectChannel();
                    }

                    EditorGUI.BeginDisabledGroup(null == selectChannelInfo);
                    if (GUILayout.Button("-", GUILayout.Width(40)))
                    {
                        if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this channel?", "Yes", "No"))
                        {
                            string desDir = string.Format("{0}/{1}", AssetBundleUtils.UpdatePackagesOutPath,
                                selectChannelInfo.channel);
                            FileUtils.DeleteDirectory(desDir);

                            selectPkgInfo.channels.RemoveAt(selectChannelIndex);
                            settings.Save();
                            selectPkgChannelNames = FindChannelNames(selectPkgInfo, out selectChannelIndex,
                                out selectChannelInfo);
                            RefreshActivePackageChannel();
                        }
                    }

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();
                }

                if (null != selectChannelInfo)
                {
                    DrawAppInfo();
                }

                EditorGUILayout.EndVertical();
            }

            private void SetSelectChannel()
            {
                if (selectChannelIndex >= 0 && selectChannelIndex < selectPkgInfo.channels.Count)
                {
                    selectChannelInfo = selectPkgInfo.channels[selectChannelIndex];

                    remoteUrlsList = new ReorderableList(selectChannelInfo.remoteUrls, typeof(string));
                    remoteUrlsList.list = selectChannelInfo.remoteUrls;
                    remoteUrlsList.drawHeaderCallback = DrawRemoteUrlsListOrderHead;
                    remoteUrlsList.drawElementCallback = DrawRemoteUrlsListOrderElement;
                }
                else
                {
                    selectChannelInfo = null;
                }
            }

            private void DrawRemoteUrlsListOrderHead(Rect rect)
            {
                EditorGUI.LabelField(rect, "Remote Urls:");
            }

            private void DrawRemoteUrlsListOrderElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                if (selectChannelInfo.remoteUrls.Count <= index)
                {
                    return;
                }

                string value = selectChannelInfo.remoteUrls[index];
                EditorGUI.BeginChangeCheck();
                rect.height = EditorGUIUtility.singleLineHeight;
                value = EditorGUI.TextField(rect, value);
                if (EditorGUI.EndChangeCheck())
                {
                    selectChannelInfo.remoteUrls[index] = value;
                }
            }

            private void DrawAppInfo()
            {
                ChannelInfo info = selectChannelInfo;

                EditorGUI.BeginChangeCheck();

                if (null != remoteUrlsList)
                {
                    remoteUrlsList.DoLayoutList();
                }

                string appVer = info.appVersion.ToString();
                appVer = EditorGUILayout.TextField("App Version:", appVer);
                string assetVer = info.assetVersion.ToString();
                assetVer = EditorGUILayout.TextField("Asset Version:", assetVer);

                info.appVersion = new VersionNum(appVer);
                info.assetVersion = new VersionNum(assetVer);

                if (EditorGUI.EndChangeCheck())
                {
                    settings.Save();
                }
            }

            private void DrawDiffType()
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    settings.diffType =
                        (DiffType)EditorGUILayout.EnumPopup("Diff Type:", settings.diffType);

                    if (settings.diffType == DiffType.EveryNewBestRange)
                    {
                        settings.rangeCount = EditorGUILayout.IntField("RangeCount:", settings.rangeCount,
                            GUILayout.MinWidth(100), GUILayout.MaxWidth(250));
                        settings.rangeCount = settings.rangeCount <= 0 ? 1 : settings.rangeCount;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            private void DrawGenerateDiffPackage()
            {
                EditorGUILayout.LabelField(string.Format("Current Project Active Platform Is:{0}", activePlatformName));
                EditorGUI.BeginChangeCheck();
                activeChannelIndex =
                    EditorGUILayout.Popup("Select Generate Channel:", activeChannelIndex, activeChannelNames);
                if (EditorGUI.EndChangeCheck())
                {
                    activeChannelInfo = activePkgInfo.channels[activeChannelIndex];
                }

                if (GUILayout.Button("Generate Diff Package", GUILayout.Height(60)))
                {
                    UpdateAssetsGenerator.Generate(activeChannelInfo.channel);
                    GetContext<FSWindow>().ShowNotification(new GUIContent("Generate diff package finished"));
                }
            }

            private void DrawAssetVersions()
            {
                float width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 80;
                bool deleteItem = false;
                foreach (var item in activeChannelInfo.assetVersions)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.TextField("Version:", item.ToString());
                    if (GUILayout.Button("-", GUILayout.Width(40)))
                    {
                        if (EditorUtility.DisplayDialog("Tip", "Are you sure delete current version?", "Yes", "No"))
                        {
                            DeleteVersion(item);
                            deleteItem = true;
                        }
                    }

                    GUILayout.EndHorizontal();

                    if (deleteItem)
                    {
                        break;
                    }
                }

                EditorGUIUtility.labelWidth = width;
            }

            public void DeleteVersion(VersionNum ver)
            {
                activeChannelInfo.assetVersions.Remove(ver);
                settings.Save();

                string desDir = Path.Combine(AssetBundleUtils.VersionManifestBackupPath(activeChannelInfo.channel),
                    ver.ToString());
                FileUtils.DeleteDirectory(desDir);
            }
        }
    }
}