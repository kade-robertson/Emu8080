using System;
using System.Collections.Generic;
using System.Text;

namespace Emu8080 
{
    public class CPU 
    {
        public enum Register {
            A = 0,
            B = 1,
            C = 2,
            D = 3,
            E = 4,
            H = 5,
            L = 6
        }
        // A 7b array which stores the accumulator and various registers.
        public byte[] Registers;

        // A 64kb arrat which emulates the on-board memory.
        public byte[] Memory;

        // A 16b index register which is a stack pointer to memory.
        public ushort StackPointer;

        // A 16b program counter which points to the next instruction.
        public ushort ProgramCounter;

        // If the MSB bit of an instruction has the value ‘1’, this flag is set; or else, it is reset.
        public bool Sign;

        // If the result of an instruction has the value ‘0’, this zero flag is set; or else, it is reset.
        public bool Zero;

        // If the number of the set bits in the result has even value, this flag is set; or else, it is reset.
        public bool Parity;

        // If there was a carry during borrow, addition, subtraction or comparison, this flag is set; or else, it is reset.
        public bool Carry;

        // If there was a carry out from 3-bit to 4-bit of the result, this flag is set; otherwise, it is reset.
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

        private byte GetNextByte() {
            return Memory[ProgramCounter + 1];
        }

        private byte GetByteAfterNext() {
            return Memory[ProgramCounter + 2];
        }

        private ushort GetTwoBytes() {
            return (ushort)(GetByteAfterNext() * 0x100 + GetNextByte());
        }

        public void Step() {
            var inst = Memory[ProgramCounter];
            switch (inst) {
                case 0x00: // NOP
                    ProgramCounter += 1;
                    break;
                case 0x01: // LXI B, D16
                    Registers[(int)Register.B] = GetByteAfterNext();
                    Registers[(int)Register.C] = GetNextByte();
                    ProgramCounter += 1;
                    break;
                case 0xC2: // JNZ (addr)
                    ProgramCounter = Zero ? ProgramCounter : GetTwoBytes();
                    break;
                case 0xC3: // JMP (addr)
                    ProgramCounter = GetTwoBytes();
                    break;
            }
        }
    }
}
