using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SharpC.Instructions;

namespace SharpC
{
    public static class Visualizer
    {
        public static Type[] Types;
        public static List<MethodBase> Functions;

        public static string Tabs;

        public static readonly List<string> RegisteredLabels = new List<string>();

        public static void Init(Assembly assembly, params Type[] includes)
        {
            var types = new List<Type>();
            types.AddRange(includes ?? new Type[0]);
            types.AddRange(assembly.GetTypes());
            Types = types.ToArray();
            Functions = new List<MethodBase>();
            foreach (var type in typeof(CilInstruction).Assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<CilAttribute>();
                if (attr == null) continue;
                RegisteredInstructions.Add(attr.Name, type);
                Console.WriteLine($"Registered instruction type \"{attr.Name}\"");
            }

            Convert();
        }

        public static void Convert()
        {
            foreach (var type in Types)
                if (type.IsClass)
                    RegisterClass(type);
        }

        private static void RegisterClass(Type type)
        {
            Functions.AddRange(type.GetMethods());
            Functions.AddRange(type.GetConstructors());
        }

        public static string Additional(MethodBase info, Type type)
        {
            var add = 0;
            foreach (var function in Functions)
            {
                if (function.DeclaringType != type) continue;
                if (function.Name != info.Name) continue;
                if (function == info) break;
                add++;
            }

            return string.Concat(Enumerable.Repeat("_", add));
        }

        public static void Deserialize()
        {
            File.Create(FileConstruct.FilePath).Dispose();
            File.WriteAllText(FileConstruct.FilePath, string.Empty);

            foreach (var type in Types)
            {
                {
                    CClasses.Add(new CClass(type));

                    foreach (var method in type.GetMethods()) CFunctions.Add(new CFunction(method, type));

                    foreach (var constructor in type.GetConstructors())
                        CConstructors.Add(new CConstructor(constructor, type));
                }
                if (type.IsEnum) CEnums.Add(new CEnum(type));
            }

            foreach (var cEnum in CEnums) FileConstruct.Write(cEnum.DefEnum);

            foreach (var cClass in CClasses) FileConstruct.Write(cClass.Structure);

            foreach (var function in CFunctions) FileConstruct.Write(function.Declaration);

            foreach (var function in CConstructors) FileConstruct.Write(function.Declaration);

            foreach (var function in CFunctions) FileConstruct.Write(function.Definition);

            foreach (var function in CConstructors) FileConstruct.Write(function.Definition);
        }

        public static IEnumerable<string> BuildBody(MethodBase body)
        {
            foreach (var parameter in body.GetParameters()) Console.WriteLine($"PAR: {parameter}");

            Console.WriteLine($"Building function {body.Name}");
            var assembly = AssemblyDefinition.ReadAssembly(body.DeclaringType?.Assembly.Location);
            var toInspect = assembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods.Select(m => new {t, m}))
                .Where(x => x.m.HasBody).ToArray();

            if (!toInspect.Any())
            {
                yield return "return 0";
                yield break;
            }

            var decleared = false;
            var method = (MethodDefinition) null;
            foreach (var me in toInspect)
            {
                if (body.DeclaringType != null && (me.m.Name != body.Name ||
                                                   body.GetParameters().Length != me.m.Parameters.Count ||
                                                   me.t.FullName != body.DeclaringType.FullName)) continue;
                var hold = true;
                for (var index = 0; index < body.GetParameters().Length; index++)
                {
                    var parameter = body.GetParameters()[index];
                    if (parameter.ParameterType.FullName == me.m.Parameters[index].ParameterType.FullName) continue;
                    hold = false;
                    break;
                }

                if (hold)
                {
                    decleared = true;
                    method = me.m;
                }
            }

            if (!decleared) yield break;

            for (var index = 0; index < method.Body.Instructions.Count; index++)
            {
                var instruction = method.Body.Instructions[index];
                Console.WriteLine($"[{instruction.Offset}][{index}] {instruction.OpCode} \"{instruction.Operand}\"");
            }

            /*
             * Vars
             */
            Variables.Clear();
            foreach (var variable in method.Body.Variables)
            {
                Console.WriteLine($"Var type {variable.VariableType.Name}");
                yield return $"\t{CType.Deserialize(variable)} var{variable.Index} = 0;\n";
                Variables.Add(new ScopeVariable
                    {Type = CType.Deserialize(variable.VariableType.Name), Value = "0"});
            }

            Console.WriteLine("\n");

            var instr = new List<ScopeInstruction>();
            uint i = 0;
            foreach (var instruction in method.Body.Instructions)
            {
                var inst = new ScopeInstruction
                {
                    Name = instruction.OpCode.Name,
                    Offset = (uint) instruction.Offset,
                    Operand = instruction.Operand != null ? instruction.Operand.ToString() : "",
                    Index = i,
                    Template = instruction
                };
                instr.Add(inst);
                i++;
            }

            FuncCount++;
            foreach (var str in BuildScope(new List<ScopeVariable>(), instr, body, 1, 0)) yield return str;
        }

