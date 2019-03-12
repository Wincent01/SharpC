using System;

namespace SharpC
{
    /// <inheritdoc />
    /// <summary>
    /// For C method calls.
    /// Alloc, Realloc, Strlen etc...
    /// </summary>
    public class CMethodCoverAttribute : Attribute
    {
        public readonly string Method;

        public CMethodCoverAttribute(string method)
        {
            Method = method;
        }
    }
}