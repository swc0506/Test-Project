using UnityEngine;

namespace Core.Storage
{
    internal class UnityStorage : IStorable
    {
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }
        
        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
        
        public void SetBool(string key, bool value)
        {
            int val = value ? 1 : 0;
            PlayerPrefs.SetInt(key, val);
            PlayerPrefs.Save();
        }

        public void SetObject<T>(string key, T value)
        {
            string json = JsonUtils.ToJson(value);
            if (!string.IsNullOrEmpty(json))
            {
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
        }
        
        public bool GetBool(string key, bool defValue)
        {
            int def = defValue ? 1 : 0;
            int val = PlayerPrefs.GetInt(key, def);
            return val == 1;
        }

        public float GetFloat(string key, float defValue)
        {
            return PlayerPrefs.GetFloat(key, defValue);
        }

        public int GetInt(string key, int defValue)
        {
            return PlayerPrefs.GetInt(key, defValue);
        }

        public string GetString(string key, string defValue)
        {
            return PlayerPrefs.GetString(key, defValue);
        }

        public T GetObject<T>(string key, T defValue)
        {
            string json=PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonUtils.ToObject<T>(json);
            }
            return defValue;
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}