using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Call a function.
    /// </summary>
    [Cil("callvirt")] [Obsolete] //TODO: Redo this one
    public class Callvirt : CilInstruction
    {
        private ScopeInstruction _template;

        private string _className;
        private string _objName;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Operand.Split(':');
            _objName = parts[2].Split('(')[0].Split('<')[0];

            var clsParts = parts[0].Split('.');
            _className = clsParts[clsParts.Length - 1];

            _template = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var method = (MethodReference) _template.Template.Operand;

            MethodInfo func = null;
            foreach (var function in Visualizer.Methods)
                if (function.Name == method.Name && method.Parameters.Count == function.GetParameters().Length)
                {
                    var hold = true;
                    for (var index = 0; index < function.GetParameters().Length; index++)
                    {
                        var parameter = function.GetParameters()[index];
                        if (parameter.ParameterType.FullName != method.Parameters[index].ParameterType.FullName)
                        {
                            hold = false;
                            break;
                        }
                    }

                    if (hold)
                    {
                        func = (MethodInfo) function;
                        break;
                    }
                }

            if (func == null) return $"\n\t// Unknown function call -> {_template}\n";

            if (_objName.StartsWith("op_")) Console.Write(0);

            var pars = new List<string>();
            for (var i = 0; i < func.GetParameters().Length; i++)
            {
                pars.Add(stack[stack.Count - 1].Value);
                stack.RemoveAt(stack.Count - 1);
            }

            if (!func.IsStatic)
            {
                var obj = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);

                var isStruct = obj.Value.Contains("struct");

                if (func.ReturnType != typeof(void))
                {
                    if (func.DeclaringType == null) return "";
                    var variable = new ScopeVariable
                    {
                        Type = $"{CType.Deserialize(func.ReturnType)}",
                        Value =
                            $"{(isStruct ? "" : $"((struct {func.DeclaringType.Name}*)")} {obj.Value}{(isStruct ? "" : ")")}->" +
                            $"{_objName}{Visualizer.Additional(func, func.DeclaringType)}({string.Concat(pars)}{(pars.Count > 0 ? "," : "")}{obj.Value})"
                    };
                    stack.Add(variable);
                }
                else
                {
                    if (func.DeclaringType != null)
                        return
                            $"\t{(isStruct ? "" : $"((struct {func.DeclaringType.Name}*)")} {obj.Value}{(isStruct ? "" : ")")}->" +
                            $"{_objName}{Visualizer.Additional(func, func.DeclaringType)}({string.Concat(pars)}{(pars.Count > 0 ? "," : "")}{obj.Value});\n";
                }

                return "";
            }

            if (func.ReturnType != typeof(void))
            {
                var variable = new ScopeVariable
                {
                    Type = $"{CType.Deserialize(func.ReturnType)}",
                    Value =
                        $"{_className}{_objName}{Visualizer.Additional(func, func.DeclaringType)}({string.Join(", ", pars)})"
                };
                stack.Add(variable);
            }
            else
            {
                return
                    $"\t{_className}{_objName}{Visualizer.Additional(func, func.DeclaringType)}({string.Join(", ", pars)});\n";
            }

            return "";
        }
    }
}