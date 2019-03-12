using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Goto a label based upon a value.
    /// Haven't found too much information about how this one works.
    /// </summary>
    [Cil("switch")]
    public class Switch : CilInstruction
    {
        private ScopeInstruction _instruction;
        private Instruction[] _instructions;

        public override void Serialize(ScopeInstruction template)
        {
            _instructions = (Instruction[]) template.Template.Operand;
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var point = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var final = $"\tswitch({point.Value})" + "\n\t{\n";
            try
            {
                for (var index = 0; index < _instructions.Length; index++)
                {
                    var inst = _instructions[index];

                    var lbs = $"FS{Visualizer.FuncCount}{_instruction.Offset}_{index}";

                    Visualizer.RegisteredLabels.Add(lbs);

                    var labelInstructions = new List<ScopeInstruction>();
                    Visualizer.Labels.Add($"{lbs}", new Visualizer.LabelStruct
                    {
                        Instructions = labelInstructions
                    });

                    foreach (var ins in instructions)
                    {
                        if (ins.Offset < _instruction.Offset) continue;
                        if (ins.Offset == _instruction.Offset)
                            labelInstructions.Add(new ScopeInstruction
                            {
                                Template = inst,
                                Name = inst.OpCode.Name,
                                Offset = (uint) inst.Offset,
                                Index = (uint) Visualizer.Index,
                                Operand = inst.Operand == null ? "" : inst.Operand.ToString()
                            });
                        else
                            labelInstructions.Add(ins);
                    }

                    final += $"\t\tcase {index}:\n" + "\t\t{\n" + $"\t\t\tgoto {lbs};" + "\n\t\t\tbreak;\n\t\t}\n";
                }
            }
            catch
            {
                //
            }

            return final + "\t}\n";
        }
    }
}