using Core.Storage;

namespace Core
{
    public static class LocalStorage
    {
        private static IStorable storage;

        static LocalStorage()
        {
            storage = new UnityStorage();
        }

        public static void SetStorage(IStorable storage)
        {
            LocalStorage.storage = storage;
        }

        public static void DeleteAll()
        {
            storage.DeleteAll();
        }

        public static void DeleteKey(string key)
        {
            storage.DeleteKey(key);
        }

        public static bool HasKey(string key)
        {
            return storage.HasKey(key);
        }


        public static bool GetBool(string key, bool defValue = false)
        {
            return storage.GetBool(key, defValue);
        }

        public static float GetFloat(string key, float defValue = 0.0f)
        {
            return storage.GetFloat(key, defValue);
        }

        public static int GetInt(string key, int defValue = 0)
        {
            return storage.GetInt(key, defValue);
        }

        public static string GetString(string key, string defValue = null)
        {
            return storage.GetString(key, defValue);
        }

        public static T GetObject<T>(string key, T defValue = default(T))
        {
            return storage.GetObject(key, defValue);
        }

        public static void SetBool(string key, bool value)
        {
            storage.SetBool(key, value);
        }

        public static void SetFloat(string key, float value)
        {
            storage.SetFloat(key, value);
        }

        public static void SetInt(string key, int value)
        {
            storage.SetInt(key, value);
        }

        public static void SetString(string key, string value)
        {
            storage.SetString(key, value);
        }

        public static void SetObject<T>(string key, T value)
        {
            storage.SetObject(key, value);
        }
    }
}