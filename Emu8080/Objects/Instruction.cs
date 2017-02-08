using System;

namespace Emu8080 
{
    public class Instruction 
    {
        //    mem,   args, registers, flag, success
        public Func<CPU, byte[], bool> Execute;
        public string Text;     // the appropriate text representation
        public byte Arity;      // how many arguments the instruction has (including itself)
        public byte Cycles;     // how many cycles this takes when 'successful'
        public byte LowCycles;  // how many cycles this takes when 'unsuccessful'
        public Func<byte[], string> GetPrintString;

        public Instruction(string text, Func<CPU, byte[], bool> execute, byte arity, byte cycles, byte lowcycles, Func<byte[], string> getprintstring) {
            Text = text;
            Execute = execute;
            Arity = arity;
            Cycles = cycles;
            LowCycles = lowcycles;
            GetPrintString = getprintstring;
        }

        public Instruction() {
            Text = string.Empty;
            Execute = null;
            Arity = 0;
            Cycles = 0;
            LowCycles = 0;
            GetPrintString = null;
        }
    }
}
