using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("dup")]
    public class Dup : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(obj);
            stack.Add(obj);
            return "";
        }
    }
}