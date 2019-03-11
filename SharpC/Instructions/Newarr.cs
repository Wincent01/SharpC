using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("newarr")]
    public class Newarr : CilInstruction
    {
        private ScopeInstruction _template;
        public string Type;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Operand.Split('.');
            Type = parts[parts.Length - 1];
            _template = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var size = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var type = CType.Deserialize(Type);

            stack.Add(new ScopeVariable
            {
                Type = $"{type}*",
                Value = $"malloc(sizeof({type}) * {size.Value})"
            });

            return "";
        }
    }
}