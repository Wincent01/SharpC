using Mono.Cecil.Cil;

namespace SharpC
{
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
}