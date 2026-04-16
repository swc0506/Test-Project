using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core.Log
{
    public partial class Commander
    {
        private Action<string> print;
        private ParametersSplitter splitter;
        private ArgumentsParser parser;
        private Dictionary<CommandInfo, List<FunctionInfo>> commandMap;

        public ParametersSplitter Splitter
        {
            get { return splitter; }
        }

        public ArgumentsParser Parser
        {
            get { return parser; }
        }

        internal IEnumerable<CommandInfo> Infos
        {
            get { return commandMap.Keys; }
        }

        public Commander(Action<string> print)
        {
            this.print = print;
            splitter = new ParametersSplitter();
            parser = new ArgumentsParser();
            commandMap = new Dictionary<CommandInfo, List<FunctionInfo>>();
            RegisterUnityType();
        }

        private Dictionary<ConsoleMethodAttribute, MethodInfo> ScanInstanceMethods(object instance)
        {
            Dictionary<ConsoleMethodAttribute, MethodInfo> map = new Dictionary<ConsoleMethodAttribute, MethodInfo>();
            if (null != instance)
            {
                BindingFlags bindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo[] methodInfos = instance.GetType().GetMethods(bindFlags);
                foreach (var method in methodInfos)
                {
                    var command = method.GetCustomAttribute<ConsoleMethodAttribute>(false);
                    if (null != command)
                    {
                        map.Add(command, method);
                    }
                }
            }

            return map;
        }

        private Dictionary<ConsoleMethodAttribute, MethodInfo> ScanStaticMethods(Type type)
        {
            Dictionary<ConsoleMethodAttribute, MethodInfo> map = new Dictionary<ConsoleMethodAttribute, MethodInfo>();
            if (null != type)
            {
                BindingFlags bindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                MethodInfo[] methodInfos = type.GetMethods(bindFlags);
                foreach (var method in methodInfos)
                {
                    var command = method.GetCustomAttribute<ConsoleMethodAttribute>(false);
                    if (null != command)
                    {
                        map.Add(command, method);
                    }
                }
            }

            return map;
        }

        public void AddCommand(string command, string annotate, Action callback)
        {
            AddMethod(command, annotate, callback.Method, callback.Target);
        }

        public void AddCommand<T1>(string command, string annotate, Action<T1> callback)
        {
            AddMethod(command, annotate, callback.Method, callback.Target);
        }

        public void AddCommand<T1, T2>(string command, string annotate, Action<T1, T2> callback)
        {
            AddMethod(command, annotate, callback.Method, callback.Target);
        }

        public void AddCommand<T1, T2, T3>(string command, string annotate, Action<T1, T2, T3> callback)
        {
            AddMethod(command, annotate, callback.Method, callback.Target);
        }

        public void AddCommand<T1, T2, T3, T4>(string command, string annotate, Action<T1, T2, T3, T4> callback)
        {
            AddMethod(command, annotate, callback.Method, callback.Target);
        }

        public void AddCommand(string command, string annotate, Delegate callback)
        {
            AddMethod(command, annotate, callback.Method, callback.Target);
        }

        public void AddInstanceCommand(string command, string annotate, string methodName, object instance)
        {
            if (null != instance)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo method = instance.GetType().GetMethod(methodName, flags);
                if (null != method)
                {
                    AddMethod(command, annotate, method, instance);
                }
            }
        }

        public void AddInstanceCommands(object instance)
        {
            var map = ScanInstanceMethods(instance);
            foreach (var item in map)
            {
                AddMethod(item.Key.Command, item.Key.Annotate, item.Value, instance);
            }
        }

        public void AddStaticCommand(string command, string annotate, string methodName, Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo method = type.GetMethod(methodName, flags);
            if (null != method)
            {
                AddMethod(command, annotate, method, null);
            }
        }

        public void AddStaticCommands(Type type)
        {
            var map = ScanStaticMethods(type);
            foreach (var item in map)
            {
                AddMethod(item.Key.Command, item.Key.Annotate, item.Value, null);
            }
        }

        public void AddStaticCommands(Assembly assembly)
        {
            Type[] types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                AddStaticCommands(type);
            }
        }

        public void RemoveCommand(string command, Action callback)
        {
            RemoveMethod(command, callback.Method);
        }

        public void RemoveCommand<T1>(string command, Action<T1> callback)
        {
            RemoveMethod(command, callback.Method);
        }

        public void RemoveCommand<T1, T2>(string command, Action<T1, T2> callback)
        {
            RemoveMethod(command, callback.Method);
        }

        public void RemoveCommand<T1, T2, T3>(string command, Action<T1, T2, T3> callback)
        {
            RemoveMethod(command, callback.Method);
        }

        public void RemoveCommand<T1, T2, T3, T4>(string command, Action<T1, T2, T3, T4> callback)
        {
            RemoveMethod(command, callback.Method);
        }

        public void RemoveCommand(string command, Delegate callback)
        {
            RemoveMethod(command, callback.Method);
        }

        public void RemoveInstanceCommand(string command, string methodName, object instance)
        {
            if (null != instance)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo method = instance.GetType().GetMethod(methodName, flags);
                if (null != method)
                {
                    RemoveMethod(command, method);
                }
            }
        }

        public void RemoveStaticCommand(string command, string methodName, Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo method = type.GetMethod(methodName, flags);
            if (null != method)
            {
                RemoveMethod(command, method);
            }
        }

        public void RemoveInstanceCommands(object instance)
        {
            var map = ScanInstanceMethods(instance);
            foreach (var item in map)
            {
                RemoveMethod(item.Key.Command, item.Value);
            }
        }

        public void RemoveStaticCommands(Type type)
        {
            var map = ScanStaticMethods(type);
            foreach (var item in map)
            {
                RemoveMethod(item.Key.Command, item.Value);
            }
        }

        public void RemoveStaticCommands(Assembly assembly)
        {
            Type[] types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                RemoveStaticCommands(type);
            }
        }

        private bool CheckMethodParameter(string command, MethodInfo method)
        {
            if (null == method)
            {
                throw new ArgumentException(string.Format("{0}->console method can't null", command));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("console command name can't null or empty");
            }

            if (splitter.FindDelimiter(command, 0) < command.Length)
            {
                throw new ArgumentException(string.Format("{0}->console command name can't contain delimiter",
                    command));
            }

            ParameterInfo[] parameters = method.GetParameters();
            foreach (var item in parameters)
            {
                if (item.ParameterType.IsByRef)
                {
                    throw new ArgumentException(string.Format(
                        "{0}->console method {1} parameters can't have 'out' or 'ref'",
                        command, method));
                }

                if (!IsSupportParameterType(item.ParameterType))
                {
                    throw new ArgumentException(string.Format(
                        "{0}->console method {1} parameters type [{2}] isn't supported",
                        command, method, item.ParameterType));
                }
            }

            return true;
        }

        private bool CompareMethodSignature(string command, MethodInfo method, Type[] parameterTypes)
        {
            ParameterInfo[] parameters = method.GetParameters();
            bool isEqual = parameters.Length == parameterTypes.Length;
            if (isEqual)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            if (!isEqual)
            {
                throw new ArgumentException(string.Format(
                    "{0}->console method parameters signature is different {1}", command, method));
            }

            return isEqual;
        }

        public void AddMethod(string command, string annotate, MethodInfo method, object instance)
        {
            if (Application.isEditor && !CheckMethodParameter(command, method))
            {
                return;
            }

            ParameterInfo[] parameters = method.GetParameters();
            int length = parameters.Length;
            Type[] parameterTypes = new Type[length];
            string[] parameterNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
                parameterNames[i] = parameters[i].Name;
            }

            CommandInfo cmdInfo = new CommandInfo(command, annotate, parameterTypes, parameterNames);
            if (!commandMap.TryGetValue(cmdInfo, out var list))
            {
                list = new List<FunctionInfo>();
                commandMap.Add(cmdInfo, list);
            }

            if (Application.isEditor && list.Count > 0 &&
                !CompareMethodSignature(command, method, list[0].parameterTypes))
            {
                return;
            }

            FunctionInfo info = new FunctionInfo(method, instance);
            if (!list.Contains(info))
            {
                list.Add(info);
            }
        }

        public void RemoveMethod(string command, MethodInfo method)
        {
            if (Application.isEditor && !CheckMethodParameter(command, method))
            {
                return;
            }

            CommandInfo cmdInfo = new CommandInfo(command);
            if (commandMap.TryGetValue(cmdInfo, out var list))
            {
                for (int i = 0, length = list.Count; i < length; i++)
                {
                    if (list[i].method == method)
                    {
                        list.RemoveAt(i);
                        if (list.Count == 0)
                        {
                            commandMap.Remove(cmdInfo);
                        }

                        break;
                    }
                }
            }
        }

        private bool IsSupportParameterType(Type type)
        {
            return parser.ContainsParse(type) || IsSupportedArrayType(type) || type.IsEnum;
        }

        private bool IsSupportedArrayType(Type type)
        {
            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1)
                {
                    return false;
                }

                type = type.GetElementType();
            }

            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() != typeof(IList<>))
                {
                    return false;
                }

                type = type.GetGenericArguments()[0];
            }

            else
            {
                return false;
            }

            return IsSupportParameterType(type);
        }

        private bool TryParseArray(string input, Type arrayType, out object output)
        {
            output = null;
            if (!splitter.TrySplitParameters(input, out var args))
            {
                return false;
            }

            IList result = (IList) Activator.CreateInstance(arrayType, args.Count);
            if (arrayType.IsArray)
            {
                Type elementType = arrayType.GetElementType();
                for (int i = 0; i < args.Count; i++)
                {
                    if (!TryParseArgument(args[i], elementType, out var obj))
                    {
                        return false;
                    }

                    result[i] = obj;
                }
            }
            else if (arrayType.IsGenericType)
            {
                Type elementType = arrayType.GetGenericArguments()[0];
                for (int i = 0; i < args.Count; i++)
                {
                    if (!TryParseArgument(args[i], elementType, out var obj))
                    {
                        return false;
                    }

                    result.Add(obj);
                }
            }

            output = result;
            return true;
        }

        private bool TryParseEnum(string input, Type enumType, out object output)
        {
            const int NONE = 0, OR = 1, AND = 2;
            int outputInt = 0;
            int operation = 0;
            for (int i = 0; i < input.Length; i++)
            {
                string enumStr;
                int orIndex = input.IndexOf('|', i);
                int andIndex = input.IndexOf('&', i);
                if (orIndex < 0)
                {
                    enumStr = input.Substring(i, (andIndex < 0 ? input.Length : andIndex) - i);
                }
                else
                {
                    enumStr = input.Substring(i, (andIndex < 0 ? orIndex : Mathf.Min(andIndex, orIndex)) - i);
                }

                int value;
                if (!int.TryParse(enumStr, out value))
                {
                    if (Enum.IsDefined(enumType, enumStr))
                    {
                        value = Convert.ToInt32(Enum.Parse(enumType, enumStr));
                    }
                    else
                    {
                        output = null;
                        return false;
                    }
                }

                if (operation == NONE)
                {
                    outputInt = value;
                }
                else if (operation == OR)
                {
                    outputInt |= value;
                }
                else
                {
                    outputInt &= value;
                }

                if (orIndex >= 0)
                {
                    if (andIndex > orIndex)
                    {
                        operation = AND;
                        i = andIndex;
                    }
                    else
                    {
                        operation = OR;
                        i = orIndex;
                    }
                }
                else if (andIndex >= 0)
                {
                    operation = AND;
                    i = andIndex;
                }
                else
                {
                    i = input.Length;
                }
            }

            output = Enum.ToObject(enumType, outputInt);
            return true;
        }

        private bool TryParseArgument(string input, Type argumentType, out object output)
        {
            if (parser.TryParse(input, argumentType, out output))
            {
                return true;
            }

            if (IsSupportedArrayType(argumentType))
            {
                return TryParseArray(input, argumentType, out output);
            }

            if (argumentType.IsEnum)
            {
                return TryParseEnum(input, argumentType, out output);
            }

            output = null;
            return false;
        }

        public bool ExecuteCommand(string text)
        {
            if (!splitter.TrySplitParameters(text, out var args))
            {
                print?.Invoke(string.Format("console command input text error:{0}", text));
                return false;
            }

            CommandInfo cmdInfo = new CommandInfo(args[0]);
            if (!commandMap.TryGetValue(cmdInfo, out var list))
            {
                print?.Invoke(string.Format("can't find console command:{0}", cmdInfo.command));
                return false;
            }

            FunctionInfo functionInfo = list[0];
            int argsCount = args.Count - 1;
            if (argsCount != functionInfo.parameterTypes.Length)
            {
                print?.Invoke(string.Format(
                    "{0}->console command take parameters error,method parameters count is:{1},but input parameters count is :{2}",
                    args[0], functionInfo.parameterTypes.Length, argsCount));
                return false;
            }

            object[] parameters = new object[argsCount];
            for (int i = 0; i < argsCount; i++)
            {
                string arg = args[i + 1];
                Type parameterType = functionInfo.parameterTypes[i];
                try
                {
                    if (!TryParseArgument(arg, parameterType, out var val))
                    {
                        print?.Invoke(string.Format("{0}->console method argument:{1} parse type:{2} fail", args[0],
                            arg, parameterType));
                        return false;
                    }

                    parameters[i] = val;
                }
                catch (Exception e)
                {
                    print?.Invoke(
                        string.Format("{0}->console method parse argument exception:{1} ", args[0], e.Message));
                    return false;
                }
            }

            foreach (var item in list)
            {
                if (item.method.IsStatic || (!item.method.IsStatic && null != item.instance))
                {
                    item.method.Invoke(item.instance, parameters);
                }
            }

            return true;
        }
    }
}