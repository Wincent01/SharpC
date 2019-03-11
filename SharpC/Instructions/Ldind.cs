using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldind")]
    public class Ldind : CilInstruction
    {
        public string Type;

        public override void Serialize(ScopeInstruction template)
        {
            Type = CType.ResolveConv(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var conv = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            stack.Add(new ScopeVariable
            {
                Type = Type,
                Value = $"(({Type}) {conv.Value})"
            });
            return "";
        }
    }
}