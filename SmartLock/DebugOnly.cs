using Microsoft.SPOT;
using System.Diagnostics;

namespace SmartLock
{
    public static class DebugOnly
    {
        [Conditional("DEBUG")]
        public static void Print(string text)
        {
            Debug.Print(text);
        }
    }
}
