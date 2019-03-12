using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <summary>
    /// Common Intermediate Language Instruction.
    /// </summary>
    public class CilInstruction
    {
        /// <summary>
        /// Serialize CIL instruction
        /// </summary>
        /// <param name="template">Scope Instruction to serialize</param>
        public virtual void Serialize(ScopeInstruction template)
        {
        }

        /// <summary>
        /// Deserialize CIL instruction to C
        /// </summary>
        /// <param name="stack">Current stack</param>
        /// <param name="instructions">Current instruction set</param>
        /// <param name="body">Method body</param>
        /// <returns>Generated C Code</returns>
        public virtual string Deserialize(IList<ScopeVariable> stack,
            IList<ScopeInstruction> instructions, MethodBase body)
        {
            return "";
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Attribute for all CIL instructions, necessary to be targeted by collector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CilAttribute : Attribute
    {
        public readonly string Name;

        /// <inheritdoc />
        /// <summary>
        /// Register CIL instruction for collection.
        /// </summary>
        /// <param name="name">CIL instruction NAME, not bytecode</param>
        public CilAttribute(string name)
        {
            Name = name;
        }
    }
}