using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.FS;
using UnityEngine;

namespace Core
{
    public class FileReadUtils
    {
        public static void Read(string path, MimeType mimeType, ReadCompletedAction callback, object userData)
        {
            FileReader fileReader = new FileReader(path, mimeType);
            fileReader.userData = userData;
            fileReader.CompletedEvent += callback;
            fileReader.Read();
        }

        public static void Read(string path, MimeType mimeType, ReadCompletedAction callback)
        {
            Read(path, mimeType, callback, null);
        }

        public static void ReadInternal(string path, MimeType mimeType, ReadCompletedAction callback, object userData)
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.Android)
            {
                path = string.Format("{0}/{1}", AssetPath.InternalAssetsPath, path);
            }
            else
            {
                path = string.Format("file://{0}/{1}", AssetPath.InternalAssetsPath, path);
            }

            FileReader fileReader = new FileReader(path, mimeType);
            fileReader.maxRetryCount = 0;
            fileReader.userData = userData;
            fileReader.CompletedEvent += callback;
            fileReader.Read();
        }

        public static void ReadInternal(string path, MimeType mimeType, ReadCompletedAction callback)
        {
            ReadInternal(path, mimeType, callback, null);
        }

        public static void Read(string path, AudioType audioType, ReadCompletedAction callback, object userData)
        {
            FileReader fileReader = new FileReader(path, audioType);
            fileReader.userData = userData;
            fileReader.CompletedEvent += callback;
            fileReader.Read();
        }

        public static void Read(string path, AudioType audioType, ReadCompletedAction callback)
        {
            Read(path, audioType, callback, null);
        }


        public static void ChainRead(IEnumerable<string> paths, MimeType mimeType, ReadCompletedAction callback,
            object userData)
        {
            ChainReader fileReader = new ChainReader(paths, mimeType);
            fileReader.userData = userData;
            fileReader.CompletedEvent += callback;
            fileReader.Read();
        }

        public static void ChainRead(IEnumerable<string> paths, MimeType mimeType, ReadCompletedAction callback)
        {
            ChainRead(paths, mimeType, callback, null);
        }


        public static void ChainRead(IEnumerable<string> paths, AudioType audioType, ReadCompletedAction callback,
            object userData)
        {
            FileReader fileReader = new ChainReader(paths, audioType);
            fileReader.userData = userData;
            fileReader.CompletedEvent += callback;
            fileReader.Read();
        }

        public static void ChainRead(IEnumerable<string> paths, AudioType audioType, ReadCompletedAction callback)
        {
            ChainRead(paths, audioType, callback, null);
        }

        public static string ReadText(string name)
        {
            string text = null;
            string loadPath = string.Format("{0}/{1}", AssetPath.UpdateAssetsPath, name);
            if (FileUtils.ExistsFile(loadPath))
            {
                using (StreamReader sr = new StreamReader(loadPath, Encoding.UTF8))
                {
                    text = sr.ReadToEnd();
                }
            }
            else
            {
                loadPath = name;
                int extensionIndex = name.LastIndexOf(".");
                if (extensionIndex > 0)
                {
                    loadPath = name.Substring(0, extensionIndex);
                }

                TextAsset textAsset = Resources.Load<TextAsset>(loadPath);
                if (null != textAsset)
                {
                    text = textAsset.text;
                }
            }

            return text;
        }

        public static byte[] ReadBytes(string name)
        {
            byte[] bytes = null;
            string loadPath = string.Format("{0}/{1}", AssetPath.UpdateAssetsPath, name);
            if (FileUtils.ExistsFile(loadPath))
            {
                bytes = File.ReadAllBytes(loadPath);
            }
            else
            {
                loadPath = name;
                int extensionIndex = name.LastIndexOf(".");
                if (extensionIndex > 0)
                {
                    loadPath = name.Substring(0, extensionIndex);
                }

                TextAsset textAsset = Resources.Load<TextAsset>(loadPath);
                if (null != textAsset)
                {
                    bytes = textAsset.bytes;
                }
            }

            return bytes;
        }

        public static string ReadFileText(string path)
        {
            string text = null;
            if (FileUtils.ExistsFile(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    text = sr.ReadToEnd();
                }
            }

            return text;
        }

        public static byte[] ReadFileBytes(string path)
        {
            byte[] bytes = null;
            if (FileUtils.ExistsFile(path))
            {
                bytes = File.ReadAllBytes(path);
            }

            return bytes;
        }
    }
}