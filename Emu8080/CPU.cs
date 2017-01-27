using System;
using System.Collections.Generic;
using System.Text;

namespace Emu8080 
{
    public class CPU 
    {
        // A 7b array which stores the accumulator and various registers.
        public byte[] Registers;

        // A 64kb arrat which emulates the on-board memory.
        public byte[] Memory;

        // A 16b index register which is a stack pointer to memory.
        public ushort StackPointer;

        // A 16b program counter which points to the next instruction.
        public ushort ProgramCounter;

        // A bit flag which is set if the result is negative.
        public bool Sign;

        // A bit flag which is set if the result is zero.
        public bool Zero;

        // A bit flag which is set if the number of bits in the result is even.
        public bool Parity;

        // A bit flag which is set if the result required a carry or borrow.
        public bool Carry;

        // A bit flag used for binary-coded decimal arithmetic.
        public bool AuxCarry;

        public CPU() {
            Registers = new byte[0x7];
            Memory = new byte[0x10000];
            StackPointer = 0x0;
            ProgramCounter = 0x0;
            Sign = false;
            Zero = false;
            Parity = false;
            Carry = false;
            AuxCarry = false;
        }
    }
}
