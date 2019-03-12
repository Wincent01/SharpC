using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Convert stack value to class.
    /// </summary>
    [Cil("castclass")]
    public class Castclass : CilInstruction
    {
        private string _castClass;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Operand.Split('.');
            _castClass = parts[parts.Length - 1];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(new ScopeVariable
            {
                Value = $"({_castClass}) ({obj.Value})",
                Type = $"{_castClass}"
            });

            return "";
        }
    }
}