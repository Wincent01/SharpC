using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("pop")]
    public class Pop : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            stack.RemoveAt(stack.Count - 1);
            return "";
        }
    }
}