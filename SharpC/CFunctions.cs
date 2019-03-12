using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpC
{
    public struct CFunction
    {
        public readonly string Declaration;
        public readonly string Definition;

        /// <summary>
        /// Generate C function.
        /// </summary>
        public CFunction(MethodInfo function, Type cls, IReadOnlyList<string> genericTypes = null)
        {
            Declaration = "";
            Definition = "";
            if (function.GetCustomAttribute<CMethodCoverAttribute>() != null ||
                function.DeclaringType.IsAbstract) return;

            if (function.GetCustomAttribute<CMethodCoverAttribute>() != null) return;

            var pars = function.GetParameters().Select(parameter =>
                $"{CType.Deserialize(parameter.ParameterType)} {parameter.Name}").ToList();

            if (function.IsGenericMethod && genericTypes != null)
            {
                var generics = function.GetGenericArguments();
                for (var i = 0; i < generics.Length; i++)
                {
                    var generic = generics[i];
                    for (var index = 0; index < function.GetParameters().Length; index++)
                    {
                        var parameter = function.GetParameters()[index];
                        if (parameter.ParameterType.Name == generic.Name)
                            pars[index] = $"{genericTypes[i]} {parameter.Name}";
                    }
                }
            }

            var parts = function.ReturnType.Name.Split('.');

            /*
             * DEC
             */

            Declaration = $"extern {CType.Deserialize(parts[parts.Length - 1])} " +
                          $"{cls.Name + function.Name}" +
                          $"{Visualizer.Additional(function, cls)} (";

            foreach (var par in pars)
            {
                Declaration += $"{par}";
                if (pars[pars.Count - 1] != par)
                    Declaration += ", ";
            }

            if (!function.IsStatic)
            {
                if (pars.Count > 0)
                    Declaration += ", ";
                Declaration += $"struct {function.DeclaringType?.Name}*me";
            }

            Declaration += ");\n\n";

            /*
             * DEF
             */

            if (function.DeclaringType != null)
            {
                Definition = $"{CType.Deserialize(parts[parts.Length - 1])} " +
                             $"{cls.Name + function.Name}" +
                             $"{Visualizer.Additional(function, cls)} (";

                foreach (var par in pars)
                {
                    Definition += $"{par}";
                    if (pars[pars.Count - 1] != par)
                        Definition += ", ";
                }

                if (!function.IsStatic)
                {
                    if (pars.Count > 0)
                        Definition += ", ";
                    Definition += $"struct {function.DeclaringType?.Name}*me";
                }

                Definition += ")\n{\n";
            }

            Visualizer.FirstPass = false;
            foreach (var line in Visualizer.BuildBody(function)) Definition += line;

            Definition += "}\n\n";
        }
    }
}