        private static IEnumerable<string> BuildScope(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body, int indite, int startIndex)
        {
            Lables = new Dictionary<string, LabelStruct>();
            Index = startIndex;

            while (true)
            {
                var yld = "";
                try
                {
                    if (Index > instructions.Count - 1) break;
                    Tabs = string.Concat(Enumerable.Repeat('\t', indite));
                    var instruction = instructions[Index];

                    //yld += $"/*\n" +
                    //       $" * {string.Join(" | ", stack)}\n" +
                    //       $" */\n" +
                    //       $"\n\t//{instruction.Name} -> {instruction.Operand}\n";

                    var instructionType = instruction.Name.Split('.')[0].ToLower();

                    Console.WriteLine($"[{instruction.Offset}][{Index}] {instruction.Name} -> {instruction.Operand}");

                    var instance = (CilInstruction) Activator.CreateInstance(RegisteredInstructions[instructionType]);
                    instance.Serialize(instruction);
                    yld += instance.Deserialize(stack, instructions, body, indite);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (Index > instructions.Count - 1) break;
                    var instruction = instructions[Index];
                    yld = $"\n/* Error {instruction.Name} -> {instruction.Operand} */\n";
                    //throw;
                }

                Index++;

                yield return yld;
            }

            FirstPass = true;

            foreach (var lbs in Lables)
            {
                yield return "\n\t" + lbs.Key + ":\n";
                foreach (var s in BuildScope(stack, lbs.Value.Instructions, body, indite, 0))
                    yield return s;
            }
        }

        public struct LabelStruct
        {
            public List<ScopeInstruction> Instructions;
            public List<ScopeVariable> Variables;
        }

        #region Instruction Variables

        public static readonly Dictionary<string, Type> RegisteredInstructions = new Dictionary<string, Type>();
        public static uint FuncCount;
        public static Dictionary<string, LabelStruct> Lables;
        public static List<ScopeVariable> Variables = new List<ScopeVariable>();
        public static int Index;
        public static bool FirstPass;

        #endregion

        #region Deserilize Variables

        public static readonly List<CFunction> CFunctions = new List<CFunction>();
        public static readonly List<CClass> CClasses = new List<CClass>();
        public static readonly List<CConstructor> CConstructors = new List<CConstructor>();
        public static readonly List<CEnum> CEnums = new List<CEnum>();

