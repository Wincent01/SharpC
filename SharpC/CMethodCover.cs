using System;

namespace SharpC
{
    public class CMethodCoverAttribute : Attribute
    {
        public readonly string Method;

        public CMethodCoverAttribute(string method)
        {
            Method = method;
        }
    }
}