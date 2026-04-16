using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreEditor.FS
{
    [Serializable]
    public class BuiltinData : IEquatable<BuiltinData>
    {
        public Object Content { get; private set; }
        public object ExtraData { get; private set; }

        public BuiltinData(Object content)
        {
            this.Content = content;
        }

        public BuiltinData(Object content, object extraData)
        {
            this.Content = content;
            this.ExtraData = extraData;
        }

        public bool Equals(BuiltinData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Content, other.Content);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BuiltinData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Content != null ? Content.GetHashCode() : 0) * 397;
            }
        }

        public override string ToString()
        {
            if (null != Content)
            {
                if (null != ExtraData)
                {
                    object displayObj = ExtraData;
                    if (ExtraData is Component comp)
                    {
                        displayObj = comp.gameObject;
                    }

                    return string.Format("{0}->{1}", Content, displayObj);
                }

                return string.Format("{0}", Content);
            }

            return string.Empty;
        }
    }

    public class BuiltinAssetsChecker
    {
        private const string UNITY_BUILTIN_EXTRA = "Resources/unity_builtin_extra";
        private const string UNITY_LIBRARY_DEF = "Library/unity default resources";

        private static bool ContainsBuiltin(string key)
        {
            return key.Contains(UNITY_BUILTIN_EXTRA) || key.Contains(UNITY_LIBRARY_DEF);
        }

        private static bool ContainsBuiltin(Object obj)
        {
            if (null == obj)
            {
                return false;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            return ContainsBuiltin(path);
        }
       
        public static Dictionary<Object, HashSet<BuiltinData>> FindBuiltinAssets(List<Object> assets)
        {
            var findProgress = new ProgressHandler();
            findProgress.SetInfo(assets.Count, "Find Builtin Assets");

            Dictionary<Object, HashSet<BuiltinData>>
                builtinMap = new Dictionary<Object, HashSet<BuiltinData>>();
            foreach (var item in assets)
            {
                findProgress.Tick();
                string path = AssetDatabase.GetAssetPath(item);
                string[] depends = AssetDatabase.GetDependencies(path);
                foreach (var dep in depends)
                {
                    Object obj = AssetDatabase.LoadMainAssetAtPath(dep);
                    if (obj is Material material)
                    {
                        CheckMaterialBuiltin(material, ref builtinMap);
                    }

                    if (obj is GameObject go)
                    {
                        CheckGameObjectBuiltin(go, ref builtinMap);
                    }
                }
            }

            return builtinMap;
        }

        private static void CheckMaterialBuiltin(Material material,
            ref Dictionary<Object, HashSet<BuiltinData>> builtinMap)
        {
            HashSet<BuiltinData> hashSet = null;
            Shader shader = material.shader;
            //shader是否是内置
            bool res = ContainsBuiltin(shader);
            if (res)
            {
                if (!builtinMap.TryGetValue(material, out hashSet))
                {
                    hashSet = new HashSet<BuiltinData>();
                    builtinMap.Add(material, hashSet);
                }

                hashSet.Add(new BuiltinData(shader));
            }

            //引用的贴图是否是内置
            for (int i = 0, count = ShaderUtil.GetPropertyCount(shader); i < count; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shader, i);
                    Texture tex = material.GetTexture(propertyName);
                    if (ContainsBuiltin(tex))
                    {
                        if (!builtinMap.TryGetValue(material, out hashSet))
                        {
                            hashSet = new HashSet<BuiltinData>();
                            builtinMap.Add(material, hashSet);
                        }

                        hashSet.Add(new BuiltinData(tex, propertyName));
                    }
                }
            }
        }

        private static void CheckGameObjectBuiltin(GameObject go,
            ref Dictionary<Object, HashSet<BuiltinData>> builtinMap)
        {
            HashSet<BuiltinData> hashSet = null;
            Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renders)
            {
                foreach (var material in render.sharedMaterials)
                {
                    if (null == material)
                    {
                        Debug.LogFormat("sharedMaterials is null:{0}->{1}", go.name, render.gameObject.name);
                        continue;
                    }

                    //材质球是否是内置
                    if (ContainsBuiltin(material))
                    {
                        if (!builtinMap.TryGetValue(go, out hashSet))
                        {
                            hashSet = new HashSet<BuiltinData>();
                            builtinMap.Add(go, hashSet);
                        }

                        hashSet.Add(new BuiltinData(material, render));
                    }
                    else
                    {
                        CheckMaterialBuiltin(material, ref builtinMap);
                    }
                }
            }

            //mesh是否是内置
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);
            foreach (var meshFilter in meshFilters)
            {
                if (ContainsBuiltin(meshFilter.sharedMesh))
                {
                    if (!builtinMap.TryGetValue(go, out hashSet))
                    {
                        hashSet = new HashSet<BuiltinData>();
                        builtinMap.Add(go, hashSet);
                    }

                    hashSet.Add(new BuiltinData(meshFilter.sharedMesh, meshFilter));
                }
            }
        }
    }
}