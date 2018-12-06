using System;
using System.Collections.Generic;

namespace SharpC
{
    public static class CType
    {
        private static readonly Dictionary<Type, string> CBaseTypes = new Dictionary<Type, string>
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
            {typeof(Type), "void*"}
        };

        public static string Resolve(string msil)
        {
            return Resolve(msil, out _);
        }

        public static string Resolve(string msil, out long value)
        {
            if (byte.TryParse(msil, out var val0))
            {
                value = val0;
                return "unsigned char";
            }

            if (sbyte.TryParse(msil, out var val1))
            {
                value = val1;
                return "signed char";
            }

            if (ushort.TryParse(msil, out var val2))
            {
                value = val2;
                return "unsigned short";
            }
            
            if (short.TryParse(msil, out var val3))
            {
                value = val3;
                return "signed short";
            }
            
            if (uint.TryParse(msil, out var val4))
            {
                value = val4;
                return "unsigned int";
            }
            
            if (int.TryParse(msil, out var val5))
            {
                value = val5;
                return "signed int";
            }
            
            if (ulong.TryParse(msil, out var val6))
            {
                value = (long) val6;
                return "unsigned long long";
            }
            
            if (long.TryParse(msil, out var val7))
            {
                value = val7;
                return "signed long long";
            }

            value = 0;
            return "void*";
        }

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
                default:
                {
                    Console.WriteLine($"No Converted installed for {msil}");
                    return "void*";
                }
            }
        }
        
        public static string Deserialize(Type obj, Visualizer visualizer)
        {
            foreach (var type in visualizer.Types)
            {
                if (type == obj)
                    return $"{type.Name}*";
            }

            try
            {
                return CBaseTypes[obj];
            }
            catch
            {
                Console.WriteLine($"No C equivalent installed for {obj.FullName}");
                return "";
            }
        }
    }
}