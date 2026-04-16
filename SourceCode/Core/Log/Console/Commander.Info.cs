using System;
using System.Reflection;

namespace Core.Log
{
    internal struct CommandInfo : IEquatable<CommandInfo>
    {
        public readonly string command;
        public readonly string description;
        public readonly Type[] parameterTypes;
        public readonly string[] parameterNames;

        public CommandInfo(string command, string description = null, Type[] parameterTypes = null,
            string[] parameterNames = null)
        {
            this.command = command;
            this.description = description;
            this.parameterTypes = parameterTypes;
            this.parameterNames = parameterNames;
        }

        public bool Equals(CommandInfo other)
        {
            return command.Equals(other.command, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is CommandInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (command != null ? command.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    internal struct FunctionInfo : IEquatable<FunctionInfo>
    {
        public readonly MethodInfo method;
        public readonly object instance;
        public readonly Type[] parameterTypes;

        public FunctionInfo(MethodInfo method, object instance)
        {
            this.method = method;
            this.instance = instance;

            ParameterInfo[] parameters = method.GetParameters();
            parameterTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }
        }

        public bool Equals(FunctionInfo other)
        {
            return Equals(method, other.method) && Equals(instance, other.instance);
        }

        public override bool Equals(object obj)
        {
            return obj is FunctionInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (method != null ? method.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (instance != null ? instance.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return method.ToString();
        }
    }
}