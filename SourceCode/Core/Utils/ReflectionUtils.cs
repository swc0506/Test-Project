using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core
{
    public static class ReflectionUtils
    {
        public static object GetPropertyValue(object obj, string fieldName)
        {
            Type type = obj.GetType();
            PropertyInfo info = type.GetProperty(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (null != info)
            {
                object value = info.GetValue(obj, null);
                return value;
            }

            return null;
        }

        public static bool SetPropertyValue(object obj, string fieldName, object value)
        {
            Type type = obj.GetType();
            PropertyInfo info = type.GetProperty(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (null != info)
            {
                object val = Convert.ChangeType(value, info.PropertyType);
                info.SetValue(obj, val, null);
                return true;
            }

            return false;
        }

        public static object GetFieldValue(object obj, string fieldName)
        {
            Type type = obj.GetType();
            FieldInfo info = type.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (null != info)
            {
                object value = info.GetValue(obj);
                return value;
            }

            return null;
        }

        public static void SetFieldValue(object obj, string fieldName, object value)
        {
            Type type = obj.GetType();
            FieldInfo info = type.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (null != info)
            {
                object val = Convert.ChangeType(value, info.FieldType);
                info.SetValue(obj, val);
            }
        }

        public static void CopyPropertyFieldInfo(object src, object des, BindingFlags flags)
        {
            if (null == src || null == des)
            {
                return;
            }

            Type srcType = src.GetType();
            Type desType = des.GetType();
            if (srcType.Name != desType.Name)
            {
                return;
            }


            FieldInfo[] srcFiledInfos = srcType.GetFields(flags);
            FieldInfo[] desFiledInfos = desType.GetFields(flags);
            for (int i = 0; i < srcFiledInfos.Length; i++)
            {
                object srcVal = srcFiledInfos[i].GetValue(src);
                desFiledInfos[i].SetValue(des, srcVal);
            }

            PropertyInfo[] srcPropertyInfos = srcType.GetProperties(flags);
            PropertyInfo[] desPropertyInfos = desType.GetProperties(flags);
            for (int i = 0; i < srcPropertyInfos.Length; i++)
            {
                if (srcPropertyInfos[i].CanRead && srcPropertyInfos[i].CanWrite)
                {
                    object srcVal = srcPropertyInfos[i].GetValue(src);
                    desPropertyInfos[i].SetValue(des, srcVal);
                }
            }
        }

        public static void CopyPropertyFieldInfo(object src, object des)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            CopyPropertyFieldInfo(src, des, flags);
        }

        public static List<T> CollectEnum<T>() where T : struct
        {
            List<T> enums = new List<T>();
            FieldInfo[] fi = typeof(T).GetFields();
            foreach (var item in fi)
            {
                if (!item.FieldType.IsEnum)
                {
                    continue;
                }

                var attribute = item.GetCustomAttribute<ObsoleteAttribute>(false);
                if (null == attribute && Enum.TryParse(item.Name, out T value))
                {
                    enums.Add(value);
                }
            }

            return enums;
        }
    }
}