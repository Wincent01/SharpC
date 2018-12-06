using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SharpC
{
    public class Visualizer
    {
        public readonly Type[] Types;
        private Assembly _assembly;
        private readonly List<MethodInfo> _functions;
        private readonly List<ConstructorInfo> _constructors;
        
        public Visualizer(Assembly assembly)
        {
            _assembly = assembly;
            Types = assembly.GetTypes();
            _functions = new List<MethodInfo>();
            _constructors = new List<ConstructorInfo>();
            Convert();
        }

        public void Convert()
        {
            foreach (var type in Types)
            {
                if (type.IsClass)
                    RegisterClass(type);
            }
        }

        private void RegisterClass(Type type)
        {
            _functions.AddRange(type.GetMethods());
            _constructors.AddRange(type.GetConstructors());
        }

        public void Deserialize()
        {
            var filePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\visualized.c";
            Console.WriteLine(filePath);
            var t = File.Create(filePath);
            t.Close();
            File.WriteAllText(filePath, string.Empty);

            foreach (var type in Types)
            {
                foreach (var function in type.GetMethods())
                {
                    var filef = File.AppendText(filePath);
                    var pars = function.GetParameters().Select(parameter =>
                        $"{CType.Deserialize(parameter.ParameterType, this)} {parameter.Name}").ToList();

                    if (function.DeclaringType != null)
                    {
                        filef.Write(
                            $"{CType.Deserialize(function.ReturnType, this)} " +
                            $"{type.Name + function.Name} (");
                    
                        foreach (var par in pars)
                        {
                            filef.Write($"{par},");
                        }
                        filef.Write($"struct {type.Name}*me);\n\n");
                    }
                    filef.Close();
                }
            }
            
            foreach (var type in Types)
            {
                var file = File.AppendText(filePath);
                
                file.WriteLine($"/*\n * START OF {type.Name} CLASS\n */");
                file.Close();
                file = File.AppendText(filePath);
                file.WriteLine($"typedef struct {type.Name}\n" + "{");
                foreach (var method in type.GetMethods())
                {
                    try
                    {
                        file.Write(
                            $"\n\t{CType.Deserialize(method.ReturnType, this)} (*{method.Name})(");
                        foreach (var parameter in method.GetParameters())
                        {
                            file.Write($"{CType.Deserialize(parameter.ParameterType, this)} {parameter.Name},");
                        }
                        file.Write($"{type.Name}*me);");
                    }
                    catch
                    {
                        Console.WriteLine("Failed");
                    }
                }
                file.WriteLine("\n}" + $" {type.Name};\n");
                
                file.Close();

                foreach (var function in type.GetMethods())
                {
                    var body = function.GetMethodBody();
                    if (body == null) continue;

                    var filef = File.AppendText(filePath);
                    var pars = function.GetParameters().Select(parameter =>
                        $"{CType.Deserialize(parameter.ParameterType, this)} {parameter.Name}").ToList();

                    if (function.DeclaringType != null)
                    {
                        if (!function.IsConstructor)
                        {
                            filef.Write(
                                $"\n{CType.Deserialize(function.ReturnType, this)} " +
                                $"{type.Name + function.Name} (");
                        }
                        else
                        {
                            filef.Write(
                                $"\n{type.Name} new{type.Name} (");
                        }

                        foreach (var par in pars)
                        {
                            filef.Write($" {par},");
                        }
                        filef.Write($" struct {type.Name}*me)\n" + "{\n");
                    }
                
                    filef.Write(BuildBody(function));
                
                    filef.Write("\n}\n");
                
                    filef.Close();
                }
                
                file = File.AppendText(filePath);
                
                file.WriteLine($"\n/*\n * END OF {type.Name} CLASS \n */\n");
                
                file.Close();
            }

            foreach (var function in _constructors)
            {
                var file = File.AppendText(filePath);
                var pars = function.GetParameters().Select(parameter =>
                    $"{CType.Deserialize(parameter.ParameterType, this)} {parameter.Name}").ToList();
                
                Console.WriteLine("Constructor!");

                if (function.DeclaringType != null)
                {
                    file.Write(
                        $"\n{function.DeclaringType.Name} new{function.DeclaringType.Name} (");
                    
                    foreach (var par in pars)
                    {
                        file.Write($" {par},");
                    }
                    
                    file.Write(")\n{\n\t" + $"struct {function.DeclaringType.Name} me;");
                    
                    if (function.DeclaringType != null)
                        foreach (var method in function.DeclaringType.GetMethods())
                        {
                            if (method.IsConstructor) continue;
                            file.WriteLine($"\n\tme.{method.Name} = &{function.DeclaringType.Name + method.Name};");
                        }
                
                    file.Write("\treturn me;");
                    file.Write("\n}\n");
                }

                file.Close();
            }
        }

        public string BuildBody(MethodInfo body)
        {
            var assembly = AssemblyDefinition.ReadAssembly(_assembly.Location);
            var toInspect = assembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods.Select(m => new {t, m}))
                .Where(x => x.m.HasBody);

            var decleared = false;
            foreach (var me in toInspect)
            {
                if (me.m.Name == body.Name)
                    decleared = true;
            }

            if (!decleared)
            {
                return "";
            }
            
            var method =
                toInspect.First(x => x.m.Name == body.Name && x.m.DeclaringType.Name == body.DeclaringType.Name);
            
            Console.WriteLine("\n");
            Console.WriteLine($"\tType = {method.t.Name}\n\t\tMethod = {method.m.Name}");
            for (var index = 0; index < method.m.Body.Instructions.Count; index++)
            {
                var instruction = method.m.Body.Instructions[index];
                Console.WriteLine($"[{instruction.Offset}][{index}] {instruction.OpCode} \"{instruction.Operand}\"");
            }

            Console.WriteLine("\n");

            var finalBody = "";
            
            var vars = new List<ScopeVariable>();

            var instr = new List<ScopeInstruction>();
            uint i = 0;
            foreach (var instruction in method.m.Body.Instructions)
            {
                var inst = new ScopeInstruction
                {
                    Name = instruction.OpCode.Name,
                    Offset = (uint) instruction.Offset,
                    Operand = instruction.Operand != null ? instruction.Operand.ToString() : "",
                    Index = i
                };
                instr.Add(inst);
                i++;
            }

            return BuildScope(vars, new List<ScopeVariable>(), instr, body, 1, 0);
        }

        private struct ScopeInstruction
        {
            public string Name;
            public string Operand;
            public uint Offset;
            public uint Index;
        }

        private struct ScopeVariable
        {
            public string Value;
            public string Type;
        }
        
        private string BuildScope(IList<ScopeVariable> vars, IList<ScopeVariable> stack, IList<ScopeInstruction> instructions, MethodInfo body, int indite, int startIndex)
        {
            var finalBody = "";

            var index = startIndex;
            while (true)
            {
                if (index > instructions.Count - 1) break;
                var tabs = string.Concat(Enumerable.Repeat('\t', indite));
                var instruction = instructions[index];
                var instructionType = instruction.Name.Split('.')[0];

                Console.WriteLine($"[{instruction.Offset}][{index}] {instruction.Name}");
                
                switch (instructionType)
                {
                    case "nop":
                        break;
                    case "ldc":
                    {
                        var parts = instruction.Name.Split('.');
                        stack.Add(new ScopeVariable
                        {
                            Value = parts.Length == 3
                                ? parts[2] != "s" ? parts[2] : instruction.Operand
                                : instruction.Operand,
                            Type = parts.Length == 3 ? CType.ResolveConv(parts[1]) : "unsigned int"
                        });
                        break;
                    }
                    case "pop":
                    {
                        stack.RemoveAt(stack.Count-1);
                        break;
                    }
                    case "stloc":
                    {
                        int point;
                        if (instruction.Operand.Contains("V"))
                            point = int.Parse(instruction.Operand.Split('_')[1]) - 1;
                        else
                            point = int.Parse(instruction.Name.Split('.')[1]);
                        while (point > vars.Count - 1)
                        {
                            vars.Add(new ScopeVariable());
                        }
                        vars[point] = new ScopeVariable {Value = $"var{vars.Count}", Type = stack[stack.Count - 1].Type};

                        finalBody +=
                            $"{tabs}{stack[stack.Count - 1].Type} var{vars.Count} = {(vars[point].Value.Contains(" ") ? "(" : "")}" +
                            $"{stack[stack.Count - 1].Value}{(vars[point].Value.Contains(" ") ? ")" : "")};\n";
                        stack.RemoveAt(stack.Count - 1);
                        
                        break;
                    }
                    case "newobj":
                    {
                        var parts = instruction.Operand.Split(':');
                        var objName = parts[0].Split('.')[parts[0].Split('.').Length - 1];
                        Console.WriteLine("Objname: " + objName);

                        foreach (var part in parts)
                        {
                            Console.WriteLine($"Part {part}");
                        }

                        var found = false;
                        foreach (var type in Types)
                        {
                            Console.WriteLine($"Comp: {type.Name} == {objName} ? {type.Name == objName}");
                            if (type.Name == objName)
                            {
                                found = true;
                                stack.Add(new ScopeVariable
                                    {Type = type.Name, Value = $"new{type.Name}()"});
                                break;
                            }
                        }

                        if (!found)
                            stack.Add(new ScopeVariable
                                {Type = "void*", Value = "0"});
                        
                        break;
                    }
                    case "callvirt":
                    {
                        var parts = instruction.Operand.Split(':');
                        var objName = parts[2].Split('(')[0];
                        
                        Console.WriteLine("Function call: " + objName);

                        MethodInfo func = null;
                        foreach (var function in _functions)
                        {
                            if (function.Name == objName)
                            {
                                func = function;
                                break;
                            }
                        }

                        if (func == null)
                            break;
                        
                        var pars = new List<string>();
                        for (var i = 0; i < func.GetParameters().Length; i++)
                        {
                            pars.Add(stack[stack.Count - 1 - i].Value);
                        }

                        if (func.ReturnType != typeof(void))
                        {
                            var variable = new ScopeVariable
                            {
                                Type = $"{CType.Deserialize(func.ReturnType, this)}",
                                Value = $"{stack[0].Value}.{objName}({string.Concat(pars)})"
                            };
                            stack.Add(variable);
                        }
                        else
                        {
                            finalBody +=
                                $"{tabs}{stack[0].Value}.{objName}" +
                                $"({string.Concat(pars)});\n";
                        }

                        break;
                    }
                    case "ldloc":
                    {
                        int point;
                        if (instruction.Operand.Contains("V"))
                            point = int.Parse(instruction.Operand.Split('_')[1]) - 1;
                        else
                            point = int.Parse(instruction.Name.Split('.')[1]);
                        stack.Add(vars[point]);
                        break;
                    }
                    case "ldarg":
                    {
                        var parts = instruction.Name.Split('.');
                        if (parts.Length == 2)
                        {
                            var at = parts[1] == "s" ? int.Parse(instruction.Operand) : int.Parse(parts[1]) - 1;
                            stack.Add(new ScopeVariable
                            {
                                Value = body.GetParameters()[at].Name,
                                Type = CType.Deserialize(body.GetParameters()[at].ParameterType, this)
                            });
                        }
                        else
                        {
                            stack.Add(new ScopeVariable
                                {Value = instruction.Operand, Type = CType.Resolve(instruction.Operand)});
                        }
                        break;
                    }
                    case "conv":
                    {
                        var newVar = new ScopeVariable
                        {
                            Value = stack[stack.Count - 1].Value,
                            Type = CType.ResolveConv(instruction.Name.Split('.')[1])
                        };
                        stack.RemoveAt(stack.Count - 1);
                        stack.Add(newVar);
                        break;
                    }
                    case "add":
                    {
                        var var0 = stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);
                        var var1 = stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);

                        stack.Add(new ScopeVariable
                        {
                            Value = $"({var0.Value} + {var1.Value})",
                            Type = var0.Type == "void*"
                                ? var1.Type == "void*" ? "unsigned long long" : var1.Type
                                : var0.Type
                        });
                        break;
                    }
                    case "ceq":
                    {
                        var var0 = stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);
                        var var1 = stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);
                        
                        stack.Add(new ScopeVariable
                        {
                            Value = $"({var0.Value} == {var1.Value})",
                            Type = "signed int"
                        });
                        break;
                    }
                    case "ret":
                    {
                        if (body.ReturnType == typeof(void))
                        {
                            finalBody += $"{tabs}return;";
                        }
                        else
                        {
                            finalBody +=
                                $"{tabs}return ({CType.Deserialize(body.ReturnType, this)}) ({stack[stack.Count - 1].Value});";
                        }
                        break;
                    }
                    case "br":
                    {
                        var point = instruction.Operand.Split(':')[0].Split('_')[1];
                        var hexIndex = int.Parse(point, System.Globalization.NumberStyles.HexNumber);
                        
                        
                        foreach (var scopeInstruction in instructions)
                        {
                            if (scopeInstruction.Offset != hexIndex) continue;
                            index = (int) scopeInstruction.Index - 1;
                            var newInstr = new ScopeInstruction
                            {
                                Name = instruction.Operand.Split(':')[1].Split(' ')[1], Operand = "",
                                Offset = scopeInstruction.Offset,
                                Index = scopeInstruction.Index
                            };
                            instructions[index + 1] = newInstr;
                            break;
                        }
                        
                        break;
                    }
                    case "brfalse":
                    {
                        var var0 = stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);

                        var point = instruction.Operand.Split(':')[0].Split('_')[1];
                        var newStartIndex = int.Parse(point, System.Globalization.NumberStyles.HexNumber);

                        var newInstructions = new List<ScopeInstruction>();
                        
                        newInstructions.AddRange(instructions);

                        var ind = 0;
                        foreach (var scopeInstruction in instructions)
                        {
                            if (scopeInstruction.Offset != newStartIndex) continue;
                            var newInstr = new ScopeInstruction
                            {
                                Name = instruction.Operand.Split(':')[1].Split(' ')[1], Operand = "",
                                Offset = scopeInstruction.Offset,
                                Index = scopeInstruction.Index
                            };
                            newInstructions[(int) scopeInstruction.Index] = newInstr;
                            ind = (int) scopeInstruction.Index;
                            break;
                        }

                        finalBody += $"{tabs}if ({var0.Value} == 0)\n{tabs}" + 
                                     "{\n" +
                                     BuildScope(vars, stack, newInstructions, body, indite + 1, ind) + 
                                     $"\n{tabs}" + "}\n";
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"No instruction case for: {instructionType}");
                        break;
                    }
                }

                index++;
            }
            
            #region Old Code
            /*
            for (var index = startIndex; index < instructions.Count; index++)
            {
                var instruction = instructions[index];
                var name = instruction.Name;

                Console.WriteLine($"[{index}] {name} : {instruction.Operand}");

                if (name == "nop")
                {
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}\n";
                    continue;
                }

                if (name.StartsWith("ldloc"))
                {
                    var value = name.Split('.')[1];
                    Console.WriteLine("Looked into: " + value);
                    if (value != "s")
                    {
                        var pass = true;
                        foreach (var i in current)
                        {
                            if (i == int.Parse(value))
                                pass = false;
                        }
                        if (pass)
                            current.Add(int.Parse(value));
                    }
                    else
                    {
                        var pass = true;
                        foreach (var i in current)
                        {
                            if (i == int.Parse(instruction.Operand.Split('_')[1]))
                                pass = false;
                        }
                        if (pass)
                            current.Add(int.Parse(instruction.Operand.Split('_')[1]) - 1);
                    }
                }

                if (name.StartsWith("ldarg"))
                {
                    var par = body.GetParameters()[int.Parse(name.Split('.')[1]) - 1];
                    vars.Add(par.Name);
                    finalBody +=
                        $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Deserialize(par.ParameterType, this)} var{vars.Count - 1} = {par.Name};\n";
                    current.Add(vars.Count - 1);
                }

                if (name.StartsWith("stloc"))
                {
                    var i = int.Parse(
                        name.Split('.')[1] == "s" ? instruction.Operand.Split('_')[1] : name.Split('.')[1]);

                    vars[i] = vars[current.Count == 0 ? vars.Count - 1 : current[0]];
                    current.Clear();
                    current.Add(vars.Count - 1);
                }

                if (name.StartsWith("conv"))
                {
                    var parts = name.Split('.');

                    if (current.Count == 0)
                        continue;
                    var value = vars[current[0]];
                    vars.Add(value);
                    finalBody +=
                        $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.ResolveConv(parts[1])} var{vars.Count - 1} = " +
                        $"({CType.ResolveConv(parts[1])}) var{current[0]};\n";
                    current.Clear();
                }

                if (name.StartsWith("ldc"))
                {
                    var parts = name.Split('.');
                    var value = "0";
                    if (parts.Length == 3)
                        value = parts[2];
                    else
                    {
                        if (instruction.Operand != null)
                            value = instruction.Operand;
                    }

                    if (value == "s")
                    {
                        value = instruction.Operand;
                    }

                    vars.Add(value);
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Resolve(value)} var{vars.Count - 1} = {value};\n";
                    current.Add(vars.Count - 1);
                }

                if (name.StartsWith("newobj"))
                {
                    vars.Add("0");
                    current.Add(vars.Count - 1);
                }

                if (name == "add")
                {
                    var var0 = vars[current[0]];
                    var var1 = vars[current[1]];
                    CType.Resolve(var0, out var num0);
                    CType.Resolve(var1, out var num1);
                    var var2 = num0 + num1;
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Resolve(var2.ToString())} var{vars.Count} = " +
                                 $"var{current[0]} + var{current[1]};\n";
                    vars.Add(var2.ToString());
                    current.Clear();
                    current.Add(vars.Count - 1);
                }

                if (name == "sub")
                {
                    var var0 = vars[current[0]];
                    var var1 = vars[current[1]];
                    CType.Resolve(var0, out var num0);
                    CType.Resolve(var1, out var num1);
                    var var2 = num0 - num1;
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Resolve(var2.ToString())} var{vars.Count} = " +
                                 $"var{current[0]} - var{current[1]};\n";
                    vars.Add(var2.ToString());
                    current.Clear();
                    current.Add(vars.Count - 1);
                }

                if (name == "mul")
                {
                    var var0 = vars[current[0]];
                    var var1 = vars[current[1]];
                    CType.Resolve(var0, out var num0);
                    CType.Resolve(var1, out var num1);
                    var var2 = num0 * num1;
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Resolve(var2.ToString())} var{vars.Count} = " +
                                 $"var{current[0]} * var{current[1]};\n";
                    vars.Add(var2.ToString());
                    current.Clear();
                    current.Add(vars.Count - 1);
                }

                if (name == "div")
                {
                    var var0 = vars[current[0]];
                    var var1 = vars[current[1]];
                    CType.Resolve(var0, out var num0);
                    CType.Resolve(var1, out var num1);
                    var var2 = num0 / num1;
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Resolve(var2.ToString())} var{vars.Count} = " +
                                 $"var{current[0]} / var{current[1]};\n";
                    vars.Add(var2.ToString());
                    current.Clear();
                    current.Add(vars.Count - 1);
                }

                if (name == "ceq")
                {
                    var var0 = vars[current[0]];
                    var var1 = vars[current[1]];
                    CType.Resolve(var0, out var num0);
                    CType.Resolve(var1, out var num1);
                    var var2 = num0 == num1 ? 1 : 0;
                    Console.WriteLine($"{var0} == {var1} = {var2}");
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}{CType.Resolve(var2.ToString())} var{vars.Count} = " +
                                 $"var{current[0]} == var{current[1]};\n";
                    vars.Add(var2.ToString());
                    current.Clear();
                    current.Add(vars.Count - 1);
                }

                if (name.StartsWith("brfalse"))
                {
                    finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}if (var{current[0]} == 1)\n{string.Concat(Enumerable.Repeat("    ", indite))}" + "{\n";
                    Console.WriteLine($"\nbrfalse: {vars[current[0]]}\n");
                    
                    current.Clear();
                    
                    var brto = instruction.Operand;
                    if (brto != null)
                    {
                        var point = int.Parse(brto.Split(':')[0].Split('_')[1]);
                        var scope = new List<ScopeInstruction>();
                        var del = true;
                        var prevVars = vars.Count;
                        for (var i = 0; i < instructions.Count; i++)
                        {
                            if (i == point)
                            {
                                Console.WriteLine("Parse:" + brto.Split(':')[1].Split(' ')[1]);
                                scope.Add(new ScopeInstruction {Name = brto.Split(':')[1].Split(' ')[1]});
                                del = false;
                            }
                            else
                            {
                                scope.Add(instructions[i]);
                            }
                            
                            if (del)
                                index++;
                        }

                        finalBody += BuildScope(vars, current, scope, body, indite + 1, point);
                        current.Clear();
                        var prev = new List<string>();
                        for (var i = 0; i < prevVars; i++)
                        {
                            prev.Add(vars[i]);
                        }

                        vars = prev;
                    }

                    finalBody += $"\n{string.Concat(Enumerable.Repeat("    ", indite))}" + "}\n";
                }

                if (name == "ret")
                {
                    if (current.Count > 0)
                    {
                        Console.WriteLine($"Returning: {string.Join(" , ", current)}");
                        finalBody += $"{string.Concat(Enumerable.Repeat("    ", indite))}return ({CType.Deserialize(body.ReturnType, this)}) " +
                                     $"var{current[0]};";
                        current.Clear();
                    }
                }

                if (name.StartsWith("br."))
                {
                    var brto = instruction.Operand;
                    var point = int.Parse(brto.Split(':')[0].Split('_')[1], System.Globalization.NumberStyles.HexNumber);
                }
            }
            */
            #endregion

            return finalBody;
        }
    }
}