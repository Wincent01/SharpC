using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Pop top stack stack value onto a variable.
    /// </summary>
    [Cil("stloc")]
    public class Stloc : CilInstruction
    {
        private int _point;

        public override void Serialize(ScopeInstruction template)
        {
            if (template.Operand.Contains("V"))
                _point = int.Parse(template.Operand.Split('_')[1]) - 1;
            else
                _point = int.Parse(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            Visualizer.Variables[_point] = new ScopeVariable
                {Value = $"var{_point}", Type = Visualizer.Variables[_point].Type};
            var item = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            return $"\tvar{_point} = ({Visualizer.Variables[_point].Type}) ({item.Value});\n";
        }
    }
}