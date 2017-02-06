using System;
using Emu8080;

namespace Test8080 
{
    public static class Harness 
    {
        public static bool CheckConditions(byte[] program, Func<CPU, bool> conditions, Action<CPU> setup = null, string goodmsg = "Test succeeded!") {
            CPU env = new CPU();
            env.Load(program);
            setup?.Invoke(env);

            var counter = 0;
            while (env.Registers.PC < program.Length && env.Step()) {
                counter += 1;
            }

            var result = conditions(env);
            if (result) {
                Console.WriteLine(goodmsg);
            }
            return result;
        }
    }
}
