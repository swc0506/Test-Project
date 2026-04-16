using System;
using System.Collections.Generic;
using UnityEngine;
#if FLATBUFFERS
using Google.FlatBuffers;

#endif

namespace Core.Config
{
    public abstract class BaseConfigGroup
    {
#if FLATBUFFERS
        private Dictionary<string, FlatbufferTable> configMap = new Dictionary<string, FlatbufferTable>();
#else
        private Dictionary<string, object> configMap = new Dictionary<string, object>();
#endif

        private Func<Type, string> typeNameFunc;
        private Dictionary<Type, string> typeNameMap = new Dictionary<Type, string>();
        private Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();

        protected HashSet<string> dontUnloadSet = new HashSet<string>();

        protected Action<string, Type> lazyLoadAction;

        public void SetTypeNameFunc(Func<Type, string> func)
        {
            typeNameFunc = func;
        }

        public void ClearTypeNameCache()
        {
            typeNameMap.Clear();
            typeMap.Clear();
        }

        protected string GetTypeName(Type type)
        {
            string name = null;
            if (null != type && !typeNameMap.TryGetValue(type, out name))
            {
                if (null != typeNameFunc)
                {
                    name = typeNameFunc.Invoke(type);
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = type.Name;
                }

                typeNameMap.Add(type, name);
            }

            return name;
        }

        private Type GetCfgTypeByUnitType(Type unitType)
        {
            Type cfgType = null;
            if (!typeMap.TryGetValue(unitType, out cfgType))
            {
                string fullName = unitType.FullName;
                int index = fullName.LastIndexOf("Unit");
                if (index > 0)
                {
                    cfgType = unitType.Assembly.GetType(fullName.Substring(0, index));
                }

                typeMap.Add(unitType, cfgType);
            }

            return cfgType;
        }

        private string GetCfgTypeNameByUnitType(Type unitType)
        {
            Type cfgType = GetCfgTypeByUnitType(unitType);
            return GetTypeName(cfgType);
        }

        private Type GetUnitTypeByCfgType(Type cfgType)
        {
            Type unitType = null;
            if (!typeMap.TryGetValue(cfgType, out unitType))
            {
                string fullName = cfgType.FullName + "Unit";
                unitType = cfgType.Assembly.GetType(fullName);
                typeMap.Add(cfgType, unitType);
            }

            return unitType;
        }


        protected void CreateConfig(string name, Type type, ByteBufferAllocator bufferAllocator)
        {
            if (bufferAllocator.Length == 0)
            {
                return;
            }
#if FLATBUFFERS
            ByteBuffer bb = new ByteBuffer(bufferAllocator, 0);
            FlatbufferTable table = new FlatbufferTable();
            Type unitType = GetUnitTypeByCfgType(type);
            table.Initial(type, unitType, bb);
            configMap[name] = table;
#endif
        }

        protected void CreateConfig(string name, Type type, byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return;
            }
#if FLATBUFFERS
            ByteBuffer bb = new ByteBuffer(bytes);
            FlatbufferTable table = new FlatbufferTable();
            Type unitType = GetUnitTypeByCfgType(type);
            table.Initial(type, unitType, bb);
            configMap[name] = table;
#endif
        }

        protected void CreateConfig(Type type, byte[] bytes)
        {
            CreateConfig(GetTypeName(type), type, bytes);
        }


        #region Get

        public bool TryGetConfig<T>(string name, out T config)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            if (!string.IsNullOrEmpty(name))
            {
#if FLATBUFFERS
                if (!configMap.TryGetValue(name, out var table))
                {
                    if (null != lazyLoadAction)
                    {
                        lazyLoadAction.Invoke(name, typeof(T));
                        configMap.TryGetValue(name, out table);
                    }
                }

                if (null != table)
                {
                    if (table.TryGetConfig<T>(out config))
                    {
                        return true;
                    }
                }
            }
#endif
            Logger.WarnFormat("Config  is null,name:{0}", name);
            config = default(T);
            return false;
        }

        public bool TryGetConfig<T>(out T config)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return TryGetConfig(GetTypeName(typeof(T)), out config);
        }


#if FLATBUFFERS
        private FlatbufferTable InternalGetTableByUnit<T>(string name) where T : IFlatbufferObject
#else
        private object InternalGetTableByUnit<T>(string name)
