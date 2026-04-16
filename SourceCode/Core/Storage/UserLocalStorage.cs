namespace Core
{
    public class UserLocalStorage
    {
        private static string uId;

        public static void SetUserId(string uId)
        {
            UserLocalStorage.uId = uId;
        }

        private static bool TryGetUserKey(string key, out string resultKey)
        {
            if (!string.IsNullOrEmpty(uId))
            {
                resultKey = key + "_" + uId;
                return true;
            }

            resultKey = null;
            return false;
        }

        public static void DeleteKey(string key)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                LocalStorage.DeleteKey(resultKey);
            }
        }

        public static bool HasKey(string key)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                return LocalStorage.HasKey(resultKey);
            }

            return false;
        }

        public static int GetInt(string key, int defValue = 0)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                return LocalStorage.GetInt(resultKey, defValue);
            }

            return defValue;
        }

        public static float GetFloat(string key, float defValue = 0.0f)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                return LocalStorage.GetFloat(resultKey, defValue);
            }

            return defValue;
        }

        public static bool GetBool(string key, bool defValue = false)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                return LocalStorage.GetBool(resultKey, defValue);
            }

            return defValue;
        }

        public static string GetString(string key, string defValue = null)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                return LocalStorage.GetString(resultKey, defValue);
            }

            return defValue;
        }

        public static T GetObject<T>(string key, T defValue = default(T))
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                return LocalStorage.GetObject<T>(resultKey, defValue);
            }

            return defValue;
        }


        public static void SetInt(string key, int value)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                LocalStorage.SetInt(resultKey, value);
            }
        }

        public static void SetFloat(string key, float value)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                LocalStorage.SetFloat(resultKey, value);
            }
        }

        public static void SetBool(string key, bool value)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                LocalStorage.SetBool(resultKey, value);
            }
        }

        public static void SetString(string key, string value)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                LocalStorage.SetString(resultKey, value);
            }
        }

        public static void SetObject<T>(string key, object value)
        {
            if (TryGetUserKey(key, out string resultKey))
            {
                LocalStorage.SetObject(resultKey, value);
            }
        }
    }
}