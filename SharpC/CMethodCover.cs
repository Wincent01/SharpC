using System;

namespace SharpC
{
    public class CMethodCoverAttribute : Attribute
    {
        public string Method;
        
        public CMethodCoverAttribute(string method)
        {
            Method = method;
        }
    }
}