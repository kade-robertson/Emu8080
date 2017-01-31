using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080 
{
    public class Instruction 
    {
        public string Text;
                   //    mem,   args, registers, flag
        public Action<byte[], byte[], Registers, Flag> Execute;
        public byte Arity;
        public byte Cycles;

        public Instruction(string text, Action<byte[], byte[], Registers, Flag> execute, byte arity, byte cycles) {
            Text = text;
            Execute = execute;
            Arity = arity;
            Cycles = cycles;
        }

        public Instruction() {
            Text = string.Empty;
            Execute = null;
            Arity = 0;
            Cycles = 0;
        }
    }
}
