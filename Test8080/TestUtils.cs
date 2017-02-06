using System;

namespace Test8080 
{
    public static class TestUtils
    {
        public static void PassOrFailPrint(bool input) {
            Console.ForegroundColor = input ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(input ? "Pass" : "Fail");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
