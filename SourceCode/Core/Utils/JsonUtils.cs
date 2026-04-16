using System;
using LitJson;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 可以把自定义类型绑定到JsonMapper，这样使用就能序列化和反序列化了
    /// </summary>
    static class CustomeTypeBindings
    {
        private static void WriteProperty(this JsonWriter w, string name, float value)
        {
            w.WritePropertyName(name);
            w.Write(value);
        }

        public static void Register()
        {
            JsonMapper.RegisterImporter<string, Type>((s) => { return Type.GetType(s); });
            JsonMapper.RegisterExporter<Type>((v, w) => { w.Write(v.FullName); });

            Action<Vector3, JsonWriter> writeVector3 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            };

            //Vector2
            JsonMapper.RegisterExporter<Vector2>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            });

            //Vector2Int
            JsonMapper.RegisterExporter<Vector2Int>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            });

            //Vector3
            JsonMapper.RegisterExporter<Vector3>((v, w) => { writeVector3(v, w); });

            //Vector3Int
            JsonMapper.RegisterExporter<Vector3Int>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            });

            //Vector4
            JsonMapper.RegisterExporter<Vector4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            //Quaternion
            JsonMapper.RegisterExporter<Quaternion>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            //Color
            JsonMapper.RegisterExporter<Color>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            //Color32
            JsonMapper.RegisterExporter<Color32>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            //Rect
            JsonMapper.RegisterExporter<Rect>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("width", v.width);
                w.WriteProperty("height", v.height);
                w.WriteObjectEnd();
            });

            //RectOffset
            JsonMapper.RegisterExporter<RectOffset>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("top", v.top);
                w.WriteProperty("left", v.left);
                w.WriteProperty("bottom", v.bottom);
                w.WriteProperty("right", v.right);
                w.WriteObjectEnd();
            });

            //Bounds
            JsonMapper.RegisterExporter<Bounds>((v, w) =>
            {
                w.WriteObjectStart();
                w.WritePropertyName("center");
                writeVector3(v.center, w);
                w.WritePropertyName("size");
                writeVector3(v.size, w);
                w.WriteObjectEnd();
            });

            //SecureInt
            JsonMapper.RegisterExporter<SecureInt>((v, w) => { w.Write(v.Value); });
            //SecureFloat
            JsonMapper.RegisterExporter<SecureFloat>((v, w) => { w.Write(v.Value); });
        }
    }

    public static class JsonUtils
    {
        /*JsonMapper为了支持int为key,有修改源码
         修改部分为 private static object ReadValue(Type inst_type, JsonReader reader) 方法
         
         Type[] mapTypes = instance.GetType().GetGenericArguments();
         TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(mapTypes[0]);
         property = converter.ConvertFromString((string)property);
         t_data.ElementType = mapTypes[1];
       
         ((IDictionary)instance).Add(
             property, ReadValue(
                t_data.ElementType, reader));
         */
        private static JsonWriter prettyWriter;

        static JsonUtils()
        {
            CustomeTypeBindings.Register();
        }

        /// <summary>
        /// 将对象序列化为JSON字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="isPretty">是否美化格式</param>
        /// <returns>序列化后的JSON字符串</returns>
        public static string ToJson(object obj, bool isPretty)
        {
            if (null == obj)
            {
                return null;
            }

            if (!isPretty)
            {
                return JsonMapper.ToJson(obj);
            }
            else
            {
                if (null == prettyWriter)
                {
                    prettyWriter = new JsonWriter { PrettyPrint = true };
                }
                else
                {
                    prettyWriter.Reset();
                }

                JsonMapper.ToJson(obj, prettyWriter);
                return prettyWriter.ToString();
            }
        }

        public static string ToJson(object obj)
        {
            return ToJson(obj, false);
        }

        /// <summary>
        /// 将JSON字符串反序列化为对象
        /// </summary>
        /// <param name="json">要反序列化的JSON字符串</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>反序列化后的对象</returns>
        public static T ToObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            return JsonMapper.ToObject<T>(json);
        }

        /// <summary>
        /// 将JSON字符串反序列化为对象
        /// </summary>
        /// <param name="json">要反序列化的JSON字符串</param>
        /// <param name="type">要反序列化的类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object ToObject(string json, Type type)
        {
            if (string.IsNullOrEmpty(json) || null == type)
            {
                return null;
            }

            return JsonMapper.ToObject(json, type);
        }

        /// <summary>
        /// 将JSON字符串反序列化为对象
        /// </summary>
        /// <param name="json">要反序列化的JSON字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static JsonData ToObject(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonMapper.ToObject(json);
        }
    }
}