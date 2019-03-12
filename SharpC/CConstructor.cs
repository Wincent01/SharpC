using System;
using System.Linq;
using System.Reflection;

namespace SharpC
{
    public struct CConstructor
    {
        public readonly string Declaration;
        public readonly string Definition;

        /// <summary>
        /// Generate C constructor.
        /// </summary>
        /// <param name="function">Method</param>
        /// <param name="cls">Class</param>
        public CConstructor(MethodBase function, Type cls)
        {
            Declaration = "";
            Definition = "";

            var pars = function.GetParameters().Select(parameter =>
                $"{CType.Deserialize(parameter.ParameterType)} {parameter.Name}").ToList();

            if (function.DeclaringType == null) return;
            Definition +=
                $"struct {function.DeclaringType.Name}* new{function.DeclaringType.Name}{Visualizer.Additional(function, cls)} (";
            foreach (var par in pars)
            {
                Definition += $"{par}";
                if (pars[pars.Count - 1] != par)
                    Definition += ", ";
            }

            Definition += ")\n{\n\t" + $"{function.DeclaringType.Name}* me = " +
                          $"malloc(sizeof({function.DeclaringType.Name}));";

            if (function.DeclaringType != null)
            {
                foreach (var method in function.DeclaringType.GetMethods())
                {
                    if (method.IsConstructor || method.IsStatic) continue;
                    Definition +=
                        $"\n\tme->{method.Name}{Visualizer.Additional(method, cls)} = &{function.DeclaringType.Name + method.Name}{Visualizer.Additional(method, cls)};";
                }

                Definition += "\n";
                Visualizer.FirstPass = false;
                foreach (var line in Visualizer.BuildBody(function)) Definition += line;
            }

            Definition += "\n\treturn me;";
            Definition += "\n}\n\n";

            Declaration +=
                $"struct {function.DeclaringType.Name}* new{function.DeclaringType.Name}{Visualizer.Additional(function, cls)} (";
            foreach (var par in pars)
            {
                Declaration += $"{par}";
                if (pars[pars.Count - 1] != par)
                    Declaration += ", ";
            }

            Declaration += ");\n\n";
        }
    }
}