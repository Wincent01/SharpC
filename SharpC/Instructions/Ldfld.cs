using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Push field variable onto stack.
    /// </summary>
    [Cil("ldfld")]
    public class Ldfld : CilInstruction
    {
        private string _field;

        public override void Serialize(ScopeInstruction template)
        {
            _field = template.Operand.Split(':')[2];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var type = "void*";
            if (body.DeclaringType != null)
                foreach (var info in body.DeclaringType.GetFields())
                {
                    if (info.Name != _field) continue;
                    type = CType.Deserialize(info.FieldType);
                    break;
                }

            Console.WriteLine($"Field {_field} -> {type}");
            stack.Add(new ScopeVariable {Value = $"{obj.Value}->{_field}", Type = type});
            return "";
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Push field variable address onto stack.
    /// </summary>
    [Cil("ldsfld")]
    public class Ldsfld : CilInstruction
    {
        private string _field;

        public override void Serialize(ScopeInstruction template)
        {
            _field = template.Operand.Split(':')[2];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var type = "void*";
            if (body.DeclaringType != null)
                foreach (var info in body.DeclaringType.GetFields())
                {
                    if (info.Name != _field) continue;
                    type = CType.Deserialize(info.FieldType);
                    break;
                }

            Console.WriteLine($"Field {_field} -> {type}");
            stack.Add(new ScopeVariable {Value = $"{_field}", Type = type});
            return "";
        }
    }
}