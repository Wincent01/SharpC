using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldelem")]
    public class Ldelem : CilInstruction
    {
        public bool Ref;
        public string Type;

        public override void Serialize(ScopeInstruction template)
        {
            if (template.Name.Split('.')[1] == "ref")
            {
                Ref = true;
                return;
            }

            Type = CType.ResolveConv(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var index = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var array = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if (Ref) Type = array.Type.Remove(array.Type.Length - 1);

            stack.Add(new ScopeVariable
            {
                Type = Type,
                Value = $"(({Type}) ({array.Value}[{index.Value}]))"
            });
            return "";
        }
    }
}