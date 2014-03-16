using System;

namespace ShoyConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = ResharperKey.GetLicense("shoy", "");
            Console.WriteLine(key);
        }
    }
}
