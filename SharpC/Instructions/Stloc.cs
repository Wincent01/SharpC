using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("stloc")]
    public class Stloc : CilInstruction
    {
        public int Point;

        public override void Serialize(ScopeInstruction template)
        {
            if (template.Operand.Contains("V"))
                Point = int.Parse(template.Operand.Split('_')[1]) - 1;
            else
                Point = int.Parse(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            Visualizer.Variables[Point] = new ScopeVariable
                {Value = $"var{Point}", Type = Visualizer.Variables[Point].Type};
            var item = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            Console.WriteLine($"{Visualizer.Variables[Point].Type}");
            return $"{Visualizer.Tabs}var{Point} = ({Visualizer.Variables[Point].Type}) ({item.Value});\n";
        }
    }
}