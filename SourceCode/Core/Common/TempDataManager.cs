using System.Collections.Generic;

namespace Core
{
    public class TempDataManager : Singleton<TempDataManager>
    {
        private Dictionary<string, object> dataMap = new Dictionary<string, object>();

        public void SetData(string key, object data)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            dataMap[key] = data;
        }

        public void RemoveData(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            dataMap.Remove(key);
        }

        public object GetData(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (dataMap.TryGetValue(key, out var data))
                {
                    return data;
                }
            }

            return null;
        }

        public T GetData<T>(string key)
        {
            return (T)GetData(key);
        }

        public void ClearAll()
        {
            dataMap.Clear();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            dataMap = null;
        }
    }
}