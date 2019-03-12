using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Push value of argument onto stack.
    /// </summary>
    [Cil("ldarg")]
    public class Ldarg : CilInstruction
    {
        private int _at;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Name.Split('.');
            if (parts.Length == 2)
            {
                _at = int.Parse(parts[1]);
                return;
            }

            _at = parts[1] == "s" ? int.Parse(template.Operand) : int.Parse(parts[1]) - 1;
            if (_at < 0)
                _at = 0;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            stack.Add(new ScopeVariable
            {
                Value = body.IsStatic ? body.GetParameters()[_at].Name :
                    _at == 0 ? "me" : body.GetParameters()[_at - 1].Name,
                Type = body.IsStatic ? CType.Deserialize(body.GetParameters()[_at].ParameterType) :
                    _at == 0 ? "void*" : CType.Deserialize(body.GetParameters()[_at - 1].ParameterType)
            });
            return "";
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Push value address of argument onto stack.
    /// </summary>
    [Cil("ldarga")]
    public class Ldarga : CilInstruction
    {
        private string _arg;

        public override void Serialize(ScopeInstruction template)
        {
            _arg = template.Operand;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            stack.Add(new ScopeVariable
            {
                Value = $"&{_arg}",
                Type = $"{CType.Deserialize(body.GetParameters().First(t => t.Name == _arg).ParameterType)}"
            });
            return "";
        }
    }
}