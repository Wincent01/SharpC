using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldelema")]
    public class Ldelema : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var index = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var array = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(new ScopeVariable
            {
                Type = array.Type,
                Value = $"{array.Value}[{index.Value}]"
            });

            return "";
        }
    }
}