using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core.Data
{
    public class DataManager : Singleton<DataManager>
    {
        private Dictionary<string, IDataable> dataMap = new Dictionary<string, IDataable>();

        public void RegisterData(Type type)
        {
            string name = type.Name;
            bool needCreate = false;
            IDataable data = null;
            if (dataMap.TryGetValue(name, out data))
            {
                if (data.GetType() != type)
                {
                    needCreate = true;
                }
            }
            else
            {
                needCreate = true;
            }

            if (needCreate)
            {
                data = (IDataable)Activator.CreateInstance(type);
                data.Initial(data);
                dataMap[name] = data;
            }
        }

        public void RegisterData<T>() where T : IDataable
        {
            RegisterData(typeof(T));
        }

        public void ClearData(Type type)
        {
            string name = type.Name;
            if (dataMap.TryGetValue(name, out var data))
            {
                data.Clear();
            }
        }

        public void ClearData<T>() where T : IDataable
        {
            ClearData(typeof(T));
        }

        public void ClearDatas(Type[] types)
        {
            foreach (var item in types)
            {
                ClearData(item);
            }
        }
        
        public void ClearAll()
        {
            foreach (var item in dataMap)
            {
                item.Value.Clear();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var item in dataMap)
            {
                item.Value.Dispose();
            }

            dataMap = null;
        }
    }
}