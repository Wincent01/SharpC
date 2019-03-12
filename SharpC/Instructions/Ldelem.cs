using System.Collections.Generic;
using System.Reflection;

namespace SharpC.Instructions
{
    /// <inheritdoc />
    /// <summary>
    /// Push value at index of array onto stack.
    /// </summary>
    [Cil("ldelem")]
    public class Ldelem : CilInstruction
    {
        private bool _ref;
        private string _type;

        public override void Serialize(ScopeInstruction template)
        {
            if (template.Name.Split('.')[1] == "ref")
            {
                _ref = true;
                return;
            }

            _type = CType.ResolveConv(template.Name.Split('.')[1]);
        }

        public override string Deserialize(IList<ScopeVariable> stack, IList<ScopeInstruction> instructions,
            MethodBase body)
        {
            var index = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var array = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if (_ref) _type = array.Type.Remove(array.Type.Length - 1);

            stack.Add(new ScopeVariable
            {
                Type = _type,
                Value = $"(({_type}) ({array.Value}[{index.Value}]))"
            });
            return "";
        }
    }
}