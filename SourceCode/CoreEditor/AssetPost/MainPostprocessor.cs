using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    class MainPostprocessor : AssetPostprocessor
    {
        private static List<PostModel> models;
        private static List<PostTexture> textures;
        private static List<PostAudio> audios;
        private static List<PostPrefab> prefabs;
        private static List<PostTextAsset> texts;
        private static List<PostAnimation> animations;
        private static List<AssignMaterialModel> assignMaterials;

        [InitializeOnLoadMethod]
        private static void Initial()
        {
            models = CreatePostInstances<PostModel>();
            textures =CreatePostInstances<PostTexture>();
            audios = CreatePostInstances<PostAudio>();
            prefabs =CreatePostInstances<PostPrefab>();
            texts = CreatePostInstances<PostTextAsset>();
            animations = CreatePostInstances<PostAnimation>();
            assignMaterials = CreatePostInstances<AssignMaterialModel>();
        }
        
        private static List<T> CreatePostInstances<T>()  where T : class
        {
            List<T> instances = new List<T>();
            Assembly assembly = Assembly.Load("Assembly-CSharp-Editor");
            if (null != assembly)
            {
                instances.AddRange(AttributeUtils.CreateInstances<T>(assembly));
            }
           
            Assembly callAssembly = Assembly.GetCallingAssembly();
            if (assembly != callAssembly)
            {
                instances.AddRange(AttributeUtils.CreateInstances<T>(callAssembly));
            }
            return instances;
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (path.StartsWith("Assets/StreamingAssets"))
                {
                    continue;
                }

                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (null != go)
                {
                    OnPostprocessPrefab(path, go);
                    continue;
                }

                TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (null != text)
                {
                    if (!Path.GetExtension(path).Equals(".cs", StringComparison.CurrentCultureIgnoreCase))
                    {
                        OnPostprocessTextAsset(path, text);
                    }

                    continue;
                }

                AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (null != animation)
                {
                    if (Path.GetExtension(path).Equals(".anim", StringComparison.CurrentCultureIgnoreCase))
                    {
                        OnPostprocessAnimationClip(path, animation);
                    }

                    continue;
                }
            }
        }

        #region Model

        private void OnPreprocessModel()
        {
            foreach (var item in models)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPreprocessModel((ModelImporter) assetImporter);
                }
            }
        }

        private void OnPostprocessModel(GameObject go)
        {
            foreach (var item in models)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPostprocessModel(go, (ModelImporter) assetImporter);
                }
            }
        }

        #endregion

        #region Texture

        private void OnPreprocessTexture()
        {
            foreach (var item in textures)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPreprocessTexture((TextureImporter) assetImporter);
                }
            }
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            foreach (var item in textures)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPostprocessTexture(texture, (TextureImporter) assetImporter);
                }
            }
        }

        private void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            foreach (var item in textures)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPostprocessSprites(texture, sprites, (TextureImporter) assetImporter);
                }
            }
        }

        #endregion

        #region AudioClip

        private void OnPreprocessAudio()
        {
            foreach (var item in audios)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPreprocessAudio((AudioImporter) assetImporter);
                }
            }
        }

        private void OnPostprocessAudio(AudioClip audioClip)
        {
            foreach (var item in audios)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPostprocessAudio(audioClip, (AudioImporter) assetImporter);
                }
            }
        }

        #endregion

        #region AnimationClip

        private void OnPreprocessAnimation()
        {
            foreach (var item in animations)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPreprocessModelAnimation((ModelImporter) assetImporter);
                }
            }
        }

        private void OnPostprocessAnimation(GameObject rootGo, AnimationClip animationClip)
        {
            foreach (var item in animations)
            {
                if (item.IsProcess(assetPath))
                {
                    item.OnPostprocessModelAnimation(rootGo, animationClip, (ModelImporter) assetImporter);
                }
            }
        }

        #endregion

        private Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            foreach (var item in assignMaterials)
            {
                if (item.IsProcess(assetPath))
                {
                    return item.OnAssignMaterialModel(material, renderer);
                }
            }

            return null;
        }

        private static void OnPostprocessPrefab(string path, GameObject go)
        {
            foreach (var item in prefabs)
            {
                if (item.IsProcess(path))
                {
                    item.OnPostprocessPrefab(path, go);
                }
            }
        }

        private static void OnPostprocessTextAsset(string path, TextAsset text)
        {
            foreach (var item in texts)
            {
                if (item.IsProcess(path))
                {
                    item.OnPostprocessTextAsset(path, text);
                }
            }
        }

        private static void OnPostprocessAnimationClip(string path, AnimationClip animationClip)
        {
            foreach (var item in animations)
            {
                if (item.IsProcess(path))
                {
                    item.OnPostprocessAnimationClip(path, animationClip);
                }
            }
        }
    }
}