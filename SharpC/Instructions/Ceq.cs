using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Test if stack values are equals.
    /// </summary>
    [Cil("ceq")]
    public class Ceq : CilInstruction
    {
        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(new ScopeVariable
            {
                Value = $"({var0.Value} == {var1.Value})",
                Type = "signed int"
            });
            return "";
        }
    }
}