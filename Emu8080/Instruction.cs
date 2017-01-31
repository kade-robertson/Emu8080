using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080 
{
    public class Instruction 
    {
        //    mem,   args, registers, flag, success
        public Func<byte[], byte[], CPURegisters, CPUFlag, bool> Execute;
        public string Text;     // the appropriate text representation
        public byte Arity;      // how many arguments the instruction has (including itself)
        public byte Cycles;     // how many cycles this takes when 'successful'
        public byte LowCycles;  // how many cycles this takes when 'unsuccessful'

        public Instruction(string text, Func<byte[], byte[], CPURegisters, CPUFlag, bool> execute, byte arity, byte cycles, byte lowcycles) {
            Text = text;
            Execute = execute;
            Arity = arity;
            Cycles = cycles;
            LowCycles = lowcycles;
        }

        public Instruction() {
            Text = string.Empty;
            Execute = null;
            Arity = 0;
            Cycles = 0;
            LowCycles = 0;
        }
    }
}
