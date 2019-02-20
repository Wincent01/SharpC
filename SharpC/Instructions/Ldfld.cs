using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("ldfld")]
    public class Ldfld : CilInstruction
    {
        public string Field;
        
        public override void Serialize(ScopeInstruction template)
        {
            Field = template.Operand.Split(':')[2];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var type = "void*";
            if (body.DeclaringType != null)
                foreach (var info in body.DeclaringType.GetFields())
                {
                    if (info.Name != Field) continue;
                    type = CType.Deserialize(info.FieldType);
                    break;
                }

            Console.WriteLine($"Field {Field} -> {type}");
            stack.Add(new ScopeVariable{Value = $"{obj.Value}->{Field}", Type = type});
            return "";
        }
    }
    
    [Cil("ldsfld")]
    public class Ldsfld : CilInstruction
    {
        public string Field;
        
        public override void Serialize(ScopeInstruction template)
        {
            Field = template.Operand.Split(':')[2];
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var type = "void*";
            if (body.DeclaringType != null)
                foreach (var info in body.DeclaringType.GetFields())
                {
                    if (info.Name != Field) continue;
                    type = CType.Deserialize(info.FieldType);
                    break;
                }

            Console.WriteLine($"Field {Field} -> {type}");
            stack.Add(new ScopeVariable{Value = $"{Field}", Type = type});
            return "";
        }
    }
}