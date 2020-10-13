using System;
using System.Diagnostics;

namespace SimpleNetCore
{

    public static class NetDebug
    {
        public static Action<string> Log = Console.WriteLine;
        public static Action<string> LogWarning = Console.WriteLine;
        public static Action<string> LogError = Console.Error.WriteLine;
    }
}
