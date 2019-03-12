using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Set field variable value.
    /// </summary>
    [Cil("stfld")]
    public class Stfld : CilInstruction
    {
        private string _field;

        public override void Serialize(ScopeInstruction template)
        {
            _field = template.Operand.Split(':')[2];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var value = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
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
            return $"\t{obj.Value}->{_field} = {value.Value};\n";
        }
    }
}