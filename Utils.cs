using System.Text;

namespace TQ.Mesh
{
    static class Utils
    {
        //TODO: Should be codepage 1252, but that's not supported by default.
        public static Encoding Encoding => Encoding.ASCII;
    }
}
