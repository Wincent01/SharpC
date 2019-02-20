using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldc")]
    public class Ldc : CilInstruction
    {
        private ScopeInstruction _instruction;
        
        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var parts = _instruction.Name.Split('.');
            stack.Add(new ScopeVariable
            {
                Value = parts.Length == 3
                    ? parts[2] != "s" ? parts[2] : _instruction.Operand
                    : _instruction.Operand,
                Type = parts.Length == 3 ? CType.ResolveConv(parts[1]) : "unsigned int"
            });
            return "";
        }
    }
}