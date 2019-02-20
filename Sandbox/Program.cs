using System;
using SandboxLib;
using SharpC;

namespace Sandbox
{
    internal static class Program
    {
        public static void Main()
        {
            Visualizer.Init(typeof(Main).Assembly, VisualizerState.Standard);
            Visualizer.Deserialize();
        }
    }
}