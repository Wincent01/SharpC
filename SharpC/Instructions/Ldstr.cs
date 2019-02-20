using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldstr")]
    public class Ldstr : CilInstruction
    {
        public string Value;
        public override void Serialize(ScopeInstruction template)
        {
            Value = template.Operand.Trim('"');
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            stack.Add(new ScopeVariable
            {
                Type = "char*",
                Value = $"\"{Value}\""
            });
            return "";
        }
    }
}