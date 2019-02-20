using System.IO;
using System.Reflection;

namespace SharpC
{
    public static class FileConstruct
    {
        public static readonly string FilePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\visualized.c";

        public static void Write(string line)
        {
            var file = File.AppendText(FilePath);
            file.Write(line);
            file.Dispose();
        }
    }
}