using System;
using SharpC;

namespace SandboxLib
{
    public class Main
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public ulong Add(ulong a, ulong b)
        {
            return a + b;
        }
    }

    public class Sign
    {
        public int Signature;
        
        public Sign(int i)
        {
            Signature = i;
        }

        public int ToUlong()
        {
            return new Main().Add(Signature, 8);
        }
    }
}