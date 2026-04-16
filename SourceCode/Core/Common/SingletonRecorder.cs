using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SingletonRecorder
    {
        private static readonly List<IDisposable> instances = new List<IDisposable>();

        internal static void Add(IDisposable instance)
        {
            instances.Add(instance);
        }

        public static void Clear()
        {
            foreach (var item in instances)
            {
                item.Dispose();
            }
            instances.Clear();
        }
    }
}