using System;
using Emu8080;

namespace Test8080 
{
    public static class Harness 
    {
        public static bool CheckConditions(byte[] program, Func<CPU, bool> conditions, Action<CPU> setup = null, bool debug = false) {
            CPU env = new CPU();
            env.DebugStream = Console.Out;
            env.Load(program);
            setup?.Invoke(env);
            
            while (env.Registers.PC < program.Length && env.Step(debug)) { }

            return conditions(env);
        }
    }
}
