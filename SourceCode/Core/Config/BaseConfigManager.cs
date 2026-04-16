using System;
using System.Collections.Generic;
#if FLATBUFFERS
using Google.FlatBuffers;

#endif

namespace Core.Config
{
    public abstract class BaseConfigManager<M, G> : Singleton<M> where M : Singleton<M> where G : BaseConfigGroup
    {
        private Dictionary<string, G> groupMap;
        protected G def;

        protected override void OnInitial()
        {
            base.OnInitial();
            groupMap = new Dictionary<string, G>();
            def = CreateDefaultConfigGroup();
        }

        protected abstract G CreateDefaultConfigGroup();

        protected abstract G CrateConfigGroup(string name);

        public G GetGroup(string name)
        {
            if (groupMap.TryGetValue(name, out var group))
            {
                group = CrateConfigGroup(name);
                groupMap.Add(name, group);
            }

            return group;
        }

        public void UnloadGroup(string name)
        {
            if (groupMap.TryGetValue(name, out var group))
            {
                group.Dispose();
                groupMap.Remove(name);
            }
        }

        public void SetTypeNameFunc(Func<Type, string> func)
        {
            def.SetTypeNameFunc(func);
        }

        public void ClearTypeNameCache()
        {
            def.ClearTypeNameCache();
        }

        public bool TryGetConfig<T>(string name, out T config)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return def.TryGetConfig(name, out config);
        }

        public bool TryGetConfig<T>(out T config)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return def.TryGetConfig(out config);
        }

        public bool TryGetUnit<T>(string name, int id, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return def.TryGetUnit(name, id, out unit);
        }

        public bool TryGetUnit<T>(int id, out T unit)
#if FLATBUFFERS
            where T : IFlatbufferObject
#endif
        {
            return def.TryGetUnit(id, out unit);
        }

        public T? GetConfig<T>(string name)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.GetConfig<T>(name);
        }

        public T? GetConfig<T>()
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.GetConfig<T>();
        }

        public T? GetUnit<T>(string name, int id)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.GetUnit<T>(name, id);
        }

        public T? GetUnit<T>(int id)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.GetUnit<T>(id);
        }

        public bool TryGetAllUnits<T>(string name, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.TryGetAllUnits<T>(name, ref units);
        }

        public bool TryGetAllUnits<T>(ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.TryGetAllUnits<T>(ref units);
        }

        public bool HasConfig(string name)
        {
            return def.HasConfig(name);
        }

        public bool HasConfig(Type type)
        {
            return def.HasConfig(type);
        }

        public bool HasConfig<T>()
        {
            return def.HasConfig<T>();
        }

        public bool FindUnit<T>(string name, Func<T, bool> expression, out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(name, expression, out unit);
        }

        public bool FindUnit<T>(Func<T, bool> expression, out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(expression, out unit);
        }

        public bool FindUnit<T, T1>(string name, Func<T, T1, bool> expression, T1 t1, out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(name, expression, t1, out unit);
        }

        public bool FindUnit<T, T1>(Func<T, T1, bool> expression, T1 t1, out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(expression, t1, out unit);
        }

        public bool FindUnit<T, T1, T2>(string name, Func<T, T1, T2, bool> expression, T1 t1, T2 t2, out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(name, expression, t1, t2, out unit);
        }

        public bool FindUnit<T, T1, T2>(Func<T, T1, T2, bool> expression, T1 t1, T2 t2, out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(expression, t1, t2, out unit);
        }


        public bool FindUnit<T, T1, T2, T3>(string name, Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(name, expression, t1, t2, t3, out unit);
        }

        public bool FindUnit<T, T1, T2, T3>(Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            out T unit)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnit(expression, t1, t2, t3, out unit);
        }

        public bool FindUnits<T>(string name, Func<T, bool> expression, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(name, expression, ref units);
        }

        public bool FindUnits<T>(Func<T, bool> expression, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(expression, ref units);
        }

        public bool FindUnits<T, T1>(string name, Func<T, T1, bool> expression, T1 t1, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(name, expression, t1, ref units);
        }

        public bool FindUnits<T, T1>(Func<T, T1, bool> expression, T1 t1, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(expression, t1, ref units);
        }

        public bool FindUnits<T, T1, T2>(string name, Func<T, T1, T2, bool> expression, T1 t1, T2 t2, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(name, expression, t1, t2, ref units);
        }

        public bool FindUnits<T, T1, T2>(Func<T, T1, T2, bool> expression, T1 t1, T2 t2, ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(expression, t1, t2, ref units);
        }

        public bool FindUnits<T, T1, T2, T3>(string name, Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(name, expression, t1, t2, t3, ref units);
        }

        public bool FindUnits<T, T1, T2, T3>(Func<T, T1, T2, T3, bool> expression, T1 t1, T2 t2, T3 t3,
            ref List<T> units)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            return def.FindUnits(expression, t1, t2, t3, ref units);
        }

        public void Foreach<T>(string name, Action<T> func)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            def.Foreach(name, func);
        }

        public void Foreach<T>(Action<T> func)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            def.Foreach(func);
        }

        public void UnloadConfig(string name)
        {
            def.UnloadConfig(name);
        }

        public void UnloadConfig(Type type)
        {
            def.UnloadConfig(type);
        }

        public void UnloadConfigs(IEnumerable<string> names)
        {
            def.UnloadConfigs(names);
        }

        public void UnloadConfigs(IEnumerable<Type> types)
        {
            def.UnloadConfigs(types);
        }

        public void UnloadAllConfigs()
        {
            def.UnloadAllConfigs();
        }

        public void SetDontUnloadConfig(string name, bool dontUnload)
        {
            def.SetDontUnloadConfig(name, dontUnload);
        }

        public void SetDontUnloadConfig(Type type, bool dontUnload)
        {
            def.SetDontUnloadConfig(type, dontUnload);
        }

        public void SetDontUnloadConfig<T>(bool dontUnload)
#if FLATBUFFERS
            where T : struct, IFlatbufferObject
#endif
        {
            def.SetDontUnloadConfig<T>(dontUnload);
        }

        public void SetDontUnloadConfigs(IEnumerable<string> names, bool dontUnload)
        {
            def.SetDontUnloadConfigs(names, dontUnload);
        }

        public void SetDontUnloadConfigs(IEnumerable<Type> types, bool dontUnload)
        {
            def.SetDontUnloadConfigs(types, dontUnload);
        }

        public void ClearDontUnloadConfigs()
        {
            def.ClearDontUnloadConfigs();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var item in groupMap)
            {
                item.Value.Dispose();
            }

            def.Dispose();

            groupMap = null;
            def = null;
        }
    }
}