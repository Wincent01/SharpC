using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Push type onto stack.
    /// </summary>
    [Cil("ldtoken")]
    public class Ldtoken : CilInstruction
    {
        public string Type;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Operand.Split('.');
            Type = parts[parts.Length - 1];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            stack.Add(new ScopeVariable {Value = Type, Type = Type});
            return "";
        }
    }
}