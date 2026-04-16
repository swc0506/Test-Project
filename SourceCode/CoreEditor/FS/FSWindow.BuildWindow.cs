using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreEditor.FS
{
    public partial class FSWindow
    {
        private class BuildWindow : BaseWindow
        {
            private const string SELECT_EDIT_PIPELINE_KEY = "FS.SELECT_EDIT_PIPELINE_KEY";

            public override string Title
            {
                get { return "Build Assets"; }
            }

            private FSSettingsObject settings;
            private bool changedConfig;

            private string[] pkgNames;
            private int selectPipeIndex;
            private StepPipeline selectPipe;

            private ReorderableList cmdList;
            private string[] stepNames;
            private string[] stepDescribes;
            private int selectStepIndex;
            private CommandStep selectStep;
            private Dictionary<string, IStepDrawable> stepDrawMap;
            private IStepDrawable selectDraw;
            private Vector2 drawPos;

            private string[] buildTargets;
            private BuildTarget buildTarget;
            private int selectBuildTargetIndex;


            private Dictionary<string, bool> buildPkgMap;


            public override void Initial(TabWindow context)
            {
                base.Initial(context);

                cmdList = new ReorderableList(null, typeof(CommandStep));
                cmdList.displayAdd = false;
                cmdList.drawHeaderCallback = DrawOrderHead;
                cmdList.drawElementCallback = DrawOrderElement;
                cmdList.onRemoveCallback = OnOrderRemoveElement;
                cmdList.onSelectCallback = OnOrderSelectElement;
                cmdList.onChangedCallback = OnOrderChangedElement;
                CollectBuildSteps();
                buildTargets = GetContext<FSWindow>().CollectBuildTargets();
                buildPkgMap = new Dictionary<string, bool>();
            }

            private void CollectBuildSteps()
            {
                var stepMap = AttributeUtils.GetTypeAttributeInfo<IBuildStep, BuildStepAttribute>();
                var iter = stepMap.GetEnumerator();
                stepNames = new string[stepMap.Count];
                stepDescribes = new string[stepMap.Count];
                stepDrawMap = new Dictionary<string, IStepDrawable>();
                int index = 0;
                while (iter.MoveNext())
                {
                    stepNames[index] = iter.Current.Value.Name;
                    stepDescribes[index] = iter.Current.Value.Describe;
                    if (typeof(IStepDrawable).IsAssignableFrom(iter.Current.Key))
                    {
                        var draw = (IStepDrawable) Activator.CreateInstance(iter.Current.Key);
                        stepDrawMap.Add(iter.Current.Value.Name, draw);
                    }

                    index++;
                }
            }

            public override void Enable()
            {
                base.Enable();
                settings = GetContext<FSWindow>().settings;

                CollectPipelineNames();

                buildTarget = EditorUserBuildSettings.activeBuildTarget;
                selectBuildTargetIndex = Array.IndexOf(buildTargets, buildTarget.ToString());
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

                drawPos = EditorGUILayout.BeginScrollView(drawPos);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    DrawSelectPackage();
                    GUILayout.Space(10);
                    if (null != selectPipe)
                    {
                        DrawCommandStep();
                    }

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();

                GUILayout.Space(5);
                DrawConfigBuildPackages();
            }

            private void CollectPipelineNames()
            {
                string selectPkgName = EditorPrefs.GetString(SELECT_EDIT_PIPELINE_KEY);
                selectPipeIndex = 0;
                for (int i = 0; i < settings.pipelines.Count; i++)
                {
                    if (null == GetContext<FSWindow>().FindPackage(settings.pipelines[i].pkgName))
                    {
                        settings.pipelines.RemoveAt(i);
                        i--;
                        changedConfig = true;
                    }
                }

                for (int i = 0; i < settings.packages.Count; i++)
                {
                    string pkgName = settings.packages[i].name;
                    if (FindPipelineIndex(pkgName) < 0)
                    {
                        StepPipeline pipeline = new StepPipeline();
                        pipeline.pkgName = pkgName;
                        settings.pipelines.Add(pipeline);
                        changedConfig = true;
                    }
                }

                pkgNames = new string[settings.packages.Count];
                for (int i = 0; i < pkgNames.Length; i++)
                {
                    pkgNames[i] = settings.packages[i].name;
                    if (selectPkgName == pkgNames[i])
                    {
                        selectPipeIndex = i;
                    }
                }

                SetSelectPipeline();
            }

            private void SetSelectPipeline()
            {
                if (selectPipeIndex >= 0 && selectPipeIndex < settings.pipelines.Count)
                {
                    selectPipe = settings.pipelines[selectPipeIndex];
                    cmdList.list = selectPipe.steps;
                }
                else
                {
                    selectPipe = null;
                    cmdList.list = null;
                }

                cmdList.elementHeight = EditorGUIUtility.singleLineHeight;
                SetSelectStepIndex(-1);
            }

            private void DrawSelectPackage()
            {
                EditorGUILayout.LabelField("<size=16>Config Build Package Pipeline</size>",GUILayout.Height(20));
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    selectPipeIndex = EditorGUILayout.Popup("Select Asset Package:", selectPipeIndex, pkgNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetString(SELECT_EDIT_PIPELINE_KEY, pkgNames[selectPipeIndex]);
                        SetSelectPipeline();
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            private int FindPipelineIndex(string pkgName)
            {
                for (int i = 0; i < settings.pipelines.Count; i++)
                {
                    if (settings.pipelines[i].pkgName == pkgName)
                    {
                        return i;
                    }
                }

                return -1;
            }

            private void DrawCommandStep()
            {
                EditorGUI.BeginChangeCheck();
                selectPipe.bundleOptions = (BuildAssetBundleOptions) EditorGUILayout.EnumFlagsField(
                    "Build AssetBundleOptions:", selectPipe.bundleOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    changedConfig = true;
                }
                
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                {
                    selectStepIndex = EditorGUILayout.Popup("Add Build Step:", selectStepIndex, stepNames);
                    if (GUILayout.Button("+", GUILayout.Width(40)))
                    {
                        string selectCmd = stepNames[selectStepIndex];
                        if (null == FindCommand(selectCmd))
                        {
                            CommandStep step = new CommandStep();
                            step.cmd = selectCmd;
                            step.enable = true;
                            selectPipe.steps.Add(step);
                            cmdList.list = selectPipe.steps;
                            settings.Save();
                        }
                        else
                        {
                            string notifyTip = string.Format("Command step name of:{0} already exist", selectCmd);
                            GetContext<FSWindow>().ShowNotification(new GUIContent(notifyTip));
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(5);
                cmdList.DoLayoutList();
                string tip = string.Format("<size=14><b>Tip:execute build step order will by index</b></size>");
                EditorGUILayout.LabelField(tip);

                GUILayout.Space(5);
                if (null != selectDraw)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    selectDraw.Draw(settings.packages[selectPipeIndex]);
                    EditorGUILayout.EndVertical();
                }
            }

            private CommandStep FindCommand(string cmd)
            {
                foreach (var item in selectPipe.steps)
                {
                    if (item.cmd == cmd)
                    {
                        return item;
                    }
                }

                return null;
            }

            private void DrawOrderHead(Rect rect)
            {
                EditorGUI.LabelField(rect, "Steps");
            }

            private int FindCommandIndex(string cmd)
            {
                for (int i = 0; i < stepNames.Length; i++)
                {
                    if (stepNames[i] == cmd)
                    {
                        return i;
                    }
                }

                return -1;
            }

            private void SetSelectStepIndex(int index)
            {
                cmdList.index = index;
                RefreshSelectStepIndex();
            }

            private void RefreshSelectStepIndex()
            {
                if (null != cmdList && cmdList.index >= 0 && selectPipe.steps.Count > 0)
                {
                    selectStep = selectPipe.steps[cmdList.index];
                    stepDrawMap.TryGetValue(selectStep.cmd, out selectDraw);
                }
                else
                {
                    cmdList.elementHeight = EditorGUIUtility.singleLineHeight;
                    selectStep = null;
                    selectDraw = null;
                }
            }

            private void DrawOrderElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                CommandStep step = selectPipe.steps[index];
                rect.height = EditorGUIUtility.singleLineHeight;

                var groupRect = rect;
                groupRect.y += 5;
                EditorGUI.LabelField(groupRect, "Enable Step:");
                groupRect.x += 80;
                EditorGUI.BeginChangeCheck();
                step.enable = EditorGUI.Toggle(groupRect, step.enable);
                if (EditorGUI.EndChangeCheck())
                {
                    changedConfig = true;
                }

                groupRect.x += 30;
                EditorGUI.LabelField(groupRect, step.cmd);

                var boxRect = rect;
                boxRect.y = groupRect.yMax + 5;
                boxRect.height = rect.height * 2;
                int cmdIndex = FindCommandIndex(step.cmd);
                if (cmdIndex >= 0 && cmdIndex < stepDescribes.Length)
                {
                    EditorGUI.HelpBox(boxRect, stepDescribes[cmdIndex], MessageType.Info);
                }
                else
                {
                    string tip = string.Format("Command step name of:{0} is missing", step.cmd);
                    EditorGUI.HelpBox(boxRect, tip, MessageType.Error);
                }

                cmdList.elementHeight = boxRect.yMax - rect.yMin + 5;
            }

            private void OnOrderRemoveElement(ReorderableList list)
            {
                if (EditorUtility.DisplayDialog("Tip", "Are you sure delete this build step?", "Yes", "No"))
                {
                    bool isLast = list.index >= list.count - 1;
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    if (isLast)
                    {
                        SetSelectStepIndex(list.count - 1);
                    }

                    settings.Save();
                }
            }

            private void OnOrderSelectElement(ReorderableList list)
            {
                RefreshSelectStepIndex();
            }

            private void OnOrderChangedElement(ReorderableList list)
            {
                changedConfig = true;
            }

            private void DrawConfigBuildPackages()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // GUILayout.Space(5);
                // EditorGUI.BeginChangeCheck();
                // config.bundleOptions = (BuildAssetBundleOptions) EditorGUILayout.EnumFlagsField(
                //     "BuildAssetBundleOptions:", config.bundleOptions);
                // if (EditorGUI.EndChangeCheck())
                // {
                //     changedConfig = true;
                // }

                GUILayout.Space(5);
                EditorGUI.BeginChangeCheck();
                selectBuildTargetIndex =
                    EditorGUILayout.Popup("Select Build Target:", selectBuildTargetIndex, buildTargets);
                if (EditorGUI.EndChangeCheck())
                {
                    Enum.TryParse<BuildTarget>(buildTargets[selectBuildTargetIndex], out buildTarget);
                    changedConfig = true;
                }

                EditorGUILayout.HelpBox(string.Format("current project active target is:{0}",
                    EditorUserBuildSettings.activeBuildTarget), MessageType.Info);

                GUILayout.Space(5);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Check Build Packages:");
                    for (int i = 0; i < settings.pipelines.Count; i++)
                    {
                        GUILayout.Space(2);
                        var pkg = GetContext<FSWindow>().FindPackage(settings.pipelines[i].pkgName);
                        if (!buildPkgMap.ContainsKey(pkg.name))
                        {
                            buildPkgMap.Add(pkg.name, true);
                        }

                        buildPkgMap[pkg.name] = EditorGUILayout.ToggleLeft(pkg.name, buildPkgMap[pkg.name]);
                    }

                    if (GUILayout.Button("<size=22>Start Build</size>", GUILayout.ExpandWidth(true),
                        GUILayout.Height(80)))
                    {
                        if (buildTarget == EditorUserBuildSettings.activeBuildTarget)
                        {
                            BuildPackages();
                        }
                        else
                        {
                            if (EditorUtility.DisplayDialog("Tip",
                                "Current select build target not equal active build target,do you want to switch to build target?",
                                "Yes", "No"))
                            {
                                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                                EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);
                                BuildPackages();
                            }
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
            }

            private void BuildPackages()
            {
                foreach (var item in settings.pipelines)
                {
                    var pkg = GetContext<FSWindow>().FindPackage(item.pkgName);
                    if (buildPkgMap[pkg.name])
                    {
                        AssetPackageBuilder.BuildPackage(pkg, item.steps);
                    }
                }

                GUIUtility.ExitGUI();
            }
        }
    }
}