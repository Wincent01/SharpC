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
        #region Instruction Variables

        private static readonly Dictionary<string, Type> RegisteredInstructions = new Dictionary<string, Type>();
        public static uint FuncCount;
        public static Dictionary<string, LabelStruct> Labels;
        public static readonly List<ScopeVariable> Variables = new List<ScopeVariable>();
        public static readonly List<string> RegisteredLabels = new List<string>();
        public static int Index;
        public static bool FirstPass;

        #endregion

        #region Deserilize Variables

        private static readonly List<CFunction> CFunctions = new List<CFunction>();
        private static readonly List<CClass> CClasses = new List<CClass>();
        private static readonly List<CConstructor> CConstructors = new List<CConstructor>();
        private static readonly List<CEnum> CEnums = new List<CEnum>();

        #endregion

        #region Registed types and methods.

        public static Type[] Types;
        public static List<MethodBase> Methods;

        #endregion


        public static void Init(Assembly assembly, params Type[] includes)
        {
            var types = new List<Type>();
            types.AddRange(includes ?? new Type[0]);
            types.AddRange(assembly.GetTypes());
            Types = types.ToArray();
            Methods = new List<MethodBase>();
            foreach (var type in typeof(CilInstruction).Assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<CilAttribute>();
                if (attr == null) continue;
                RegisteredInstructions.Add(attr.Name, type);
                Console.WriteLine($"Registered instruction type \"{attr.Name}\"");
            }

            Convert();
        }

        private static void Convert()
        {
            foreach (var type in Types)
                if (type.IsClass)
                    RegisterClass(type);
        }

        private static void RegisterClass(Type type)
        {
            Methods.AddRange(type.GetMethods());
            Methods.AddRange(type.GetConstructors());
        }

        public static string Additional(MethodBase info, Type type)
        {
            var add = 0;
            foreach (var function in Methods)
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
            /*
             * Create generated file.
             */
            File.Create(FileConstruct.FilePath).Dispose();
            File.WriteAllText(FileConstruct.FilePath, string.Empty);

            /*
             * Collect types.
             */
            foreach (var type in Types)
            {
                CClasses.Add(new CClass(type));

                foreach (var method in type.GetMethods()) CFunctions.Add(new CFunction(method, type));

                foreach (var constructor in type.GetConstructors())
                    CConstructors.Add(new CConstructor(constructor, type));
                
                if (type.IsEnum) CEnums.Add(new CEnum(type));
            }

            /*
             * Write the generated C code.
             */
            foreach (var cEnum in CEnums) FileConstruct.Write(cEnum.DefEnum);

            foreach (var cClass in CClasses) FileConstruct.Write(cClass.Structure);

            foreach (var function in CFunctions) FileConstruct.Write(function.Declaration);

            foreach (var function in CConstructors) FileConstruct.Write(function.Declaration);

            foreach (var function in CFunctions) FileConstruct.Write(function.Definition);

            foreach (var function in CConstructors) FileConstruct.Write(function.Definition);
        }

        /// <summary>
        /// Build C code body.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static IEnumerable<string> BuildBody(MethodBase body)
        {
            /*
             * Get methods from Mono.cil
             */
            var assembly = AssemblyDefinition.ReadAssembly(body.DeclaringType?.Assembly.Location);
            var toInspect = assembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods.Select(m => new {t, m}))
                .Where(x => x.m.HasBody).ToArray();

            if (!toInspect.Any())
            {
                //Failed to find any methods.
                yield return "return 0";
                yield break;
            }

            /*
             * Find the correct method.
             */
            var declared = false;
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

                if (!hold) continue;
                declared = true;
                method = me.m;
            }

            if (!declared) yield break;

            for (var index = 0; index < method.Body.Instructions.Count; index++)
            {
                var instruction = method.Body.Instructions[index];
                //For debugging
                Console.WriteLine($"[{instruction.Offset}][{index}] {instruction.OpCode} \"{instruction.Operand}\"");
            }

            /*
             * Collect the method's variables.
             */
            Variables.Clear();
            foreach (var variable in method.Body.Variables)
            {
                yield return $"\t{CType.Deserialize(variable)} var{variable.Index} = 0;\n";
                Variables.Add(new ScopeVariable
                    {Type = CType.Deserialize(variable.VariableType.Name), Value = "0"});
            }

            /*
             * Build Scope Instructions from Instructors
             */
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

            //For label naming.
            FuncCount++;
            
            //Build scope.
            foreach (var str in BuildScope(new List<ScopeVariable>(), instr, body, 1, 0)) yield return str;
        }

        private static IEnumerable<string> BuildScope(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body, int indite, int startIndex)
        {
            Labels = new Dictionary<string, LabelStruct>();
            Index = startIndex;

            while (true)
            {
                var yld = "";
                try
                {
                    if (Index > instructions.Count - 1) break;
                    var instruction = instructions[Index];

//#define DEBUG_CODE
#if DEBUG_CODE
                    yld += $"/*\n" +
                           $" * {string.Join(" | ", stack)}\n" +
                           $" */\n" +
                           $"\n\t//{instruction.Name} -> {instruction.Operand}\n";
#endif
                    
                    var instructionType = instruction.Name.Split('.')[0].ToLower();

                    var instance = (CilInstruction) Activator.CreateInstance(RegisteredInstructions[instructionType]);
                    
                    /*
                     * Should combine these sometime.
                     */
                    instance.Serialize(instruction);
                    yld += instance.Deserialize(stack, instructions, body);
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

            //Allow code to be cut off when unreachable
            FirstPass = true;

            /*
             * Build scope for generated labels.
             */
            foreach (var lbs in Labels)
            {
                yield return $"\n\t{lbs.Key}:\n";
                foreach (var s in BuildScope(stack, lbs.Value.Instructions, body, indite, 0))
                    yield return s;
            }
        }

        public struct LabelStruct
        {
            /*
             * Might add more to this.
             */
            public List<ScopeInstruction> Instructions;
        }
    }
}