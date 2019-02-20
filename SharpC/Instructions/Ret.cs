using System.Collections.Generic;
using System.Reflection;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace SharpC.Instructions
{
    [Cil("ret")]
    public class Ret : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            try
            {
                if (((MethodInfo) body).ReturnType == typeof(void))
                {
                    return $"{Visualizer.Tabs}return;\n";
                }

                var parts = ((MethodInfo) body).ReturnType.Name.Split('.');

                var ret = stack[stack.Count - 1].Value;
                stack.Clear();
                return $"{Visualizer.Tabs}return ({CType.Deserialize(parts[parts.Length - 1])})" +
                       $" ({ret});\n";
            }
            catch
            {
                //Is constructor
                return "";
            }
        }
    }
}