using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldnull")]
    public class Ldnull : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            stack.Add(new ScopeVariable
            {
                Value = "((void*) 0)",
                Type = "void*"
            });
            return "";
        }
    }
}