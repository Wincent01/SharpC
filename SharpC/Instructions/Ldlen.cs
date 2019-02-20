using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldlen")]
    public class Ldlen : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            
            stack.Add(new ScopeVariable
            {
                Type = "unsigned int",
                Value = $"sizeof({obj.Value})"
            });
            return "";
        }
    }
}