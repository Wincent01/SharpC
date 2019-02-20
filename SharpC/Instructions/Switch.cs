using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil.Cil;

namespace SharpC.Instructions
{
    [Cil("switch")]
    public class Switch : CilInstruction
    {
        public Instruction[] Instructions;
        public ScopeInstruction Instruction;
        
        public override void Serialize(ScopeInstruction template)
        {
            Instructions = (Instruction[]) template.Template.Operand;
            Instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var point = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var final = $"\tswitch({point.Value})" + "\n\t{\n";
            try
            {
                for (var index = 0; index < Instructions.Length; index++)
                {
                    var inst = Instructions[index];
                    
                    var lbs = $"FS{Visualizer.FuncCount}{Instruction.Offset}_{index}";

                    Visualizer.RegisteredLabels.Add(lbs);
            
                    var labelInstructions = new List<ScopeInstruction>();
                    Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct{
                        Instructions = labelInstructions
                    });
                    
                    foreach (var ins in instructions)
                    {
                        if (ins.Offset < Instruction.Offset) continue;
                        if (ins.Offset == Instruction.Offset)
                        {
                            labelInstructions.Add(new ScopeInstruction
                            {
                                Template = inst,
                                Name = inst.OpCode.Name,
                                Offset = (uint) inst.Offset,
                                Index = (uint) Visualizer.Index,
                                Operand = inst.Operand == null ? "" : inst.Operand.ToString()
                            });
                        }
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