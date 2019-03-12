using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Push variable value onto the stack.
    /// </summary>
    [Cil("ldloc")]
    public class Ldloc : CilInstruction
    {
        public int Point;

        public override void Serialize(ScopeInstruction template)
        {
            if (template.Operand.Contains("V"))
                Point = int.Parse(template.Operand.Split('_')[1]) - 1;
            else
                Point = int.Parse(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body)
        {
            stack.Add(Visualizer.Variables[Point]);
            return "";
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Push variable reference onto the stack.
    /// </summary>
    [Cil("ldloca")]
    public class Ldloca : CilInstruction
    {
        public int Point;

        public override void Serialize(ScopeInstruction template)
        {
            if (template.Operand.Contains("V"))
            {
                Point = int.Parse(template.Operand.Split('_')[1]) - 1;
                if (Point < 0) Point = 0;
            }
            else
            {
                Point = int.Parse(template.Name.Split('.')[1]);
            }
        }

        public override string Deserialize(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body)
        {
            stack.Add(new ScopeVariable
            {
                Type = Visualizer.Variables[Point].Type,
                Value = $"&{Visualizer.Variables[Point].Value}"
            });
            return "";
        }
    }
}