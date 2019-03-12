using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <summary>
    /// Duplicate stack value.
    /// </summary>
    [Cil("dup")]
    public class Dup : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(obj);
            stack.Add(obj);
            return "";
        }
    }
}