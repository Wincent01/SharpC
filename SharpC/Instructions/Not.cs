using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("not")]
    public class Not : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(new ScopeVariable
            {
                Value = $"(~ {var0.Value})",
                Type = var0.Type
            });
            return "";
        }
    }
}