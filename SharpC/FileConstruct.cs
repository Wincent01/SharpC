using System.IO;
using System.Reflection;

namespace SharpC
{
    public static class FileConstruct
    {
        /// <summary>
        /// Path to generated file.
        /// </summary>
        public static readonly string FilePath =
            $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\visualized.c";

        /// <summary>
        /// Write to generated file.
        /// </summary>
        /// <param name="line"></param>
        public static void Write(string line)
        {
            var file = File.AppendText(FilePath);
            file.Write(line);
            file.Dispose();
        }
    }
}