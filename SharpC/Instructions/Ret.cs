using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Return
    /// </summary>
    [Cil("ret")]
    public class Ret : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            try
            {
                if (((MethodInfo) body).ReturnType == typeof(void)) return $"\treturn;\n";

                var parts = ((MethodInfo) body).ReturnType.Name.Split('.');

                var ret = stack[stack.Count - 1].Value;
                stack.Clear();
                return $"\treturn ({CType.Deserialize(parts[parts.Length - 1])})" +
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