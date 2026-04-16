using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoreEditor
{
    public static class AttributeUtils
    {
        /// <summary>
        /// 获取Assembly 被约束的所有类型
        /// </summary>
        /// <param name="assembly">指定Assembly</param>
        /// <typeparam name="T">约束的类型</typeparam>
        /// <returns></returns>
        public static List<Type> GetTypes<T>(Assembly assembly) where T : class
        {
            List<Type> types = new List<Type>();
            Type[] allTypes = assembly.GetTypes();
            foreach (Type item in allTypes)
            {
                if (typeof(T).IsAssignableFrom(item))
                {
                    types.Add(item);
                }
            }

            return types;
        }

        public static List<Type> GetTypes<T>() where T : class
        {
            return GetTypes<Type>(Assembly.GetCallingAssembly());
        }

        /// <summary>
        ///  获取Assembly 被约束的所有实例
        /// </summary>
        /// <param name="assembly">指定Assembly</param>
        /// <typeparam name="T">约束的类型</typeparam>
        /// <returns></returns>
        public static List<T> CreateInstances<T>(Assembly assembly) where T : class
        {
            List<T> instances = new List<T>();
            List<Type> types = GetTypes<T>(assembly);
            foreach (var item in types)
            {
                if (!item.IsAbstract && !item.IsInterface)
                {
                    object instance = Activator.CreateInstance(item);
                    instances.Add((T)instance);
                }
            }

            return instances;
        }

        public static List<T> CreateInstances<T>() where T : class
        {
            return CreateInstances<T>(Assembly.GetCallingAssembly());
        }


        /// <summary>
        /// 获取Assembly 被约束特性下的类型
        /// </summary>
        /// <param name="assembly">指定Assembly</param>
        /// <typeparam name="A">约束的特性</typeparam>
        /// <returns></returns>
        public static List<Type> GetTypesByAttribute<A>(Assembly assembly) where A : Attribute
        {
            List<Type> types = new List<Type>();
            Type[] allTypes = assembly.GetTypes();
            foreach (Type item in allTypes)
            {
                var attribute = item.GetCustomAttribute<A>();
                if (null != attribute)
                {
                    types.Add(item);
                }
            }

            return types;
        }

        public static List<Type> GetTypesByAttribute<A>() where A : Attribute
        {
            return GetTypesByAttribute<A>(Assembly.GetCallingAssembly());
        }


        /// <summary>
        /// 根绝约束类型和特性获取
        /// </summary>
        /// <typeparam name="T">约束类型</typeparam>
        /// <typeparam name="A">约束特性</typeparam>
        /// <returns></returns>
        public static Dictionary<Type, A> GetTypeAttributeInfo<T, A>(Assembly assembly)
            where T : class where A : Attribute
        {
            Dictionary<Type, A> resMap = new Dictionary<Type, A>();
            Type[] allTypes = assembly.GetTypes();
            foreach (Type item in allTypes)
            {
                var attribute = item.GetCustomAttribute<A>();
                if (null != attribute && typeof(T).IsAssignableFrom(item))
                {
                    resMap[item] = attribute;
                }
            }

            return resMap;
        }

        public static Dictionary<Type, A> GetTypeAttributeInfo<T, A>() where T : class where A : Attribute
        {
            Dictionary<Type, A> map = new Dictionary<Type, A>();
            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            assemblies.Add(Assembly.GetCallingAssembly());
            assemblies.Add(Assembly.Load("Assembly-CSharp-Editor"));
            foreach (var item in assemblies)
            {
                if (null != item)
                {
                    var resMap = GetTypeAttributeInfo<T, A>(item);
                    foreach (var attr in resMap)
                    {
                        map[attr.Key] = attr.Value;
                    }
                }
            }

            return map;
        }

        public static void GetInstancesAttribute<T, A>(Assembly assembly, out List<T> insts, out List<A> attris)
            where T : class where A : Attribute
        {
            insts = new List<T>();
            attris = new List<A>();
            Dictionary<Type, A> types = GetTypeAttributeInfo<T, A>(assembly);
            foreach (var item in types)
            {
                if (!item.Key.IsAbstract && !item.Key.IsInterface)
                {
                    T obj = (T)Activator.CreateInstance(item.Key);
                    insts.Add(obj);
                    attris.Add(item.Value);
                }
            }
        }

        public static void GetInstancesAttribute<T, A>(out List<T> insts, out List<A> attris)
            where T : class where A : Attribute
        {
            insts = new List<T>();
            attris = new List<A>();

            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            assemblies.Add(Assembly.GetCallingAssembly());
            assemblies.Add(Assembly.Load("Assembly-CSharp-Editor"));

            foreach (var item in assemblies)
            {
                if (null != item)
                {
                    GetInstancesAttribute<T, A>(item, out var itemInsts, out var itemAttris);

                    insts.AddRange(itemInsts);
                    attris.AddRange(itemAttris);
                }
            }
        }

        /// <summary>
        /// 获取Assembly 被约束特性以及被约束类型的类型
        /// </summary>
        /// <param name="assembly">指定Assembly</param>
        /// <typeparam name="T">约束类型</typeparam>
        /// <typeparam name="A">约束特性</typeparam>
        /// <returns></returns>
        public static List<Type> GetTypesByAttribute<T, A>(Assembly assembly) where T : class where A : Attribute
        {
            List<Type> types = new List<Type>();
            var map = GetTypeAttributeInfo<T, A>(assembly);
            types.AddRange(map.Keys);
            return types;
        }

        public static List<Type> GetTypesByAttribute<T, A>() where T : class where A : Attribute
        {
            return GetTypesByAttribute<T, A>(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// 获取Assembly 被约束特性以及被约束类型的实例
        /// </summary>
        /// <param name="assembly">指定Assembly</param>
        /// <typeparam name="T">约束类型</typeparam>
        /// <typeparam name="A">约束特性</typeparam>
        /// <returns></returns>
        public static List<T> GetInstancesByAttribute<T, A>(Assembly assembly) where T : class where A : Attribute
        {
            List<T> instances = new List<T>();
            List<Type> types = GetTypesByAttribute<T, A>(assembly);
            foreach (var item in types)
            {
                if (!item.IsAbstract && !item.IsInterface)
                {
                    T obj = (T)Activator.CreateInstance(item);
                    instances.Add(obj);
                }
            }

            return instances;
        }

        public static List<T> GetInstancesByAttribute<T, A>() where T : class where A : Attribute
        {
            return GetInstancesByAttribute<T, A>(Assembly.GetCallingAssembly());
        }
    }
}