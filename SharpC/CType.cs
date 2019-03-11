using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace SharpC
{
    public struct SpecialFunction
    {
        public string Value;
        public uint Parameters;
    }

    public static class CType
    {
        public static readonly Dictionary<Type, string> CBaseTypes = new Dictionary<Type, string>
        {
            {typeof(sbyte), "signed char"},
            {typeof(byte), "unsigned char"},
            {typeof(char), "unsigned char"},
            {typeof(int), "signed int"},
            {typeof(uint), "unsigned int"},
            {typeof(short), "signed short"},
            {typeof(ushort), "unsigned short"},
            {typeof(long), "signed long long"},
            {typeof(ulong), "unsigned long long"},
            {typeof(string), "char*"},
            {typeof(void), "void"},
            {typeof(object), "void*"},
            {typeof(bool), "signed int"},
            {typeof(Type), "struct Object*"},
            {typeof(float), "float"},
            {typeof(double), "double"}
        };

        public static string ResolveConv(string msil)
        {
            switch (msil)
            {
                case "i8":
                    return "signed long long";
                case "i4":
                    return "signed int";
                case "u1":
                    return "unsigned char";
                case "u2":
                    return "unsigned short";
                case "u8":
                    return "unsigned long long";
                case "r4":
                    return "float";
                case "i":
                    return "signed int";
                case "i1":
                    return "signed int";
                default:
                {
                    Console.WriteLine($"No Converted installed for {msil}");
                    return "void*";
                }
            }
        }

        public static string Deserialize(string obj)
        {
            var dict = new Dictionary<string, string>();
            foreach (var type in CBaseTypes) dict.Add(type.Key.Name, type.Value);

            foreach (var type in Visualizer.Types)
            {
                if (type.Name != obj.Trim('[', '*', '&', ']')) continue;
                if (obj.Contains("[")) return $"struct {type.Name.Split('[')[0]}**";

                if (obj.Contains("*")) return $"struct {type.Name.Split('*')[0]}*";

                if (obj.Contains("&")) return $"struct {type.Name.Split('&')[0]}";

                return $"struct {type.Name}*";
            }

            try
            {
                var extraPointers = "";
                if (obj.Contains("[") || obj.Contains("*") || obj.Contains("&")) extraPointers = "*";

                obj = obj.Trim('[', '*', '&', ']');
                return dict[obj] + extraPointers;
            }
            catch
            {
                return "void*";
            }
        }

        public static string Deserialize(VariableDefinition obj)
        {
            if (!obj.VariableType.IsGenericInstance && !obj.VariableType.IsGenericParameter)
                return Deserialize(obj.VariableType.Name);
            Console.WriteLine($"Generic type replaced by void* -> {obj.VariableType.Name}");
            //Generic types
            return
                $"void*{(obj.VariableType.IsArray || obj.VariableType.IsPointer || obj.VariableType.IsByReference ? "*" : "")}";
        }

        public static string Deserialize(Type obj)
        {
            if (obj.IsGenericType || obj.IsGenericParameter || obj.IsGenericTypeDefinition || obj.Name.Contains("<") ||
                obj.Name.Contains("`") || obj.IsConstructedGenericType)
            {
                Console.WriteLine($"Generic type replaced by void* -> {obj.Name}");
                //Generic types
                return $"void*{(obj.IsArray || obj.IsPointer || obj.IsByRef ? "*" : "")}";
            }

            foreach (var type in Visualizer.Types)
            {
                if (type != obj) continue;
                if (obj.IsArray) return $"{type.Name.Split('[')[0]}**";

                if (obj.IsPointer) return $"{type.Name.Split('*')[0]}*";

                if (obj.IsByRef) return $"{type.Name.Split('&')[0]}";
                if (type == obj)
                    return $"struct {type.Name}*";
            }

            try
            {
                return CBaseTypes[obj];
            }
            catch
            {
                try
                {
                    //return Deserialize(obj.Name, visualizer);
                }
                catch
                {
                    Console.WriteLine($"No C equivalent installed for {obj.FullName}");
                    return "void*";
                }
            }

            return "void*";
        }
    }
}