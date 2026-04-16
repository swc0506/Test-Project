using System;
using System.Collections.Generic;
using System.IO;

namespace CoreEditor
{
    public class FileWatcherUtils
    {
        private class WatcherData
        {
            public FileSystemWatcher Watcher { get; private set; }
            public Action<string, FileSystemEventArgs> Handler { get; private set; }


            public WatcherData(FileSystemWatcher watcher, Action<string, FileSystemEventArgs> handler)
            {
                this.Watcher = watcher;
                this.Handler = handler;
            }
        }

        private static List<WatcherData> watchers = new List<WatcherData>();

        public static bool AddWatcher(string path, string filter, Action<string, FileSystemEventArgs> handler)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileSystemWatcher watcher = new FileSystemWatcher(path, filter);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;

            var watcherData = new WatcherData(watcher, handler);
            watchers.Add(watcherData);
            return true;
        }

        public static bool AddWatcher(string path, Action<string, FileSystemEventArgs> handler)
        {
            return AddWatcher(path, "*.*", handler);
        }

        public static bool RemoveWatcher(string path)
        {
            for (int i = 0; i < watchers.Count; i++)
            {
                if (watchers[i].Watcher.Path == path)
                {
                    watchers[i].Watcher.Dispose();
                    watchers.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private static void TryInvoke(object sender, FileSystemEventArgs e)
        {
            foreach (var item in watchers)
            {
                if (item.Watcher == sender)
                {
                    item.Handler?.Invoke(item.Watcher.Path, e);
                    break;
                }
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            TryInvoke(sender, e);
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            TryInvoke(sender, e);
        }
    }
}