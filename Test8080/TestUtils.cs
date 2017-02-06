using System;
using System.Linq;
using System.Collections.Generic;

namespace Test8080 
{
    public static class TestUtils
    {
        public static bool DoTests(List<KeyValuePair<string, bool>> tests, string header = "Test results:") {
            Console.WriteLine(header);

            foreach (var t in tests) {
                Console.Write($" - {t.Key} Test : ");
                PassOrFailPrint(t.Value);
            }

            return tests.All(x => x.Value);
        }

        public static void PassOrFailPrint(bool input) {
            Console.ForegroundColor = input ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(input ? "Pass" : "Fail");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
