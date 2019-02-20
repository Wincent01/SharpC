using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("castclass")]
    public class Castclass : CilInstruction
    {
        public string CastClass;
        
        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Operand.Split('.');
            CastClass = parts[parts.Length - 1];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(new ScopeVariable
            {
                Value = $"({CastClass}) ({obj.Value})", 
                Type = $"{CastClass}"
            });
            
            return "";
        }
    }
}