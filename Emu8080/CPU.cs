﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emu8080 {

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

        public ushort BCRegister {
            get {
                return (ushort)(Registers[0x01] << 8 | Registers[0x02]);
            } set {
                Registers[0x02] = (byte)(value & 0xFF);
                Registers[0x01] = (byte)(value >> 8);
            }
        }

        public ushort DERegister {
            get {
                return (ushort)(Registers[0x03] << 8 | Registers[0x04]);
            } set {
                Registers[0x04] = (byte)(value & 0xFF);
                Registers[0x03] = (byte)(value >> 8);
            }
        }

        public ushort HLRegister {
            get {
                return (ushort)(Registers[0x05] << 8 | Registers[0x06]);
            } set {
                Registers[0x06] = (byte)(value & 0xFF);
                Registers[0x05] = (byte)(value >> 8);
            }
        }

        // A 64KB array which emulates the on-board memory.
        //   0x0000 - 0x1FFF: ROM (8KB)
        //   0x2000 - 0x23FF: wRAM (1KB)
        //   0x2400 - 0x3FFF: vRAM (6KB)
        //   0x4000 - 0xFFFF: RAM Mirror
        public byte[] Memory;

        public byte Flags;
        public byte CarryBit = 0x01;
        public byte ParityBit = 0x04;
        public byte AuxCarryBit = 0x10;
        public byte InterruptBit = 0x20;
        public byte ZeroBit = 0x40;
        public byte SignBit = 0x80;

        // A 16b index register which is a stack pointer to memory.
        public ushort StackPointer;

        // A 16b program counter which points to the next instruction.
        public ushort ProgramCounter;

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
            return Memory[index + 1];
        }

        public byte GetByteAfterNext(ushort index) {
            return Memory[index + 2];
        }

        // Since we are in little-endian, need the second byte first.
        public byte[] GetTwoBytes(ushort index) {
            return new byte[] { GetByteAfterNext(index), GetNextByte(index) };
        }

        public ushort GetTwoBytesJoined(ushort index) {
            return (ushort)(GetByteAfterNext(index) << 8 | GetNextByte(index));
        }

        public ushort IncrementRegisterPair(ushort regpair) {
            var result = (ushort)(regpair + 1);
            return result;
        }

        public void FlagSet(ushort input, byte auxin, byte extrain = 0x01, bool do_sign = true, bool do_zero = true, bool do_parity = true, bool do_carry = false, bool do_auxcarry = true) {
            if (do_sign) {
                if ((input & 0x80) > 0) {
                    Flags |= SignBit;
                } else {
                    Flags &= (byte)(~SignBit & 0xFF);
                }
            }
            if (do_zero) {
                if (input == 0) {
                    Flags |= ZeroBit;
                } else {
                    Flags &= (byte)(~ZeroBit & 0xFF);
                }
            }
            if (do_parity) {
                if (((input & 0xFF) % 2) == 0) {
                    Flags |= ParityBit;
                } else {
                    Flags &= (byte)(~ParityBit & 0xFF);
                }
            }
            if (do_carry) {
                if (input >= 0x100 || input < 0) {
                    Flags |= CarryBit;
                } else {
                    Flags &= (byte)(~CarryBit & 0xFF);
                }
            }
            if (do_auxcarry) {
                if ((((extrain ^ input) ^ auxin) & 0x10) > 0) {
                    Flags |= AuxCarryBit;
                } else {
                    Flags &= (byte)(~AuxCarryBit & 0xFF);
                }
            }
        }

        public void FlagSet(short input, byte auxin, byte extrain = 0x01, bool do_sign = true, bool do_zero = true, bool do_parity = true, bool do_carry = false, bool do_auxcarry = true) {
            if (do_sign) {
                if ((input & 0x80) > 0) {
                    Flags |= SignBit;
                } else {
                    Flags &= (byte)(~SignBit & 0xFF);
                }
            }
            if (do_zero) {
                if (input == 0) {
                    Flags |= ZeroBit;
                } else {
                    Flags &= (byte)(~ZeroBit & 0xFF);
                }
            }
            if (do_parity) {
                if (((input & 0xFF) % 2) == 0) {
                    Flags |= ParityBit;
                } else {
                    Flags &= (byte)(~ParityBit & 0xFF);
                }
            }
            if (do_carry) {
                if (input >= 0x100 || input < 0) {
                    Flags |= CarryBit;
                } else {
                    Flags &= (byte)(~CarryBit & 0xFF);
                }
            }
            if (do_auxcarry) {
                if (((((extrain > 0x00 ? extrain : 0x01) ^ input) ^ auxin) & 0x10) > 0) {
                    Flags |= AuxCarryBit;
                } else {
                    Flags &= (byte)(~AuxCarryBit & 0xFF);
                }
            }
        }

        public byte AndBytes(byte left, byte right) {
            var result = (byte)(left & right);
            Console.WriteLine($"{left} & {right} = {result} ($20C0 = {Memory[0x20C0]})");
            FlagSet(result, left, right, do_carry: true);
            if ((((left & 0x08) >> 0x03) | ((right & 0x08) >> 0x03)) > 0) {
                Flags &= (byte)(~AuxCarryBit & 0xFF);
            }
            //Flags &= (byte)(~CarryBit & 0xFF);
            return result;
        }

        public byte XorBytes(byte left, byte right) {
            var result = (byte)(left ^ right);
            FlagSet(result, left, right, do_carry: true);
            Flags &= (byte)(~AuxCarryBit & 0xFF);
            Flags &= (byte)(~CarryBit & 0xFF);
            return result;
        }

        public byte OrBytes(byte left, byte right) {
            var result = (byte)(left | right);
            FlagSet(result, left, right, do_carry: true);
            Flags &= (byte)(~AuxCarryBit & 0xFF);
            Flags &= (byte)(~CarryBit & 0xFF);
            return result;
        }

        public byte IncrementByte(byte input) {
            var result = (byte)(input + 1);
            FlagSet(result, input);
            return result;
        }

        public byte DecrementByte(byte input) {
            var result = (byte)(input - 1);
            FlagSet(result, input);
            return result;
        }

        public byte AddBytes(byte left, byte right, bool carry = false) {
            if (carry && (Flags & CarryBit) > 0) {
                right++;
            }
            var result = (ushort)(left + right);
            FlagSet(result, left, right, do_carry: true);
            return (byte)(result & 0xFF);
        }

        public byte SubBytes(byte left, byte right, bool carry = false) {
            if (carry && (Flags & CarryBit) > 0) {
                right++;
            }
            var result = (ushort)(left - right);
            FlagSet(result, left, right, do_carry: true);
            return (byte)(result & 0xFF);
        }

        public void RotateAccumulatorLeft()
        {
            byte result = (byte)(((Registers[0x00] & 0x80) >> 7) | (Registers[0x00] << 1));
            FlagSet(result, Registers[0x00], 0x00, false, false, false, true, false);
            Registers[0x00] = result;
        }

        public void RotateAccumulatorRight()
        {
            byte result = (byte)(((Registers[0x00] & 0x01) << 7) | (Registers[0x00] >> 1));
            FlagSet(result, Registers[0x00], 0x00, false, false, false, true, false);
            Registers[0x00] = result;
        }

        public void AccumulatorAnd(byte input)
        {
            byte result = (byte)(Registers[0x00] & input);
            FlagSet(result, Registers[0x00], do_carry: true);
            Registers[0x00] = result;
        }

        public void AccumulatorAdd(byte input)
        {
            byte result = (byte)(Registers[0x00] + input);
            FlagSet(result, Registers[0x00], do_carry: true);
            Registers[0x00] = (byte)(result & 0xFF);
        }

        public ushort DoDoubleAdd(ushort source, ushort dest)
        {
            uint result = (uint)(source + dest);
            if (result > 0xFFFF) {
                Flags |= CarryBit;
            }
            return (ushort)(result & 0xFFFF);
        }

        public void RegisterSwap(byte reg1, byte reg2)
        {
            var temp = Registers[reg1];
            Registers[reg1] = Registers[reg2];
            Registers[reg2] = temp;
        }

        public void Reset() {
            Registers = new byte[0x7];
            Memory = new byte[0x10000];
            StackPointer = 0xFFFE;
            ProgramCounter = 0x0;
            Flags = 0x00;
        }

        public string FlagStr() {
            var sb = new StringBuilder();
            sb.Append((Flags & ZeroBit) > 0 ? "z" : ".");
            sb.Append((Flags & SignBit) > 0 ? "s" : ".");
            sb.Append((Flags & ParityBit) > 0 ? "p" : ".");
            sb.Append((Flags & AuxCarryBit) > 0 ? "a" : ((Flags & InterruptBit) > 0 ? "i" : "."));
            sb.Append((Flags & CarryBit) > 0 ? "c" : ".");
            return sb.ToString();
        }

        public CPUState() {
            Registers = new byte[0x7];
            Memory = new byte[0x10000];
            StackPointer = 0xFFFE;
            ProgramCounter = 0x0;
            Flags = 0x00;
        }
    }

    public class CPUInstruction
    {
        public string InstructionString;
        public Action OpAction;
        public byte Arity; 

        public CPUInstruction(string instrstring, Action act, byte arty) {
            InstructionString = instrstring;
            OpAction = act;
            Arity = arty;
        }
    }

    public static class CPU 
    {
        public static CPUState State = new CPUState();

        private static Dictionary<byte, CPUInstruction> Instructions = new Dictionary<byte, CPUInstruction>() {
            { 0x00, new CPUInstruction("NOP    ", () => { }, 1) },
            { 0x01, new CPUInstruction("LXI    B,", () => { Array.Copy(State.GetTwoBytes(State.ProgramCounter), 0x00, State.Registers, 0x01, 0x02); }, 3) },
            { 0x05, new CPUInstruction("DCR    B", () => { State.Registers[0x01] = State.DecrementByte(State.Registers[0x01]); }, 1) },
            { 0x06, new CPUInstruction("MVI    B,", () => { State.Registers[0x01] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x07, new CPUInstruction("RLC    ", () => { State.RotateAccumulatorLeft(); }, 1) },
            { 0x09, new CPUInstruction("DAD    B", () => {
                var result = State.DoDoubleAdd(State.HLRegister, State.BCRegister);
                State.HLRegister = result;
            }, 1) },
            { 0x0D, new CPUInstruction("DCR    C", () => { State.Registers[0x02] = State.DecrementByte(State.Registers[0x02]); }, 1) },
            { 0x0E, new CPUInstruction("MVI    C,", () => { State.Registers[0x02] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x0F, new CPUInstruction("RRC    ", () => { State.RotateAccumulatorRight(); }, 1) },
            { 0x11, new CPUInstruction("LXI    D,", () => { Array.Copy(State.GetTwoBytes(State.ProgramCounter), 0x00, State.Registers, 0x03, 0x02); }, 3) },
            { 0x13, new CPUInstruction("INX    D", () => { State.DERegister = State.IncrementRegisterPair(State.DERegister); }, 1) },
            { 0x19, new CPUInstruction("DAD    D", () => {
                var result = State.DoDoubleAdd(State.HLRegister, State.DERegister);
                State.Registers[0x05] = (byte)(result / 0x100);
                State.Registers[0x06] = (byte)(result & 0xFF);
            }, 1) },
            { 0x1A, new CPUInstruction("LDAX   D", () => { State.Registers[0x00] = State.Memory[State.DERegister]; }, 1) },
            { 0x20, new CPUInstruction("RIM    ", () => { /* Coming eventually. */ }, 1) },
            { 0x21, new CPUInstruction("LXI    H,", () => { Array.Copy(State.GetTwoBytes(State.ProgramCounter), 0x00, State.Registers, 0x05, 0x02); }, 3) },
            { 0x23, new CPUInstruction("INX    H", () => { State.HLRegister = State.IncrementRegisterPair(State.HLRegister); }, 1) },
            { 0x26, new CPUInstruction("MVI    H,", () => { State.Registers[0x05] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x29, new CPUInstruction("DAD    H", () => {
                var result = State.DoDoubleAdd(State.HLRegister, State.HLRegister);
                State.Registers[0x05] = (byte)(result / 0x100);
                State.Registers[0x06] = (byte)(result & 0xFF);
            }, 1) },
            { 0x2A, new CPUInstruction("LHLD   ", () => {
                var addr = State.GetTwoBytesJoined(State.ProgramCounter);
                State.Registers[0x05] = State.Memory[addr + 1];
                State.Registers[0x06] = State.Memory[addr];
            }, 3) },
            { 0x2E, new CPUInstruction("MVI    L,", () => { State.Registers[0x06] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x31, new CPUInstruction("LXI    SP,", () => { State.StackPointer = State.GetTwoBytesJoined(State.ProgramCounter); }, 3) },
            { 0x32, new CPUInstruction("STA    ", () => { Array.Copy(State.Registers, 0x00, State.Memory, State.GetTwoBytesJoined(State.ProgramCounter), 0x01); }, 3) },
            { 0x35, new CPUInstruction("DCR    M", () => { State.Memory[State.HLRegister] = State.DecrementByte(State.Memory[State.HLRegister]); }, 1) },
            { 0x36, new CPUInstruction("MVI    M,", () => { State.Memory[State.HLRegister] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x3A, new CPUInstruction("LDA    A,", () => { State.Registers[0x00] = State.Memory[State.GetTwoBytesJoined(State.ProgramCounter)]; }, 3) },
            { 0x3E, new CPUInstruction("MVI    A,", () => { State.Registers[0x00] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x3F, new CPUInstruction("CMC    ", () => { State.Flags &= (byte)(~State.CarryBit & 0xFF); }, 1) },
            { 0x40, new CPUInstruction("MOV    B,B", () => {  }, 1) },
            { 0x41, new CPUInstruction("MOV    B,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x01, 0x01); }, 1) },
            { 0x42, new CPUInstruction("MOV    B,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x01, 0x01); }, 1) },
            { 0x43, new CPUInstruction("MOV    B,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x01, 0x01); }, 1) },
            { 0x44, new CPUInstruction("MOV    B,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x01, 0x01); }, 1) },
            { 0x45, new CPUInstruction("MOV    B,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x01, 0x01); }, 1) },
            { 0x46, new CPUInstruction("MOV    B,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x01, 0x01); }, 1) },
            { 0x47, new CPUInstruction("MOV    B,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x01, 0x01); }, 1) },
            { 0x48, new CPUInstruction("MOV    C,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x02, 0x01); }, 1) },
            { 0x49, new CPUInstruction("MOV    C,C", () => {  }, 1) },
            { 0x4A, new CPUInstruction("MOV    C,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4B, new CPUInstruction("MOV    C,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4C, new CPUInstruction("MOV    C,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4D, new CPUInstruction("MOV    C,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4E, new CPUInstruction("MOV    C,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4F, new CPUInstruction("MOV    C,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x02, 0x01); }, 1) },
            { 0x50, new CPUInstruction("MOV    D,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x03, 0x01); }, 1) },
            { 0x51, new CPUInstruction("MOV    D,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x03, 0x01); }, 1) },
            { 0x52, new CPUInstruction("MOV    D,D", () => {  }, 1) },
            { 0x53, new CPUInstruction("MOV    D,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x03, 0x01); }, 1) },
            { 0x54, new CPUInstruction("MOV    D,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x03, 0x01); }, 1) },
            { 0x55, new CPUInstruction("MOV    D,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x03, 0x01); }, 1) },
            { 0x56, new CPUInstruction("MOV    D,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x03, 0x01); }, 1) },
            { 0x57, new CPUInstruction("MOV    D,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x04, 0x01); }, 1) },
            { 0x58, new CPUInstruction("MOV    E,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x04, 0x01); }, 1) },
            { 0x59, new CPUInstruction("MOV    E,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5A, new CPUInstruction("MOV    E,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5B, new CPUInstruction("MOV    E,E", () => {  }, 1) },
            { 0x5C, new CPUInstruction("MOV    E,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5D, new CPUInstruction("MOV    E,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5E, new CPUInstruction("MOV    E,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5F, new CPUInstruction("MOV    E,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x05, 0x01); }, 1) },
            { 0x60, new CPUInstruction("MOV    H,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x05, 0x01); }, 1) },
            { 0x61, new CPUInstruction("MOV    H,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x05, 0x01); }, 1) },
            { 0x62, new CPUInstruction("MOV    H,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x05, 0x01); }, 1) },
            { 0x63, new CPUInstruction("MOV    H,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x05, 0x01); }, 1) },
            { 0x64, new CPUInstruction("MOV    H,H", () => {  }, 1) },
            { 0x65, new CPUInstruction("MOV    H,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x05, 0x01); }, 1) },
            { 0x66, new CPUInstruction("MOV    H,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x05, 0x01); }, 1) },
            { 0x67, new CPUInstruction("MOV    H,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x06, 0x01); }, 1) },
            { 0x68, new CPUInstruction("MOV    L,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x06, 0x01); }, 1) },
            { 0x69, new CPUInstruction("MOV    L,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6A, new CPUInstruction("MOV    L,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6B, new CPUInstruction("MOV    L,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6C, new CPUInstruction("MOV    L,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6D, new CPUInstruction("MOV    L,L", () => {  }, 1) },
            { 0x6E, new CPUInstruction("MOV    L,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6F, new CPUInstruction("MOV    L,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x06, 0x01); }, 1) },
            { 0x70, new CPUInstruction("MOV    M,B", () => { Array.Copy(State.Registers, 0x01, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x71, new CPUInstruction("MOV    M,C", () => { Array.Copy(State.Registers, 0x02, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x72, new CPUInstruction("MOV    M,D", () => { Array.Copy(State.Registers, 0x03, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x73, new CPUInstruction("MOV    M,E", () => { Array.Copy(State.Registers, 0x04, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x74, new CPUInstruction("MOV    M,H", () => { Array.Copy(State.Registers, 0x05, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x75, new CPUInstruction("MOV    M,L", () => { Array.Copy(State.Registers, 0x06, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x76, new CPUInstruction("HLT    ", () => { /* Program should halt! */ }, 1) },
            { 0x77, new CPUInstruction("MOV    M,A", () => { Array.Copy(State.Registers, 0x00, State.Memory, State.HLRegister, 0x01); }, 1) },
            { 0x78, new CPUInstruction("MOV    A,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x00, 0x01); }, 1) },
            { 0x79, new CPUInstruction("MOV    A,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7A, new CPUInstruction("MOV    A,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7B, new CPUInstruction("MOV    A,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7C, new CPUInstruction("MOV    A,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7D, new CPUInstruction("MOV    A,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x00, 0x01);}, 1) },
            { 0x7E, new CPUInstruction("MOV    A,M", () => { Array.Copy(State.Memory, State.HLRegister, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7F, new CPUInstruction("MOV    A,A", () => {  }, 1) },
            { 0x80, new CPUInstruction("ADD    B", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x01], State.Registers[0x00]); }, 1) },
            { 0x81, new CPUInstruction("ADD    C", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x02], State.Registers[0x00]); }, 1) },
            { 0x82, new CPUInstruction("ADD    D", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x03], State.Registers[0x00]); }, 1) },
            { 0x83, new CPUInstruction("ADD    E", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x04], State.Registers[0x00]); }, 1) },
            { 0x84, new CPUInstruction("ADD    H", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x05], State.Registers[0x00]); }, 1) },
            { 0x85, new CPUInstruction("ADD    L", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x06], State.Registers[0x00]); }, 1) },
            { 0x86, new CPUInstruction("ADD    M", () => { State.Registers[0x00] = State.AddBytes(State.Memory[State.HLRegister], State.Registers[0x00]); }, 1) },
            { 0x87, new CPUInstruction("ADD    A", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x00], State.Registers[0x00]); }, 1) },
            { 0x88, new CPUInstruction("ADC    B", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x01], State.Registers[0x00], carry: true); }, 1) },
            { 0x89, new CPUInstruction("ADC    C", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x02], State.Registers[0x00], carry: true); }, 1) },
            { 0x8A, new CPUInstruction("ADC    D", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x03], State.Registers[0x00], carry: true); }, 1) },
            { 0x8B, new CPUInstruction("ADC    E", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x04], State.Registers[0x00], carry: true); }, 1) },
            { 0x8C, new CPUInstruction("ADC    H", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x05], State.Registers[0x00], carry: true); }, 1) },
            { 0x8D, new CPUInstruction("ADC    L", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x06], State.Registers[0x00], carry: true); }, 1) },
            { 0x8E, new CPUInstruction("ADC    M", () => { State.Registers[0x00] = State.AddBytes(State.Memory[State.HLRegister], State.Registers[0x00], carry: true); }, 1) },
            { 0x8F, new CPUInstruction("ADC    A", () => { State.Registers[0x00] = State.AddBytes(State.Registers[0x00], State.Registers[0x00], carry: true); }, 1) },
            { 0x90, new CPUInstruction("SUB    B", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x01], State.Registers[0x00]); }, 1) },
            { 0x91, new CPUInstruction("SUB    C", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x02], State.Registers[0x00]); }, 1) },
            { 0x92, new CPUInstruction("SUB    D", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x03], State.Registers[0x00]); }, 1) },
            { 0x93, new CPUInstruction("SUB    E", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x04], State.Registers[0x00]); }, 1) },
            { 0x94, new CPUInstruction("SUB    H", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x05], State.Registers[0x00]); }, 1) },
            { 0x95, new CPUInstruction("SUB    L", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x06], State.Registers[0x00]); }, 1) },
            { 0x96, new CPUInstruction("SUB    M", () => { State.Registers[0x00] = State.SubBytes(State.Memory[State.HLRegister], State.Registers[0x00]); }, 1) },
            { 0x97, new CPUInstruction("SUB    A", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x00], State.Registers[0x00]); }, 1) },
            { 0x98, new CPUInstruction("SBB    B", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x01], State.Registers[0x00], carry: true); }, 1) },
            { 0x99, new CPUInstruction("SBB    C", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x02], State.Registers[0x00], carry: true); }, 1) },
            { 0x9A, new CPUInstruction("SBB    D", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x03], State.Registers[0x00], carry: true); }, 1) },
            { 0x9B, new CPUInstruction("SBB    E", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x04], State.Registers[0x00], carry: true); }, 1) },
            { 0x9C, new CPUInstruction("SBB    H", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x05], State.Registers[0x00], carry: true); }, 1) },
            { 0x9D, new CPUInstruction("SBB    L", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x06], State.Registers[0x00], carry: true); }, 1) },
            { 0x9E, new CPUInstruction("SBB    M", () => { State.Registers[0x00] = State.SubBytes(State.Memory[State.HLRegister], State.Registers[0x00], carry: true); }, 1) },
            { 0x9F, new CPUInstruction("SBB    A", () => { State.Registers[0x00] = State.SubBytes(State.Registers[0x00], State.Registers[0x00], carry: true); }, 1) },
            { 0xA0, new CPUInstruction("ANA    B", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x01], State.Registers[0x00]); }, 1) },
            { 0xA1, new CPUInstruction("ANA    C", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x02], State.Registers[0x00]); }, 1) },
            { 0xA2, new CPUInstruction("ANA    D", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x03], State.Registers[0x00]); }, 1) },
            { 0xA3, new CPUInstruction("ANA    E", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x04], State.Registers[0x00]); }, 1) },
            { 0xA4, new CPUInstruction("ANA    H", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x05], State.Registers[0x00]); }, 1) },
            { 0xA5, new CPUInstruction("ANA    L", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x06], State.Registers[0x00]); }, 1) },
            { 0xA6, new CPUInstruction("ANA    M", () => { State.Registers[0x00] = State.AndBytes(State.Memory[State.HLRegister], State.Registers[0x00]); }, 1) },
            { 0xA7, new CPUInstruction("ANA    A", () => { State.Registers[0x00] = State.AndBytes(State.Registers[0x00], State.Registers[0x00]); }, 1) },
            { 0xA8, new CPUInstruction("XRA    B", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x01], State.Registers[0x00]); }, 1) },
            { 0xA9, new CPUInstruction("XRA    C", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x02], State.Registers[0x00]); }, 1) },
            { 0xAA, new CPUInstruction("XRA    D", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x03], State.Registers[0x00]); }, 1) },
            { 0xAB, new CPUInstruction("XRA    E", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x04], State.Registers[0x00]); }, 1) },
            { 0xAC, new CPUInstruction("XRA    H", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x05], State.Registers[0x00]); }, 1) },
            { 0xAD, new CPUInstruction("XRA    L", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x06], State.Registers[0x00]); }, 1) },
            { 0xAE, new CPUInstruction("XRA    M", () => { State.Registers[0x00] = State.XorBytes(State.Memory[State.HLRegister], State.Registers[0x00]); }, 1) },
            { 0xAF, new CPUInstruction("XRA    A", () => { State.Registers[0x00] = State.XorBytes(State.Registers[0x00], State.Registers[0x00]); }, 1) },
            { 0xC1, new CPUInstruction("POP    B", () => {
                State.Registers[0x02] = State.Memory[State.StackPointer];
                State.Registers[0x01] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xC2, new CPUInstruction("JNZ    ", () => {
                if ((State.Flags & State.ZeroBit) == 0) {
                    State.ProgramCounter = (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3);
                }
            }, 3) },
            { 0xC3, new CPUInstruction("JMP    ", () => { State.ProgramCounter = (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3); }, 3) },
            { 0xC5, new CPUInstruction("PUSH   B", () => {
                State.StackPointer -= 0x02;
                State.Memory[State.StackPointer] = State.Registers[0x02];
                State.Memory[State.StackPointer + 1] = State.Registers[0x01];
            }, 1) },
            { 0xC6, new CPUInstruction("ADI    ", () => {
                State.AccumulatorAdd(State.GetNextByte(State.ProgramCounter));
            }, 2) },
            { 0xC9, new CPUInstruction("RET    ", () => {
                State.ProgramCounter = State.GetTwoBytesJoined((ushort)(State.StackPointer - 1));
                State.StackPointer += 0x02;
            }, 3) },
            { 0xCA, new CPUInstruction("JZ     ", () => {
                if ((State.Flags & State.ZeroBit) != 0) {
                    State.ProgramCounter = (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3);
                }
            }, 3) },
            { 0xCD, new CPUInstruction("CALL   ", () => {
                State.StackPointer -= 0x02;
                Array.Copy(new byte[] { (byte)(State.ProgramCounter & 0xFF), (byte)(State.ProgramCounter / 0x100) }, 0x00, State.Memory, State.StackPointer, 0x02);
                State.ProgramCounter = (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3);
            }, 3) },
            { 0xD1, new CPUInstruction("POP    D", () => {
                State.Registers[0x04] = State.Memory[State.StackPointer];
                State.Registers[0x03] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xDA, new CPUInstruction("JC     ", () => {
                if ((State.Flags & State.CarryBit) != 0) {
                    State.ProgramCounter = (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3);
                }
            }, 3) },
            { 0xD3, new CPUInstruction("OUT    ", () => { /* Does nothing for now. */ }, 2) },
            { 0xD5, new CPUInstruction("PUSH   D", () => {
                State.StackPointer -= 0x02;
                State.Memory[State.StackPointer] = State.Registers[0x04];
                State.Memory[State.StackPointer + 1] = State.Registers[0x03];
            }, 1) },
            { 0xDB, new CPUInstruction("IN     ", () => { /* Does nothing for now. */ }, 2) },
            { 0xE1, new CPUInstruction("POP    H", () => {
                State.Registers[0x06] = State.Memory[State.StackPointer];
                State.Registers[0x05] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xE5, new CPUInstruction("PUSH   H", () => {
                State.StackPointer -= 0x02;
                State.Memory[State.StackPointer] = State.Registers[0x06];
                State.Memory[State.StackPointer + 1] = State.Registers[0x05];
            }, 1) },
            { 0xE6, new CPUInstruction("ANI    ", () => {
                State.AccumulatorAnd(State.GetNextByte(State.ProgramCounter));
            }, 2) },
            { 0xEB, new CPUInstruction("XCHG", () => {
                State.RegisterSwap(0x05, 0x03);
                State.RegisterSwap(0x06, 0x04);
            }, 1) },
            { 0xF1, new CPUInstruction("POP    PSW", () => {
                State.Flags = State.Memory[State.StackPointer];
                State.Registers[0x00] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xF3, new CPUInstruction("DI     ", () => { State.Flags &= (byte)(~State.InterruptBit & 0xFF); }, 1) },
            { 0xF5, new CPUInstruction("PUSH   PSW", () => {
                State.StackPointer -= 0x02;
                State.Memory[State.StackPointer] = State.Flags;
                State.Memory[State.StackPointer + 1] = State.Registers[0x00];
            }, 1) },
            { 0xFB, new CPUInstruction("EI     ", () => { State.Flags |= State.InterruptBit; }, 1) },
            { 0xFE, new CPUInstruction("CPI    ", () => {
                short result = (short)(State.Registers[0x00] - State.GetNextByte(State.ProgramCounter));
                State.FlagSet(result, State.Registers[0x00], do_carry: true);
            }, 2) }
        };

        public static Dictionary<byte, string> InstructionText = new Dictionary<byte, string>();

        public static bool Step(bool debug = false, bool printonly = false, bool fullprint = true, int counter = 0, int max = 0) {
            var inst = State.Memory[State.ProgramCounter];
            try {
                var instruct = Instructions[inst];
                if (debug) {
                    if ((!fullprint && (max - counter) < 25) || (debug && fullprint)) {
                        var outs = new StringBuilder();
                        //outs.Append($"[{State.Registers[0x00].ToString("X2")}{State.Flags.ToString("X2")} {State.BCRegister.ToString("X4")} {State.DERegister.ToString("X4")} {State.HLRegister.ToString("X4")} {State.StackPointer.ToString("X4")} {State.FlagStr()}] ");
                        outs.Append($"0x{State.ProgramCounter.ToString("X4")}: {instruct.InstructionString}");
                        if (instruct.Arity == 2) {
                            outs.Append($"${State.GetNextByte(State.ProgramCounter).ToString("X2")}");
                        } else if (instruct.Arity == 3) {
                            outs.Append($"${State.GetTwoBytesJoined(State.ProgramCounter).ToString("X4")}");
                        }
                        Console.WriteLine(outs.ToString());
                    }
                }
                if (!printonly) {
                    instruct.OpAction();
                }
                State.ProgramCounter += instruct.Arity;
                return true;
            } catch (Exception ex) {
                Console.WriteLine($"[0x{State.ProgramCounter.ToString("X4")}] Error: Opcode 0x{inst.ToString("X2")} not found.");
                return false;
            }
        }
    }
}
