using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Test if object is of type
    /// </summary>
    [Cil("isinst")]
    public class Isinst : CilInstruction
    {
        /*
         * I have no idea how I could do this in C at the moment.
         */
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
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