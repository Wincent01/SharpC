using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("throw")]
    public class CilThrow : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            return $"{Visualizer.Tabs}exit(1);\n";
        }
    }
}