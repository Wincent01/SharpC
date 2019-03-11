using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    public class Operation : CilInstruction
    {
        protected virtual string Code => "";

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body, int indite)
        {
            var var0 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var var1 = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            stack.Add(new ScopeVariable
            {
                Value = $"({var1.Value} {Code} {var0.Value})",
                Type = var0.Type == "void*"
                    ? var1.Type == "void*" ? "unsigned long long" : var1.Type
                    : var0.Type
            });
            return "";
        }
    }

    [Cil("add")]
    public class Add : Operation
    {
        protected override string Code => "+";
    }

    [Cil("sub")]
    public class Sub : Operation
    {
        protected override string Code => "-";
    }

    [Cil("mul")]
    public class Mul : Operation
    {
        protected override string Code => "*";
    }

    [Cil("div")]
    public class Div : Operation
    {
        protected override string Code => "/";
    }

    [Cil("clt")]
    public class Clt : Operation
    {
        protected override string Code => "<";
    }

    [Cil("cgt")]
    public class Cgt : Operation
    {
        protected override string Code => ">";
    }

    [Cil("shl")]
    public class Shl : Operation
    {
        protected override string Code => "<<";
    }

    [Cil("shr")]
    public class Shr : Operation
    {
        protected override string Code => ">>";
    }

    [Cil("or")]
    public class Or : Operation
    {
        protected override string Code => "|";
    }

    [Cil("and")]
    public class And : Operation
    {
        protected override string Code => "&";
    }

    [Cil("xor")]
    public class Xor : Operation
    {
        protected override string Code => "^";
    }

    [Cil("rem")]
    public class Rem : Operation
    {
        protected override string Code => "%";
    }
}