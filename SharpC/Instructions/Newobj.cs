using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("newobj")]
    public class Newobj : CilInstruction
    {
        public string ObjName;

        public override void Serialize(ScopeInstruction template)
        {
            var parts = template.Operand.Split(':');
            ObjName = parts[0].Split('.')[parts[0].Split('.').Length - 1];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var found = false;
            foreach (var type in Visualizer.Types)
            {
                if (type.Name != ObjName) continue;
                found = true;
                stack.Add(new ScopeVariable
                    {Type = type.Name, Value = $"new{type.Name}()"});
                break;
            }

            if (!found)
                stack.Add(new ScopeVariable
                    {Type = "void*", Value = "0"});
            return "";
        }
    }
}