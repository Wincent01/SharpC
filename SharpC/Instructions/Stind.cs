using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Store type value.
    /// </summary>
    [Cil("stind")]
    public class Stind : CilInstruction
    {
        public string Type;

        public override void Serialize(ScopeInstruction template)
        {
            Type = CType.ResolveConv(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            stack.Add(new ScopeVariable {Value = Type, Type = Type});
            return "";
        }
    }
}