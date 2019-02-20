using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("conv")]
    public class Conv : CilInstruction
    {
        public string ConvCode;
        
        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Name.Split('.');
            ConvCode = parts[parts.Length - 1];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var newVar = new ScopeVariable
            {
                Value = stack[stack.Count - 1].Value,
                Type = CType.ResolveConv(ConvCode)
            };
            stack.RemoveAt(stack.Count - 1);
            stack.Add(newVar);
            return "";
        }
    }
}