#endif
        {
            if (!string.IsNullOrEmpty(name))
            {
#if FLATBUFFERS
                if (!configMap.TryGetValue(name, out var table))
                {
                    if (null != lazyLoadAction)
                    {
                        Type cfgType = GetCfgTypeByUnitType(typeof(T));
                        if (null != cfgType)
                        {
                            lazyLoadAction.Invoke(name, cfgType);
                            configMap.TryGetValue(name, out table);
                        }
                    }
                }

                return table;
            }
#endif
            return null;
        }

        public bool TryGetUnit<T>(string name, int id, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                if (table.TryGetUnit<T>(id, out unit))
                {
                    return true;
                }
#endif
            }

            if (id != 0)
            {
                Logger.WarnFormat("Config unit is null,name:{0},id:{1}", name, id);
            }

            unit = default(T);
            return false;
        }

        public bool TryGetUnit<T>(int id, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return TryGetUnit(GetCfgTypeNameByUnitType(typeof(T)), id, out unit);
        }


        public T? GetConfig<T>(string name)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            if (TryGetConfig<T>(name, out T cfg))
            {
                return cfg;
            }

            return null;
        }

        public T? GetConfig<T>()
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return GetConfig<T>(GetTypeName(typeof(T)));
        }

        public T? GetUnit<T>(string name, int id)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            if (TryGetUnit<T>(name, id, out T unit))
            {
                return unit;
            }

            return null;
        }

        public T? GetUnit<T>(int id)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return GetUnit<T>(GetCfgTypeNameByUnitType(typeof(T)), id);
        }

        public bool TryGetAllUnits<T>(string name, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            units?.Clear();
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (null == units)
                    {
                        units = new List<T>();
                    }

                    units.Add((T)item.Value);
                }
#endif
            }

            return null != units && units.Count > 0;
        }

        public bool TryGetAllUnits<T>(ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return TryGetAllUnits(GetCfgTypeNameByUnitType(typeof(T)), ref units);
        }

        #endregion

        #region HasConfig

        public bool HasConfig(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return configMap.ContainsKey(name);
        }

        public bool HasConfig(Type type)
        {
            return HasConfig(GetTypeName(type));
        }

        public bool HasConfig<T>()
        {
            return HasConfig(typeof(T));
        }

        #endregion

        #region FindUnit

        public bool FindUnit<T>(string name, Func<T, bool> expression, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value))
                    {
                        unit = (T)item.Value;
                        return true;
                    }
                }
#endif
            }

            unit = default(T);
            return false;
        }

        public bool FindUnit<T>(Func<T, bool> expression, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnit(GetCfgTypeNameByUnitType(typeof(T)), expression, out unit);
        }


        public bool FindUnit<T, T1>(string name, Func<T, T1, bool> expression, T1 t1, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value, t1))
                    {
                        unit = (T)item.Value;
                        return true;
                    }
                }
#endif
            }

            unit = default(T);
            return false;
        }

        public bool FindUnit<T, T1>(Func<T, T1, bool> expression, T1 t1, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnit(GetCfgTypeNameByUnitType(typeof(T)), expression, t1, out unit);
        }


        public bool FindUnit<T, T1, T2>(string name, Func<T, T1, T2, bool> expression, T1 t1, T2 t2, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value, t1, t2))
                    {
                        unit = (T)item.Value;
                        return true;
                    }
                }
#endif
            }

            unit = default(T);
            return false;
        }

        public bool FindUnit<T, T1, T2>(Func<T, T1, T2, bool> expression, T1 t1, T2 t2, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnit(GetCfgTypeNameByUnitType(typeof(T)), expression, t1, t2, out unit);
        }


        public bool FindUnit<T, T1, T2, T3>(string name, Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value, t1, t2, t3))
                    {
                        unit = (T)item.Value;
                        return true;
                    }
                }
#endif
            }

            unit = default(T);
            return false;
        }

        public bool FindUnit<T, T1, T2, T3>(Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnit(GetCfgTypeNameByUnitType(typeof(T)), expression, t1, t2, t3, out unit);
        }


        public bool FindUnits<T>(string name, Func<T, bool> expression, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            units?.Clear();
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value))
                    {
                        if (null == units)
                        {
                            units = new List<T>();
                        }

                        units.Add((T)item.Value);
                    }
                }
#endif
            }

            return null != units && units.Count > 0;
        }

        public bool FindUnits<T>(Func<T, bool> expression, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnits(GetCfgTypeNameByUnitType(typeof(T)), expression, ref units);
        }


        public bool FindUnits<T, T1>(string name, Func<T, T1, bool> expression, T1 t1, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            units?.Clear();
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value, t1))
                    {
                        if (null == units)
                        {
                            units = new List<T>();
                        }

                        units.Add((T)item.Value);
                    }
                }
