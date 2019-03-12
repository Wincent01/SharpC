using SandboxLib;
using SharpC;

namespace Sandbox
{
    internal static class Program
    {
        public static void Main()
        {
            Visualizer.Init(typeof(TestingClass).Assembly, VisualizerState.Standard);
            Visualizer.Deserialize();
        }
    }
}