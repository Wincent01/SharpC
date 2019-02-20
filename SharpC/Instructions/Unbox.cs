using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("unbox")]
    public class Unbox : CilInstruction
    {
        /*
         * TODO: Somehow fix this
         */
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            stack.Add(new ScopeVariable
            {
                Type = "void*",
                Value = "0"
            });
            return "";
        }
    }
}