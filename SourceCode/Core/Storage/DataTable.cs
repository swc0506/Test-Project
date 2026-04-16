using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using Core.FS;
using UnityEngine;

namespace Core.Storage
{
    public enum SaveType
    {
        Binary,
        Text,
    }

    public class DataTable<T> where T : class
    {
        private string rootPath;
        private string name;
        private SaveType saveType;
        private string savePath;

        private float interval = 2;
        private float saveCountThreshold = 5;
        private bool useThread = true;
        private object fileLock;

        private Action callback;
        private T data;

        private float prevSaveTime;
        private int tickSaveCount;

        public T Data
        {
            get { return data; }
        }

        public DataTable(string rootPath, string name, SaveType saveType)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = string.Format("{0}/DataTables", AssetPath.UpdateAssetsPath);
            }

            this.rootPath = rootPath;
            this.name = name;
            this.saveType = saveType;
            savePath = string.Format("{0}/{1}.dt", rootPath, name);

            MonoEventProxy.Instance.LateUpdateEvent += OnLateUpdate;
            MonoEventProxy.Instance.ApplicationQuitEvent += OnApplicationQuit;
        }

        public DataTable(string name, SaveType saveType) : this(null, name, saveType)
        {
        }

        public DataTable(string name) : this(null, name, SaveType.Text)
        {
        }

        public void SetSaveRule(float interval, int saveCountThreshold, bool useThread)
        {
            this.interval = interval;
            this.saveCountThreshold = saveCountThreshold;
            this.useThread = useThread;
        }

        public void Load()
        {
            if (null != data)
            {
                return;
            }

            object fileData = null;
            if (saveType == SaveType.Binary)
            {
                fileData = FileReadUtils.ReadFileBytes(savePath);
            }
            else
            {
                fileData = FileReadUtils.ReadFileText(savePath);
            }

            OnLoadCompleted(fileData, savePath, null);
        }

        public void LoadAsync(Action callback)
        {
            if (null != data)
            {
                callback?.Invoke();
                return;
            }

            this.callback = callback;

            var mimeType = saveType == SaveType.Binary ? MimeType.Binary : MimeType.Text;
            FileReadUtils.Read(savePath, mimeType, OnLoadCompleted);
        }

        private void OnLoadCompleted(object data, string path, object userData)
        {
            string str = null;
            if (saveType == SaveType.Binary)
            {
                byte[] bytes = (byte[])data;
                if (null != bytes)
                {
                    byte[] uBytes = EncryptUtils.Decrypt(name, bytes);
                    str = Encoding.UTF8.GetString(uBytes);
                }
            }
            else
            {
                str = (string)data;
            }

            if (!string.IsNullOrEmpty(str))
            {
                this.data = JsonUtils.ToObject<T>(str);
            }
            else
            {
                this.data = Activator.CreateInstance<T>();
            }

            if (null != callback)
            {
                callback.Invoke();
                callback = null;
            }
        }

        public void Save()
        {
            if (null == data)
            {
                return;
            }

            if (interval <= 0)
            {
                SaveFile();
            }
            else
            {
                tickSaveCount++;
                if (tickSaveCount >= saveCountThreshold)
                {
                    SaveFile();
                }
            }
        }

        private void SaveFile()
        {
            if (useThread)
            {
                if (null == fileLock)
                {
                    fileLock = new object();
                }

                ThreadPool.QueueUserWorkItem(OnThreadSave);
            }
            else
            {
                DoSaveFile();
            }
        }

        private void OnThreadSave(object obj)
        {
            lock (fileLock)
            {
                DoSaveFile();
            }
        }

        private void DoSaveFile()
        {
            string str = JsonUtils.ToJson(data);
            if (!string.IsNullOrEmpty(str))
            {
                if (saveType == SaveType.Binary)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(str);
                    byte[] uBytes = EncryptUtils.Encrypt(name, bytes);
                    FileUtils.CreateFile(savePath, uBytes);
                }
                else
                {
                    FileUtils.CreateFile(savePath, str);
                }
            }
        }

        private void OnLateUpdate()
        {
            if (tickSaveCount > 0 && Time.unscaledTime - prevSaveTime > interval)
            {
                tickSaveCount = 0;
                prevSaveTime = Time.unscaledTime;
                SaveFile();
            }
        }

        private void OnApplicationQuit()
        {
            if (tickSaveCount > 0)
            {
                DoSaveFile();
            }
        }

        public void Dispose()
        {
            data = null;
            MonoEventProxy.Instance.LateUpdateEvent -= OnLateUpdate;
            MonoEventProxy.Instance.ApplicationQuitEvent -= OnApplicationQuit;
        }
    }
}