#endif
            }

            return null != units && units.Count > 0;
        }

        public bool FindUnits<T, T1>(Func<T, T1, bool> expression, T1 t1, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnits(GetCfgTypeNameByUnitType(typeof(T)), expression, t1, ref units);
        }

        public bool FindUnits<T, T1, T2>(string name, Func<T, T1, T2, bool> expression, T1 t1, T2 t2, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            units?.Clear();
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value, t1, t2))
                    {
                        if (null == units)
                        {
                            units = new List<T>();
                        }

                        units.Add((T)item.Value);
                    }
                }
#endif
            }

            return null != units && units.Count > 0;
        }

        public bool FindUnits<T, T1, T2>(Func<T, T1, T2, bool> expression, T1 t1, T2 t2, ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnits(GetCfgTypeNameByUnitType(typeof(T)), expression, t1, t2, ref units);
        }


        public bool FindUnits<T, T1, T2, T3>(string name, Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            units?.Clear();
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    if (expression.Invoke((T)item.Value, t1, t2, t3))
                    {
                        if (null == units)
                        {
                            units = new List<T>();
                        }

                        units.Add((T)item.Value);
                    }
                }
#endif
            }

            return null != units && units.Count > 0;
        }

        public bool FindUnits<T, T1, T2, T3>(Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            ref List<T> units)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return FindUnits(GetCfgTypeNameByUnitType(typeof(T)), expression, t1, t2, t3, ref units);
        }

        public void Foreach<T>(string name, Action<T> func)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            var table = InternalGetTableByUnit<T>(name);
            if (null != table)
            {
#if FLATBUFFERS
                foreach (var item in table)
                {
                    func.Invoke((T)item.Value);
                }
#endif
            }
        }

        public void Foreach<T>(Action<T> func)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            Foreach(GetCfgTypeNameByUnitType(typeof(T)), func);
        }

        #endregion

        #region Unload

        public virtual void UnloadConfig(string name)
        {
            dontUnloadSet.Remove(name);
            RemoveConfig(name);
        }

        private void RemoveConfig(string name)
        {
            if (configMap.TryGetValue(name, out var config))
            {
                configMap.Remove(name);
                if (config is IClearable clearable)
                {
                    clearable.Clear();
                }
            }
        }

        public void UnloadConfig(Type type)
        {
            UnloadConfig(GetTypeName(type));
        }

        public void UnloadConfigs(IEnumerable<string> names)
        {
            foreach (var item in names)
            {
                UnloadConfig(item);
            }
        }

        public void UnloadConfigs(IEnumerable<Type> types)
        {
            foreach (var item in types)
            {
                UnloadConfig(item);
            }
        }

        public virtual void UnloadAllConfigs()
        {
            List<string> removes = new List<string>();
            foreach (var item in configMap)
            {
                if (!dontUnloadSet.Contains(item.Key))
                {
                    removes.Add(item.Key);
                }
            }

            foreach (var item in removes)
            {
                RemoveConfig(item);
            }
        }

        #endregion

        #region DontUnload

        public void SetDontUnloadConfig(string name, bool dontUnload)
        {
            if (dontUnload)
            {
                dontUnloadSet.Add(name);
            }
            else
            {
                dontUnloadSet.Remove(name);
            }
        }

        public void SetDontUnloadConfig(Type type, bool dontUnload)
        {
            SetDontUnloadConfig(GetTypeName(type), dontUnload);
        }

        public void SetDontUnloadConfig<T>(bool dontUnload)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            SetDontUnloadConfig(typeof(T), dontUnload);
        }

        public void SetDontUnloadConfigs(IEnumerable<string> names, bool dontUnload)
        {
            foreach (var item in names)
            {
                SetDontUnloadConfig(item, dontUnload);
            }
        }

        public void SetDontUnloadConfigs(IEnumerable<Type> types, bool dontUnload)
        {
            foreach (var item in types)
            {
                SetDontUnloadConfig(item, dontUnload);
            }
        }

        public void ClearDontUnloadConfigs()
        {
            dontUnloadSet.Clear();
        }

        #endregion


        public virtual void Dispose()
        {
            UnloadAllConfigs();
            configMap = null;
            typeNameFunc = null;
            typeNameMap = null;
            typeMap = null;
            dontUnloadSet = null;
        }
    }
}