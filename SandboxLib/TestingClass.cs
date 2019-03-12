using SharpC;

namespace SandboxLib
{
    /*
     * Just to demonstrate, no use.
     */
    
    public class TestingClass
    {
        public long Value;
        
        public TestingClass()
        {
            Value = (long) 554e5;
        }

        public void Method()
        {
            OtherMethod(6, 64323);
        }

        public long OtherMethod(int first, int second)
        {
            Value -= second;
            return (first << ~(5 ^ second)) * Value;
        }
    }
}