using System;
using System.Collections.Generic;
using System.Text;

namespace Emu8080 
{
    public class CPUState {
        // A 7B array which stores the accumulator and various registers.
        //   0x00 : A (accumulator)
        //   0x01 : B
        //   0x02 : C
        //   0x03 : D
        //   0x04 : E
        //   0x05 : H
        //   0x06 : L
        public byte[] Registers;

        // A 64KB array which emulates the on-board memory.
        //   0x0000 - 0x1FFF: ROM (8KB)
        //   0x2000 - 0x23FF: wRAM (1KB)
        //   0x2400 - 0x3FFF: vRAM (6KB)
        //   0x4000 - 0xFFFF: RAM Mirror
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

        public bool LoadROM(byte[] romdata) {
            if (romdata.Length > 0x2000) {
                return false;
            }
            try {
                Array.Copy(romdata, 0, Memory, 0, romdata.Length);
                return true;
            } catch {
                return false;
            }
        }

        public byte GetNextByte(ushort index) {
            return Memory[ProgramCounter + 1];
        }

        public byte GetByteAfterNext(ushort index) {
            return Memory[index + 2];
        }

        // Since we are in little-endian, need the second byte first.
        public byte[] GetTwoBytes(ushort index) {
            return new byte[] { GetByteAfterNext(index), GetNextByte(index) };
        }
        
        public ushort GetTwoBytesJoined(ushort index) {
            return (ushort)(GetByteAfterNext(index) * 0x100 + GetNextByte(index));
        }

        public ushort HLRegisterToAddress() {
            return (ushort)(Registers[0x05] * 0x100 + Registers[0x06]);
        }

        public CPUState() {
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

    public static class CPU 
    {
        public static CPUState State = new CPUState();

        private static Dictionary<byte, Action> Instructions = new Dictionary<byte, Action>() {
            { 0x00 /* NOP        */, () => { } }, 
            { 0x01 /* LXI B, D16 */, () => { Array.Copy(State.GetTwoBytes(State.ProgramCounter), 0x00, State.Registers, 0x01, 0x02); } },
            { 0x3F /* CMC        */, () => { State.Carry = !State.Carry; } },
            { 0x40 /* MOV B, B   */, () => { } },
            { 0x41 /* MOV B, C   */, () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x01, 0x01); } },
            { 0x42 /* MOV B, D   */, () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x01, 0x01); } },
            { 0x43 /* MOV B, E   */, () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x01, 0x01); } },
            { 0x44 /* MOV B, H   */, () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x01, 0x01); } },
            { 0x45 /* MOV B, L   */, () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x01, 0x01); } },
            { 0x46 /* MOV B, M   */, () => { Array.Copy(State.Memory, State.HLRegisterToAddress(), State.Registers, 0x01, 0x01); } },
            { 0x47 /* MOV B, A   */, () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x01, 0x01); } },
            { 0x48 /* MOV C, B   */, () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x02, 0x01); } },
            { 0x49 /* MOV C, C   */, () => { } },
            { 0x4A /* MOV C, D   */, () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x02, 0x01); } },
            { 0x4B /* MOV C, E   */, () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x02, 0x01); } },
            { 0x4C /* MOV C, H   */, () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x02, 0x01); } },
            { 0x4D /* MOV C, L   */, () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x02, 0x01); } },
            { 0x4E /* MOV C, M   */, () => { Array.Copy(State.Memory, State.HLRegisterToAddress(), State.Registers, 0x02, 0x01); } },
            { 0x4F /* MOV C, A   */, () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x02, 0x01); } },
            { 0x50 /* MOV D, B   */, () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x03, 0x01); } },
            { 0x51 /* MOV D, C   */, () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x03, 0x01); } },
            { 0x52 /* MOV D, D   */, () => { } },
            { 0x53 /* MOV D, E   */, () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x03, 0x01); } },
            { 0x54 /* MOV D, H   */, () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x03, 0x01); } },
            { 0x55 /* MOV D, L   */, () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x03, 0x01); } },
            { 0x56 /* MOV D, M   */, () => { Array.Copy(State.Memory, State.HLRegisterToAddress(), State.Registers, 0x03, 0x01); } },
            { 0x57 /* MOV D, A   */, () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x04, 0x01); } },
            { 0x58 /* MOV E, B   */, () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x04, 0x01); } },
            { 0x59 /* MOV E, C   */, () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x04, 0x01); } },
            { 0x5A /* MOV E, D   */, () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x04, 0x01); } },
            { 0x5B /* MOV E, E   */, () => { } },
            { 0x5C /* MOV E, H   */, () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x04, 0x01); } },
            { 0x5D /* MOV E, L   */, () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x04, 0x01); } },
            { 0x5E /* MOV E, M   */, () => { Array.Copy(State.Memory, State.HLRegisterToAddress(), State.Registers, 0x04, 0x01); } },
            { 0x5F /* MOV E, A   */, () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x05, 0x01); } },
            { 0x60 /* MOV H, B   */, () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x05, 0x01); } },
            { 0x61 /* MOV H, C   */, () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x05, 0x01); } },
            { 0x62 /* MOV H, D   */, () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x05, 0x01); } },
            { 0x63 /* MOV H, E   */, () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x05, 0x01); } },
            { 0x64 /* MOV H, H   */, () => { } },
            { 0x65 /* MOV H, L   */, () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x05, 0x01); } },
            { 0x66 /* MOV H, M   */, () => { Array.Copy(State.Memory, State.HLRegisterToAddress(), State.Registers, 0x05, 0x01); } },
            { 0x67 /* MOV H, A   */, () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x06, 0x01); } },
            { 0x68 /* MOV L, B   */, () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x06, 0x01); } },
            { 0x69 /* MOV L, C   */, () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x06, 0x01); } },
            { 0x6A /* MOV L, D   */, () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x06, 0x01); } },
            { 0x6B /* MOV L, E   */, () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x06, 0x01); } },
            { 0x6C /* MOV L, H   */, () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x06, 0x01); } },
            { 0x6D /* MOV L, L   */, () => { } },
            { 0x6E /* MOV L, M   */, () => { Array.Copy(State.Memory, State.HLRegisterToAddress(), State.Registers, 0x06, 0x01); } },
            { 0x6F /* MOV L, A   */, () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x06, 0x01); } },
        };

        

        public static void Step() {
            var inst = State.Memory[State.ProgramCounter];
            Instructions[inst]();
            State.ProgramCounter += 1;
        }
    }
}
