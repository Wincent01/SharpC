using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("stind")]
    public class Stind : CilInstruction
    {
        public string Type;

        public override void Serialize(ScopeInstruction template)
        {
            Type = CType.ResolveConv(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            stack.Add(new ScopeVariable {Value = Type, Type = Type});
            return "";
        }
    }
}