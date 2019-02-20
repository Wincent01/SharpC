using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("stelem")]
    public class Stelem : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var element = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var index = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var array = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            return $"{Visualizer.Tabs}{array.Value}[{index.Value}] = {element.Value};\n";
        }
    }
}