        #endregion
    }

    public struct ScopeInstruction
    {
        public string Name;
        public string Operand;
        public uint Offset;
        public uint Index;
        public Instruction Template;

        public override string ToString()
        {
            return $"[{Offset}] {Name} -> {Operand} | [{Index}]";
        }
    }

    public class ScopeVariable
    {
        public string Type;
        public string Value;

        public override string ToString()
        {
            return $"[{Type}] {Value}";
        }
    }

    public struct CFunction
    {
        public string Declaration;
        public string Definition;

        public CFunction(MethodInfo function, Type cls, IReadOnlyList<string> genericTypes = null)
        {
            Declaration = "";
            Definition = "";
            if (function.GetCustomAttribute<CMethodCoverAttribute>() != null ||
                function.DeclaringType.IsAbstract) return;

            if (function.GetCustomAttribute<CMethodCoverAttribute>() != null) return;

            var pars = function.GetParameters().Select(parameter =>
                $"{CType.Deserialize(parameter.ParameterType)} {parameter.Name}").ToList();

            if (function.IsGenericMethod && genericTypes != null)
            {
                var generics = function.GetGenericArguments();
                for (var i = 0; i < generics.Length; i++)
                {
                    var generic = generics[i];
                    for (var index = 0; index < function.GetParameters().Length; index++)
                    {
                        var parameter = function.GetParameters()[index];
                        if (parameter.ParameterType.Name == generic.Name)
                            pars[index] = $"{genericTypes[i]} {parameter.Name}";
                    }
                }
            }

            var parts = function.ReturnType.Name.Split('.');

            /*
             * DEC
             */

            Declaration = $"extern {CType.Deserialize(parts[parts.Length - 1])} " +
                          $"{cls.Name + function.Name}" +
                          $"{Visualizer.Additional(function, cls)} (";

            foreach (var par in pars)
            {
                Declaration += $"{par}";
                if (pars[pars.Count - 1] != par)
                    Declaration += ", ";
            }

            if (!function.IsStatic)
            {
                if (pars.Count > 0)
                    Declaration += ", ";
                Declaration += $"struct {function.DeclaringType?.Name}*me";
            }

            Declaration += ");\n\n";

            /*
             * DEF
             */

            if (function.DeclaringType != null)
            {
                Definition = $"{CType.Deserialize(parts[parts.Length - 1])} " +
                             $"{cls.Name + function.Name}" +
                             $"{Visualizer.Additional(function, cls)} (";

                foreach (var par in pars)
                {
                    Definition += $"{par}";
                    if (pars[pars.Count - 1] != par)
                        Definition += ", ";
                }

                if (!function.IsStatic)
                {
                    if (pars.Count > 0)
                        Definition += ", ";
                    Definition += $"struct {function.DeclaringType?.Name}*me";
                }

                Definition += ")\n{\n";
            }

            Visualizer.FirstPass = false;
            foreach (var line in Visualizer.BuildBody(function)) Definition += line;

            Definition += "}\n\n";
        }
    }

    public struct CClass
    {
        public readonly string Structure;

        public CClass(Type type, string[] genericTypes = null)
        {
            Structure = "";
            if (type.IsAbstract) return;

            if (type.Name.Contains("<") || type.Name.Contains("`"))
            {
                Console.WriteLine($"Unsupported and/or generic type {type.Name}");
                return;
            }

            Structure += $"typedef struct {type.Name}\n" + "{\n";
            var funcs = type.GetMethods().ToList();
            funcs.AddRange(type.GetMethods(BindingFlags.NonPublic));

            var methodList = (from method in type.GetMethods()
                where type.BaseType != null
                from info in type.BaseType?.GetMethods()
                where method.GetBaseDefinition() == info
                select method).ToList();

            foreach (var method in type.GetMethods())
            {
                if (method.GetCustomAttribute<CMethodCoverAttribute>() != null) return;
                if (type.BaseType != null)
                {
                    var none = true;
                    foreach (var info in type.BaseType.GetMethods())
                        if (method.GetBaseDefinition() == info)
                            none = false;
                    if (none)
                        methodList.Add(method);
                }
                else
                {
                    methodList.Add(method);
                }
            }

            foreach (var method in methodList)
            {
                if (method.IsStatic) continue;
                try
                {
                    var parts = method.ReturnType.Name.Split('.');
                    Structure +=
                        $"\t{CType.Deserialize(parts[parts.Length - 1])} (*{method.Name}{Visualizer.Additional(method, type)})(";
                    foreach (var parameter in method.GetParameters())
                        Structure += $"{CType.Deserialize(parameter.ParameterType)} {parameter.Name}";

                    if (!method.IsStatic)
                    {
                        if (method.GetParameters().Length > 0)
                            Structure += ", ";
                        Structure += $"struct {type.Name}*me";
                    }

                    Structure += ");\n\n";
                }
                catch
                {
                    Console.WriteLine("Failed");
                }
            }

            var fields = type.GetFields().ToList();
            fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                try
                {
                    var newName = field.Name.Where(t => t != '<' && t != '>')
                        .Aggregate("", (current, t) => current + t);
                    Structure += $"\t{CType.Deserialize(field.FieldType.Name)} {newName};\n";
                }
                catch
                {
                    Console.WriteLine("Failed");
                }
            }

            Structure += "}" + $" {type.Name};\n\n";
        }
    }

    public struct CEnum
    {
        public string DefEnum;

        public CEnum(Type type)
        {
            DefEnum = $"enum {type.Name}\n" + "{\n";

            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var i = 0;
            foreach (int value in values)
            {
                DefEnum += $"\t{names[i]} = {value}{(i == names.Length - 1 ? "" : ",")}\n";
                i++;
            }

            DefEnum += "};\n\n";
        }
    }

    public struct CConstructor
    {
        public string Declaration;
        public string Definition;

        public CConstructor(MethodBase function, Type cls)
        {
            Declaration = "";
            Definition = "";

            var pars = function.GetParameters().Select(parameter =>
                $"{CType.Deserialize(parameter.ParameterType)} {parameter.Name}").ToList();

            if (function.DeclaringType == null) return;
            Definition +=
                $"struct {function.DeclaringType.Name}* new{function.DeclaringType.Name}{Visualizer.Additional(function, cls)} (";
            foreach (var par in pars)
            {
                Definition += $"{par}";
                if (pars[pars.Count - 1] != par)
                    Definition += ", ";
            }

            Definition += ")\n{\n\t" + $"{function.DeclaringType.Name}* me = " +
                          $"malloc(sizeof({function.DeclaringType.Name}));";

            if (function.DeclaringType != null)
            {
                foreach (var method in function.DeclaringType.GetMethods())
                {
                    if (method.IsConstructor || method.IsStatic) continue;
                    Definition +=
                        $"\n\tme->{method.Name}{Visualizer.Additional(method, cls)} = &{function.DeclaringType.Name + method.Name}{Visualizer.Additional(method, cls)};";
                }

                Definition += "\n";
                Visualizer.FirstPass = false;
                foreach (var line in Visualizer.BuildBody(function)) Definition += line;
            }

            Definition += "\n\treturn me;";
            Definition += "\n}\n\n";

            Declaration +=
                $"struct {function.DeclaringType.Name}* new{function.DeclaringType.Name}{Visualizer.Additional(function, cls)} (";
            foreach (var par in pars)
            {
                Declaration += $"{par}";
                if (pars[pars.Count - 1] != par)
                    Declaration += ", ";
            }

            Declaration += ");\n\n";
        }
    }
}