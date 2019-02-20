using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    public class CilInstruction
    {
        public virtual void Serialize(ScopeInstruction template)
        {
            
        }

        public virtual string Deserialize(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body, int indite)
        {
            return "";
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CilAttribute : Attribute
    {
        public string Name;
        
        public CilAttribute(string name)
        {
            Name = name;
        }
    }
}