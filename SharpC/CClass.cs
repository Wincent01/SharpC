using System;
using System.Linq;
using System.Reflection;

namespace SharpC
{
    public struct CClass
    {
        public readonly string Structure;

        /// <summary>
        /// Generate C class/struct.
        /// </summary>
        /// <param name="type">Class type</param>
        public CClass(Type type)
        {
            Structure = "";
            if (type.IsAbstract) return;

            if (type.Name.Contains("<") || type.Name.Contains("`"))
            {
                Console.WriteLine($"Unsupported and/or generic type {type.Name}");
                return;
            }

            Structure += $"typedef struct {type.Name}\n" + "{\n";
            var methods = type.GetMethods().ToList();
            methods.AddRange(type.GetMethods(BindingFlags.NonPublic));

            var methodList = (from method in type.GetMethods()
                where type.BaseType != null
                from info in type.BaseType?.GetMethods()
                where method.GetBaseDefinition() == info
                select method).ToList();

            /*
             * Collect methods
             */
            foreach (var method in type.GetMethods())
            {
                if (method.GetCustomAttribute<CMethodCoverAttribute>() != null) return;
                if (type.BaseType != null)
                {
                    var none = true;
                    foreach (var info in type.BaseType.GetMethods())
                        if (method.GetBaseDefinition() == info)
                            none = false;
                    if (none)
                        methodList.Add(method);
                }
                else
                {
                    methodList.Add(method);
                }
            }

            foreach (var method in methodList)
            {
                if (method.IsStatic) continue;
                try
                {
                    var parts = method.ReturnType.Name.Split('.');
                    Structure +=
                        $"\t{CType.Deserialize(parts[parts.Length - 1])} (*{method.Name}{Visualizer.Additional(method, type)})(";
                    foreach (var parameter in method.GetParameters())
                        Structure += $"{CType.Deserialize(parameter.ParameterType)} {parameter.Name}";

                    if (!method.IsStatic)
                    {
                        if (method.GetParameters().Length > 0)
                            Structure += ", ";
                        Structure += $"struct {type.Name}*me";
                    }

                    Structure += ");\n\n";
                }
                catch
                {
                    Console.WriteLine("Failed");
                }
            }

            var fields = type.GetFields().ToList();
            fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                try
                {
                    var newName = field.Name.Where(t => t != '<' && t != '>')
                        .Aggregate("", (current, t) => current + t);
                    Structure += $"\t{CType.Deserialize(field.FieldType.Name)} {newName};\n";
                }
                catch
                {
                    Console.WriteLine("Failed");
                }
            }

            Structure += "}" + $" {type.Name};\n\n";
        }
    }
}