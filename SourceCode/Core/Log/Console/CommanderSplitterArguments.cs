using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Log
{
    public class ParametersSplitter
    {
        //分割符号
        private readonly HashSet<char> Splits = new HashSet<char>() {' ', ','};

        //定界符号
        private readonly string[] GroupDelimiters = new string[] {"\"\"", "''", "{}", "()", "[]"};


        private int FindStartGroupDelimiter(char value)
        {
            for (int i = 0; i < GroupDelimiters.Length; i++)
            {
                if (value == GroupDelimiters[i][0])
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindEndGroupDelimiter(int delimiterIndex, string text, int startIndex)
        {
            char startChar = GroupDelimiters[delimiterIndex][0];
            char endChar = GroupDelimiters[delimiterIndex][1];
            int depth = 1;
            for (int i = startIndex; i < text.Length; i++)
            {
                if (text[i] == endChar && --depth == 0)
                {
                    return i;
                }

                if (text[i] == startChar)
                {
                    depth++;
                }
            }

            return text.Length;
        }

        public int FindDelimiter(string text, int startIndex)
        {
            foreach (var item in Splits)
            {
                int index = text.IndexOf(item, startIndex);
                if (index >= 0)
                {
                    return index;
                }
            }

            return text.Length;
        }

        public bool IsDelimiter(char chars)
        {
            return Splits.Contains(chars);
        }

        public bool TrySplitParameters(string text, out List<string> args)
        {
            args = null;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            for (int i = 0, length = text.Length; i < length; i++)
            {
                if (Splits.Contains(text[i]))
                {
                    continue;
                }

                int delimiterIndex = FindStartGroupDelimiter(text[i]);
                if (delimiterIndex >= 0)
                {
                    int endIndex = FindEndGroupDelimiter(delimiterIndex, text, i + 1);
                    if (null == args)
                    {
                        args = new List<string>();
                    }

                    args.Add(text.Substring(i + 1, endIndex - i - 1));
                    i = (endIndex < text.Length - 1 && Splits.Contains(text[endIndex + 1]))
                        ? endIndex + 1
                        : endIndex;
                }
                else
                {
                    int endIndex = FindDelimiter(text, i + 1);
                    if (null == args)
                    {
                        args = new List<string>();
                    }

                    args.Add(text.Substring(i,
                        Splits.Contains(text[endIndex - 1]) ? endIndex - i - 1 : endIndex - i));
                    i = endIndex;
                }
            }

            return null != args;
        }
    }

    public class ArgumentsParser
    {
        public delegate bool ParseArgumentFunc(string input, out object output);

        public delegate bool ContainsFallback(Type type);

        public delegate bool ParseFallback(string input, Type type, out object output);


        private Dictionary<Type, ParseArgumentFunc> parseFuncMap;
        private Dictionary<Type, string> typeNameMap;
        private HashSet<ContainsFallback> containsFallbacks;
        private HashSet<ParseFallback> parseFallbacks;

        public ArgumentsParser()
        {
            parseFuncMap = new Dictionary<Type, ParseArgumentFunc>()
            {
                {typeof(int), ParseInt},
                {typeof(uint), ParseUInt},
                {typeof(long), ParseLong},
                {typeof(ulong), ParseULong},
                {typeof(short), ParseShort},
                {typeof(ushort), ParseUShort},
                {typeof(byte), ParseByte},
                {typeof(sbyte), ParseSByte},
                {typeof(float), ParseFloat},
                {typeof(double), ParseDouble},
                {typeof(decimal), ParseDecimal},
                {typeof(char), ParseChar},
                {typeof(string), ParseString},
                {typeof(bool), ParseBool}
            };
            typeNameMap = new Dictionary<Type, string>();
            foreach (var item in parseFuncMap)
            {
                RegisterTypeName(item.Key);
            }

            containsFallbacks = new HashSet<ContainsFallback>();
            parseFallbacks = new HashSet<ParseFallback>();
        }

        public bool ContainsParse(Type type)
        {
            if (parseFuncMap.ContainsKey(type))
            {
                return true;
            }

            foreach (var item in containsFallbacks)
            {
                if (item.Invoke(type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryParse(string input, Type type, out object output)
        {
            if (parseFuncMap.TryGetValue(type, out var func))
            {
                return func.Invoke(input, out output);
            }

            foreach (var item in parseFallbacks)
            {
                if (item.Invoke(input, type, out output))
                {
                    return true;
                }
            }

            output = null;
            return false;
        }

        public void RegisterParse(Type type, ParseArgumentFunc func)
        {
            if (!parseFuncMap.ContainsKey(type))
            {
                parseFuncMap.Add(type, func);
                RegisterTypeName(type);
            }
        }

        public void RegisterTypeName(Type type, string name)
        {
            if (!typeNameMap.ContainsKey(type))
            {
                typeNameMap.Add(type, name);
            }
        }

        public void RegisterTypeName(Type type)
        {
            string name = type.Name;
            RegisterTypeName(type, name);
        }

        public string GetParseTypeName(Type type)
        {
            if (!typeNameMap.TryGetValue(type, out var name))
            {
                name = type.Name;
            }

            return name;
        }

        public Type GetParseType(string name)
        {
            string baseTypeName = name;
            int arrayIndex = name.LastIndexOf("[]");
            if (arrayIndex >= 0)
            {
                baseTypeName = name.Substring(0, arrayIndex);
            }

            Type type = GetBaseType(baseTypeName);
            if (arrayIndex >= 0)
            {
                type = GetArrayType(type);
            }

            if (null == type)
            {
                Logger.WarnFormat("GetParseType is null,name:{0}", name);
            }

            return type;
        }
        
        private Type GetBaseType(string baseTypeName)
        {
            foreach (var item in typeNameMap)
            {
                if (item.Value == baseTypeName)
                {
                    return item.Key;
                }
            }

            return null;
        }

        private Type GetArrayType(Type type)
        {
            if (type == typeof(int))
            {
                return typeof(int[]);
            }
            else if (type == typeof(float))
            {
                return typeof(float[]);
            }
            else if (type == typeof(long))
            {
                return typeof(long[]);
            }
            else if (type == typeof(bool))
            {
                return typeof(bool[]);
            }
            else if (type == typeof(string))
            {
                return typeof(string[]);
            }

            return null;
        }
        
        public void AddContainsFallback(ContainsFallback fallback)
        {
            containsFallbacks.Add(fallback);
        }

        public void AddParseFallback(ParseFallback fallback)
        {
            parseFallbacks.Add(fallback);
        }

        private bool ParseInt(string input, out object output)
        {
            bool res = int.TryParse(input, out int value);
            output = value;
            return res;
        }

        private bool ParseUInt(string input, out object output)
        {
            bool res = uint.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseLong(string input, out object output)
        {
            bool res = long.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseULong(string input, out object output)
        {
            bool res = ulong.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseShort(string input, out object output)
        {
            bool res = short.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseUShort(string input, out object output)
        {
            bool res = ushort.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseByte(string input, out object output)
        {
            bool res = byte.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseSByte(string input, out object output)
        {
            bool res = sbyte.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseFloat(string input, out object output)
        {
            bool res = float.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseDouble(string input, out object output)
        {
            bool res = double.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseDecimal(string input, out object output)
        {
            bool res = decimal.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseChar(string input, out object output)
        {
            bool res = char.TryParse(input, out var value);
            output = value;
            return res;
        }

        private bool ParseString(string input, out object output)
        {
            output = input;
            return true;
        }

        private bool ParseBool(string input, out object output)
        {
            if (input == "1" || input.ToLowerInvariant() == "true")
            {
                output = true;
                return true;
            }
            else if (input == "0" || input.ToLowerInvariant() == "false")
            {
                output = false;
                return true;
            }

            output = false;
            return false;
        }
    }
}