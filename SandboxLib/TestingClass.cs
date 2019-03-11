using SharpC;

namespace SandboxLib
{
    public class Main
    {
        public void Entry()
        {
            object i = null;
            i = CMethods.Realloc(i, 10 * sizeof(char));
            LotsOfPars(54324, i, 'f', null, this);
        }

        public void LotsOfPars(long ree, object ria, char rep, string[] pointers, Main self)
        {
        }
    }

    public static class CMethods
    {
        [CMethodCover("realloc")]
        public static object Realloc(object obj, uint newSize)
        {
            return obj;
        }
    }
}