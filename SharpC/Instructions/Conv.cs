using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Convert stack value onto stack.
    /// </summary>
    [Cil("conv")]
    public class Conv : CilInstruction
    {
        private string _convCode;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Name.Split('.');
            _convCode = parts[parts.Length - 1];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var newVar = new ScopeVariable
            {
                Value = stack[stack.Count - 1].Value,
                Type = CType.ResolveConv(_convCode)
            };
            stack.RemoveAt(stack.Count - 1);
            stack.Add(newVar);
            return "";
        }
    }
}