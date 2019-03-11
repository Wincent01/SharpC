using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SharpC.Instructions
{
    [Cil("br")]
    public class Br : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body, int indite)
        {
            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);

            Console.WriteLine($"HEX: {hexIndex}");
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";

            foreach (var label in Visualizer.RegisteredLabels)
            {
                if (label != lbs) continue;

                if (Visualizer.FirstPass)
                    Visualizer.Index = instructions.Count + 1;
                return $"{Visualizer.Tabs}goto {lbs};\n";
            }

            Visualizer.RegisteredLabels.Add(lbs);

            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            if (Visualizer.FirstPass)
                Visualizer.Index = instructions.Count + 1;
            return $"{Visualizer.Tabs}goto {lbs};\n";
        }
    }

    [Cil("brtrue")]
    public class Brtrue : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var label = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            Console.WriteLine($"Contains {Visualizer.Lables.Count} labels");
            if (Visualizer.RegisteredLabels.Contains(label))
                return $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) == 1) goto {label};\n";

            Visualizer.RegisteredLabels.Add(label);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add(label, new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) == 1) goto {label};\n";
        }
    }

    [Cil("brfalse")]
    public class Brfalse : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            if (Visualizer.RegisteredLabels.Contains(lbs))
                return $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) == 0) goto {lbs};\n";

            Visualizer.RegisteredLabels.Add(lbs);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) == 0) goto {lbs};\n";
        }
    }

    [Cil("bne")]
    public class Bne : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            if (Visualizer.RegisteredLabels.Contains(lbs))
                return
                    $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) <= (unsigned int) ({var1.Value})) goto {lbs};\n";

            Visualizer.RegisteredLabels.Add(lbs);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return
                $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) != (unsigned int) ({var1.Value})) goto {lbs};\n";
        }
    }

    [Cil("bge")]
    public class Bge : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            if (Visualizer.RegisteredLabels.Contains(lbs))
                return
                    $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) <= (unsigned int) ({var1.Value})) goto {lbs};\n";

            Visualizer.RegisteredLabels.Add(lbs);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return
                $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) <= (unsigned int) ({var1.Value})) goto {lbs};\n";
        }
    }

    [Cil("ble")]
    public class Ble : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            if (Visualizer.RegisteredLabels.Contains(lbs))
                return
                    $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) <= (unsigned int) ({var1.Value})) goto {lbs};\n";

            Visualizer.RegisteredLabels.Add(lbs);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return
                $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) >= (unsigned int) ({var1.Value})) goto {lbs};\n";
        }
    }

    [Cil("blt")]
    public class Blt : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            if (Visualizer.RegisteredLabels.Contains(lbs))
                return
                    $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) <= (unsigned int) ({var1.Value})) goto {lbs};\n";

            Visualizer.RegisteredLabels.Add(lbs);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) > (unsigned int) ({var1.Value})) goto {lbs};\n";
        }
    }

    [Cil("beq")]
    public class Beq : CilInstruction
    {
        private ScopeInstruction _instruction;

        public override void Serialize(ScopeInstruction template)
        {
            _instruction = template;
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            var point = _instruction.Operand.Split(':')[0].Split('_')[1];
            var hexIndex = int.Parse(point, NumberStyles.HexNumber);
            var sum = _instruction.Operand.ToCharArray().Aggregate(0, (current, car) => current + car);
            var lbs = $"F{Visualizer.FuncCount}{hexIndex}_{sum}";
            if (Visualizer.RegisteredLabels.Contains(lbs))
                return
                    $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) == (unsigned int) ({var1.Value})) goto {lbs};\n";

            Visualizer.RegisteredLabels.Add(lbs);
            var labelInstructions = new List<ScopeInstruction>();
            Visualizer.Lables.Add($"{lbs}", new Visualizer.LabelStruct
            {
                Instructions = labelInstructions
            });
            foreach (var ins in instructions)
            {
                if (ins.Offset < hexIndex) continue;
                if (ins.Offset == hexIndex)
                {
                    var operand = _instruction.Operand.Split(':')[1].Split(' ');
                    labelInstructions.Add(new ScopeInstruction
                    {
                        Name = _instruction.Operand.Split(':')[1].Split(' ')[1],
                        Operand = operand.Length == 3 ? operand[2] : "",
                        Offset = ins.Offset,
                        Index = ins.Index
                    });
                }
                else
                {
                    labelInstructions.Add(ins);
                }
            }

            return
                $"{Visualizer.Tabs}if ((unsigned int) ({var0.Value}) == (unsigned int) ({var1.Value})) goto {lbs};\n";
        }
    }
}