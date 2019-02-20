using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("starg")]
    public class Starg : CilInstruction
    {
        private ScopeInstruction _instruction;
        
        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var type = CType.Deserialize(body.GetParameters().Where(t => t.Name == _instruction.Operand).GetType());
            stack.Add(new ScopeVariable {Value = _instruction.Operand, Type = type});
            return "";
        }
    }
}