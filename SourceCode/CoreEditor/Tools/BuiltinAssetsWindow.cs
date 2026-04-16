using System;
using System.Collections.Generic;
using System.IO;
using CoreEditor.FS;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreEditor.Tools
{
    public class BuiltinAssetsWindow : EditorWindow
    {
        private const string DEFAULT_SHADER = "Legacy Shaders/Diffuse";

        private GUILayoutOption width;
        private GUILayoutOption height;

        private float itemOffset = 50;
        private GUILayoutOption itemWidth;
        private GUILayoutOption itemHeight;

        private Object findTarget;
        private List<Object> assets;
        private Dictionary<Object, HashSet<BuiltinData>> builtinMap;
        private Vector2 scrollPos;

        private readonly string[] OPTIONAL_TYPES = new[] {"All", "GameObject", "Material"};
        private int selectTypeIndex;


        [MenuItem("Tools/Assets Optimize/Builtin Assets Window", false, 5011)]
        public static void ShowWindow()
        {
            BuiltinAssetsWindow editor = GetWindow<BuiltinAssetsWindow>("Builtin Assets Window");
            editor.minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            width = GUILayout.ExpandWidth(true);
            height = GUILayout.Height(50);
            itemHeight = GUILayout.Height(20);
        }

        private void OnGUI()
        {
            findTarget = EditorGUILayout.ObjectField("Find Target:", findTarget, typeof(Object), false);
            if (null == findTarget)
            {
                findTarget = AssetDatabase.LoadMainAssetAtPath("Assets");
            }

            if (GUILayout.Button("Seek Builtin Assets", width, height))
            {
                assets = AssetUtils.GetAssets(findTarget);
                builtinMap = BuiltinAssetsChecker.FindBuiltinAssets(assets);
            }

            if (null == builtinMap)
            {
                return;
            }

            DrawBuiltinAssets();

            GUILayout.Space(5);
            if (GUILayout.Button("Replace Material Builtin Shader To Local", width, height))
            {
                ReplaceMaterialBuiltinShaderToLocal();
            }

            if (GUILayout.Button("Replace Material Standard Shader To Diffuse", width, height))
            {
                ReplaceMaterialStandardShaderToDiffuse();
            }

            if (GUILayout.Button("Clear Model Default-Material", width, height))
            {
                ClearModelDefaultMaterial();
            }

            if (GUILayout.Button("Remove Default-ParticleSystem Material", width, height))
            {
                RemoveDefaultParticleSystemMaterial();
            }
        }

        private void DrawBuiltinAssets()
        {
            if (builtinMap.Count == 0)
            {
                GUILayout.Label("Don't Find Builtin Assets");
                return;
            }

            selectTypeIndex = EditorGUILayout.Popup("Filter Type", selectTypeIndex, OPTIONAL_TYPES);
            GUILayout.Label("Total Count:" + builtinMap.Count);

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            itemWidth = GUILayout.Width(position.width - itemOffset * 2);
            foreach (var item in builtinMap)
            {
                if (selectTypeIndex != 0 && item.Key.GetType().Name != OPTIONAL_TYPES[selectTypeIndex])
                {
                    continue;
                }

                GUILayout.Space(5);
                GUI.backgroundColor = Color.yellow;
                GUILayout.BeginVertical(EditorStyles.helpBox);
                string path = AssetDatabase.GetAssetPath(item.Key);
                if (GUILayout.Button(path, itemHeight))
                {
                    Selection.activeObject = item.Key;
                }

                GUILayout.Space(3);
                foreach (var builtin in item.Value)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(itemOffset);
                        GUILayout.TextField(builtin.ToString(), EditorStyles.textField, itemWidth);
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndVertical();
                GUI.backgroundColor = Color.white;
            }

            GUILayout.EndScrollView();
        }

        private void ProcessBuiltinAssets(Action<Object, HashSet<BuiltinData>, List<BuiltinData>> replaceFunc)
        {
            List<Object> removeObjs = new List<Object>();
            foreach (var item in builtinMap)
            {
                List<BuiltinData> removes = new List<BuiltinData>();

                replaceFunc.Invoke(item.Key, item.Value, removes);
              
                if (removes.Count > 0)
                {
                    foreach (var builtin in removes)
                    {
                        item.Value.Remove(builtin);
                    }
                    
                    EditorUtility.SetDirty(item.Key);
                }

                if (item.Value.Count == 0)
                {
                    removeObjs.Add(item.Key);
                }
            }

            foreach (var item in removeObjs)
            {
                builtinMap.Remove(item);
            }

            AssetDatabase.SaveAssets();
        }

        private void ReplaceMaterialBuiltinShaderToLocal()
        {
            ProcessBuiltinAssets((Object obj, HashSet<BuiltinData> builtins, List<BuiltinData> removes) =>
            {
                if (obj is Material material)
                {
                    foreach (var builtin in builtins)
                    {
                        //替换默认的shader
                        if (builtin.Content is Shader)
                        {
                            material.shader = Shader.Find(material.shader.name);
                            removes.Add(builtin);
                        }
                    }
                }
            });
        }

        private void ReplaceMaterialStandardShaderToDiffuse()
        {
            ProcessBuiltinAssets((Object obj, HashSet<BuiltinData> builtins, List<BuiltinData> removes) =>
            {
                if (obj is Material material)
                {
                    foreach (var builtin in builtins)
                    {
                        //替换默认的shader
                        if (builtin.Content is Shader shader)
                        {
                            if (shader.name == "Standard")
                            {
                                material.shader = Shader.Find(DEFAULT_SHADER);
                                removes.Add(builtin);
                            }
                        }
                    }
                }
            });
        }

        private void ClearModelDefaultMaterial()
        {
            ProcessBuiltinAssets((Object obj, HashSet<BuiltinData> builtins, List<BuiltinData> removes) =>
            {
                if (obj is GameObject go)
                {
                    string extName = Path.GetExtension(AssetDatabase.GetAssetPath(go));
                    bool isPrefab = extName.Equals("prefab", StringComparison.CurrentCultureIgnoreCase);
                    foreach (var builtin in builtins)
                    {
                        if (builtin.Content is Material)
                        {
                            //是fbx模型 替换默认的material
                            if (!isPrefab)
                            {
                                string path = AssetDatabase.GetAssetPath(go);
                                ModelImportSetting.ClearModelImportDefaultMaterial(path);
                                removes.Add(builtin);
                            }
                        }
                    }
                }
            });
        }


        private void RemoveDefaultParticleSystemMaterial()
        {
            ProcessBuiltinAssets((Object obj, HashSet<BuiltinData> builtins, List<BuiltinData> removes) =>
            {
                if (obj is GameObject go)
                {
                    foreach (var builtin in builtins)
                    {
                        if (builtin.Content is Material && builtin.ExtraData is Renderer render)
                        {
                            ParticleSystem ps = render.GetComponent<ParticleSystem>();
                            if (ps)
                            {
                                render.materials = new Material[0];
                                DestroyImmediate(ps, true);
                            }

                            removes.Add(builtin);
                        }
                    }
                }
            });
        }
    }
}