using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Store stack value in array at index.
    /// </summary>
    [Cil("stelem")]
    public class Stelem : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var element = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var index = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var array = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            return $"\t{array.Value}[{index.Value}] = {element.Value};\n";
        }
    }
}