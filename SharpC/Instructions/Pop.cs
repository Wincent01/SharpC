using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <summary>
    /// Pop value off stack
    /// </summary>
    [Cil("pop")]
    public class Pop : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            stack.RemoveAt(stack.Count - 1);
            return "";
        }
    }
}