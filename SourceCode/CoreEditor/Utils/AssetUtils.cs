using System.Collections.Generic;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;
using Object = UnityEngine.Object;

namespace CoreEditor
{
    public enum TextureType
    {
        Unknown,
        PNG,
        JPG,
        EXR,
        TGA
    }

    public static class AssetUtils
    {
        private static readonly HashSet<string> IgnoreSuffixes = new HashSet<string>()
        {
            ".meta", ".cs"
        };

        private static readonly HashSet<Type> IgnoreType = new HashSet<Type>()
        {
            typeof(DefaultAsset),
            typeof(MonoScript),
        };

        public static bool CheckRelativeAssetsPath(string path, out string relativePath)
        {
            relativePath = Application.dataPath;
            int assetsIndex = -1;
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("\\", "/");
                assetsIndex = path.IndexOf(relativePath);
            }

            if (assetsIndex >= 0)
            {
                relativePath = path.Substring(relativePath.Length + 1);
                return true;
            }

            return false;
        }

        public static string FilePathToAssetPath(string path)
        {
            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            return path;
        }

        public static bool CreateAsset(Object obj, string path)
        {
            if (null == obj || string.IsNullOrEmpty(path))
            {
                return false;
            }

            FileUtils.CreateDirectory(Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(obj, path);
            return true;
        }


        public static List<string> GetAssetsPath(string dir, string extension = ".*")
        {
            List<string> paths = new List<string>();
            if (!FileUtils.ExistsDirectory(dir))
            {
                return paths;
            }

            string[] filePaths = Directory.GetFiles(dir, "*" + extension, SearchOption.AllDirectories);
            foreach (var item in filePaths)
            {
                if (!IgnoreSuffixes.Contains(Path.GetExtension(item)))
                {
                    paths.Add(FilePathToAssetPath(item));
                }
            }

            return paths;
        }

        public static List<Object> GetAssets(string dir, string extension = ".*")
        {
            List<Object> objects = new List<Object>();
            List<string> paths = GetAssetsPath(dir, extension);
            var progress = new ProgressHandler();
            progress.SetInfo(paths.Count, "Seek Assets");
            foreach (var item in paths)
            {
                progress.Tick();
                Object obj = AssetDatabase.LoadMainAssetAtPath(item);
                if (null == obj)
                {
                    Debug.LogWarningFormat("LoadMainAssetAtPath is null:{0}", item);
                    continue;
                }

                if (!IgnoreType.Contains(obj.GetType()))
                {
                    objects.Add(obj);
                }
            }

            return objects;
        }

        public static List<Object> GetAssets(Object obj, string extension = ".*")
        {
            List<Object> objects = new List<Object>();
            if (null != obj)
            {
                //文件夹
                if (obj is DefaultAsset)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    objects = GetAssets(path, extension);
                }
                else
                {
                    if (!IgnoreType.Contains(obj.GetType()))
                    {
                        objects.Add(obj);
                    }
                }
            }

            return objects;
        }

        public static List<Object> GetSelectAssets()
        {
            List<Object> objects = new List<Object>();
            var assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            foreach (var item in assets)
            {
                if (item is DefaultAsset)
                {
                    continue;
                }

                objects.Add(item);
            }

            return objects;
        }

        public static List<string> GetSubDirectories(string dir)
        {
            List<string> paths = new List<string>();
            if (FileUtils.ExistsDirectory(dir))
            {
                DirectoryInfo[] dInfos = new DirectoryInfo(dir).GetDirectories();
                foreach (var item in dInfos)
                {
                    paths.Add(FilePathToAssetPath(item.FullName));
                }
            }

            return paths;
        }

        public static void SaveAsset(Object obj)
        {
            if (null != obj)
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
            }
        }

        public static HashSet<string> GetBuildScenes()
        {
            HashSet<string> scenes = new HashSet<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e != null && e.enabled)
                {
                    scenes.Add(e.path);
                }
            }

            return scenes;
        }

        public static TextureType GetTextureType(string path)
        {
            String imgType = Path.GetExtension(path).ToLower();
            if (imgType == ".png")
            {
                return TextureType.PNG;
            }
            else if (imgType == ".jpg")
            {
                return TextureType.JPG;
            }
            else if (imgType == ".exr")
            {
                return TextureType.EXR;
            }
            else if (imgType == ".tga")
            {
                return TextureType.TGA;
            }
            return TextureType.Unknown;
        }
    }
}