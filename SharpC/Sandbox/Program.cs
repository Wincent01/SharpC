using System.Reflection;
using SharpC;

namespace Sandbox
{
    internal static class Program
    {
        public static void Main()
        {
            var vis = new Visualizer(Assembly.GetExecutingAssembly());
            vis.Deserialize();
        }
    }

    public class TestClass1
    {
        public TestClass1()
        {
            
        }
        
        public uint TestMethod1(int i)
        {
            if (i == 4)
            {
                if (i + 3 == 7)
                {
                    return 1;
                }
                return 6;
            }
            if (i + 3 == 7)
            {
                return 1;
            }
            return (uint) i;
        }

        public ulong TestMethod2(uint i, uint l)
        {
            int ll = 5;
            long tt = 3;
            long iii = tt + l;
            return (ulong) iii;
        }

        public byte TestMethod3()
        {
            return 8 + 4;
        }

        public int Function(int i)
        {
            return i + 2;
        }
    }

    public class TestClass2
    {
        public TestClass2()
        {
            
        }
        
        public ushort TestMethod3(ushort l)
        {
            var test = new TestClass1();
            var i = test.Function(4);
            int il = i;
            return (ushort) (il + l);
        }
    }
}