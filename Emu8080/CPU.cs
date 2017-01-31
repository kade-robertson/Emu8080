using System;
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

    public class CPU 
    {

        public byte[] CPUMemory;
        public Registers CPURegisters;
        public Flag CPUFlag;

        public bool Step(bool debug = true) {
            var inst = CPUMemory[CPURegisters.PC];
            try {
                var todo = InstructionSet.Instructions[inst];
                var args = new byte[2];
                if (todo.Arity == 2) {
                    args[0] = CPUMemory[++CPURegisters.PC];
                }
                if (todo.Arity == 3) {
                    args[1] = CPUMemory[++CPURegisters.PC];
                }
                CPURegisters.PC++;
                return todo.Execute(CPUMemory, args, CPURegisters, CPUFlag);
            } catch (Exception ex) {
                Console.WriteLine($"[0x{CPURegisters.PC.ToString("X4")}] Error: Opcode 0x{inst.ToString("X2")} not found.");
                return false;
            }
        }

        public bool Check() {
            foreach (KeyValuePair<byte, Instruction> k in InstructionSet.Instructions) {
                Console.WriteLine(k.Key.ToString("X2") + " : " + k.Value.Text);
            }
            return true;
        }

        public CPU() {
            CPUMemory = new byte[65536];
            CPURegisters = new Registers();
            CPUFlag = new Flag();
        }

        public CPU(byte[] rom, int index = 0) {
            CPUMemory = new byte[65536];
            CPURegisters = new Registers();
            CPUFlag = new Flag();
            Array.Copy(rom, 0, CPUMemory, index, rom.Length);
        }
    }
}
