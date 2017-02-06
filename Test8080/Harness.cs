﻿using System;
using Emu8080;

namespace Test8080 
{
    public static class Harness 
    {
        public static bool CheckConditions(byte[] program, Func<CPU, bool> conditions, Action<CPU> setup = null) {
            CPU env = new CPU();
            env.Load(program);
            setup?.Invoke(env);
            
            while (env.Registers.PC < program.Length && env.Step()) { }

            return conditions(env);
        }
    }
}
