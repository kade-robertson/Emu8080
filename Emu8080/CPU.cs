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

        // Tells the processor whether to process interrupts. Currently un-used.
        public bool InterruptProcessingEnabled;

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
            return (ushort)(GetByteAfterNext(index) * 0x100 + GetNextByte(index));
        }

        public ushort RegisterToAddress(byte big, byte small) {
            return (ushort)(Registers[big] * 0x100 + Registers[small]);
        }

        public byte[] IncrementRegisterPair(byte big, byte small) {
            var result = Registers[big] * 0x100 + Registers[small] + 1;
            return new byte[] { (byte)(result / 0x100), (byte)(result & 0xFF) };
        }

        public bool CalculateParity(byte value) {
            var bits = new BitArray(value);
            var oneBits = 0;
            for (int i = 0; i < bits.Length; i++) {
                oneBits += bits[i] ? 1 : 0;
            }
            return (oneBits % 2) == 0;
        }

        public void FlagSet(ushort input, byte auxin, bool do_sign = true, bool do_zero = true, bool do_parity = true, bool do_carry = false, bool do_auxcarry = true)
        {
            if (do_sign) Sign = (input >> 7) == 1;
            if (do_zero) Zero = input == 0;
            if (do_parity) Parity = CalculateParity((byte)(input & 0xFF));
            if (do_carry) Carry = (input > 0xFF);
            if (do_auxcarry) AuxCarry = (auxin & 0xF) > (input & 0xF);
        }

        public void FlagSet(short input, byte auxin, bool do_sign = true, bool do_zero = true, bool do_parity = true, bool do_carry = false, bool do_auxcarry = true)
        {
            if (do_sign) Sign = (input >> 7) == 1;
            if (do_zero) Zero = input == 0;
            if (do_parity) Parity = CalculateParity((byte)(input & 0xFF));
            if (do_carry) Carry = (input > 0xFF) || (input < 0);
            if (do_auxcarry) AuxCarry = (auxin & 0xF) > (input & 0xF);
        }

        public void DoAccumulatorAnd(byte source, ref byte dest) {
            byte result = (byte)(dest & source);
            FlagSet(result, dest, do_carry: true);
            dest = (byte)(result & 0xFF);
        }

        public void DoRegisterAdd(byte source, ref byte dest, bool usecarry = false) {
            ushort result = (ushort)(source + dest + (usecarry ? (Carry ? 1 : 0) : 0));
            FlagSet(result, dest, do_carry: true);
            dest = (byte)(result & 0xFF);
        }

        public void DoRegisterSub(byte source, ref byte dest, bool usecarry = false) {
            ushort result = (ushort)(dest - source - (usecarry ? (Carry ? 1 : 0) : 0));
            FlagSet(result, dest, do_carry: true);
            dest = (byte)(result & 0xFF);
        }

        public void DoRegisterIncrement(byte regpointer) {
            byte result = (byte)(Registers[regpointer] + 1);
            FlagSet(result, Registers[regpointer]);
            Registers[regpointer] = result;
        }

        public void DoRegisterDecrement(byte regpointer) {
            byte result = (byte)(Registers[regpointer] - 1);
            FlagSet(result, Registers[regpointer]);
            Registers[regpointer] = result;
        }

        public void DoMemoryDecrement(ushort pointer) {
            byte result = (byte)(Memory[pointer] - 1);
            FlagSet(result, Memory[pointer]);
            Memory[pointer] = result;
        }

        public void DoMemoryIncrement(ushort pointer) {
            byte result = (byte)(Memory[pointer] + 1);
            FlagSet(result, Memory[pointer]);
            Memory[pointer] = result;
        }

        public void RotateAccumulatorLeft()
        {
            byte result = (byte)(((Registers[0x00] & 0x80) >> 7) | (Registers[0x00] << 1));
            Carry = (Registers[0x00] & 0x80) == 0x80;
            Registers[0x00] = result;
        }

        public void RotateAccumulatorRight()
        {
            byte result = (byte)(((Registers[0x00] & 0x01) << 7) | (Registers[0x00] >> 1));
            Carry = (Registers[0x00] & 0x01) == 1;
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
            Carry = (result > 0xFFFF);
            return (ushort)(result & 0xFFFF);
        }

        public void RegisterSwap(byte reg1, byte reg2)
        {
            var temp = Registers[reg1];
            Registers[reg1] = Registers[reg2];
            Registers[reg2] = temp;
        }

        public byte GetFlagByte() {
            var x = new BitArray(8);
            x.Set(7, Sign);
            x.Set(6, Zero);
            x.Set(5, false);
            x.Set(4, AuxCarry);
            x.Set(3, false);
            x.Set(2, Parity);
            x.Set(1, true);
            x.Set(0, Carry);
            var ret = new byte[1];
            x.CopyTo(ret, 0);
            return ret[0];
        }

        public void SetFlags(byte b) {
            var x = new BitArray(b);
            Sign = x.Length >= 8 ? x.Get(7) : false;
            Zero = x.Length >= 7 ? x.Get(6) : false;
            AuxCarry = x.Length >= 5 ? x.Get(4) : false;
            Parity = x.Length >= 3 ? x.Get(2) : false;
            Carry = x.Length >= 2 ? x.Get(1) : false;
        }

        public void Reset() {
            Registers = new byte[0x7];
            Memory = new byte[0x10000];
            StackPointer = 0xFFFE;
            ProgramCounter = 0x0;
            Sign = false;
            Zero = false;
            Parity = false;
            Carry = false;
            AuxCarry = false;
        }

        public CPUState() {
            Registers = new byte[0x7];
            Memory = new byte[0x10000];
            StackPointer = 0xFFFE;
            ProgramCounter = 0x0;
            Sign = false;
            Zero = false;
            Parity = false;
            Carry = false;
            AuxCarry = false;
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
            { 0x05, new CPUInstruction("DCR    B", () => { State.DoRegisterDecrement(0x01); }, 1) },
            { 0x06, new CPUInstruction("MVI    B,", () => { State.Registers[0x01] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x07, new CPUInstruction("RLC    ", () => { State.RotateAccumulatorLeft(); }, 1) },
            { 0x09, new CPUInstruction("DAD    B", () => {
                var result = State.DoDoubleAdd(State.RegisterToAddress(0x05, 0x06), State.RegisterToAddress(0x01, 0x02));
                State.Registers[0x05] = (byte)(result / 0x100);
                State.Registers[0x06] = (byte)(result & 0xFF);
            }, 1) },
            { 0x0D, new CPUInstruction("DCR    C", () => { State.DoRegisterIncrement(0x02); }, 1) },
            { 0x0E, new CPUInstruction("MVI    C,", () => { State.Registers[0x02] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x0F, new CPUInstruction("RRC    ", () => { State.RotateAccumulatorRight(); }, 1) },
            { 0x11, new CPUInstruction("LXI    D,", () => { Array.Copy(State.GetTwoBytes(State.ProgramCounter), 0x00, State.Registers, 0x03, 0x02); }, 3) },
            { 0x13, new CPUInstruction("INX    D", () => { Array.Copy(State.IncrementRegisterPair(0x03, 0x04), 0x00, State.Registers, 0x03, 0x02); }, 1) },
            { 0x19, new CPUInstruction("DAD    D", () => {
                var result = State.DoDoubleAdd(State.RegisterToAddress(0x05, 0x06), State.RegisterToAddress(0x03, 0x04));
                State.Registers[0x05] = (byte)(result / 0x100);
                State.Registers[0x06] = (byte)(result & 0xFF);
            }, 1) },
            { 0x1A, new CPUInstruction("LDAX   D", () => { State.Registers[0x00] = State.Memory[State.RegisterToAddress(0x03, 0x04)]; }, 1) },
            { 0x20, new CPUInstruction("RIM    ", () => { /* Coming eventually. */ }, 1) },
            { 0x21, new CPUInstruction("LXI    H,", () => { Array.Copy(State.GetTwoBytes(State.ProgramCounter), 0x00, State.Registers, 0x05, 0x02); }, 3) },
            { 0x23, new CPUInstruction("INX    H", () => { Array.Copy(State.IncrementRegisterPair(0x05, 0x06), 0x00, State.Registers, 0x05, 0x02); }, 1) },
            { 0x26, new CPUInstruction("MVI    H,", () => { State.Registers[0x05] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x29, new CPUInstruction("DAD    H", () => {
                var result = State.DoDoubleAdd(State.RegisterToAddress(0x05, 0x06), State.RegisterToAddress(0x05, 0x06));
                State.Registers[0x05] = (byte)(result / 0x100);
                State.Registers[0x06] = (byte)(result & 0xFF);
            }, 1) },
            { 0x2E, new CPUInstruction("MVI    L,", () => { State.Registers[0x06] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x31, new CPUInstruction("LXI    SP,", () => { State.StackPointer = State.GetTwoBytesJoined(State.ProgramCounter); }, 3) },
            { 0x32, new CPUInstruction("STA    ", () => { Array.Copy(State.Registers, 0x00, State.Memory, State.GetTwoBytesJoined(State.ProgramCounter), 0x01); }, 3) },
            { 0x35, new CPUInstruction("DCR    M", () => { State.DoMemoryDecrement(State.RegisterToAddress(0x05, 0x06)); }, 1) },
            { 0x36, new CPUInstruction("MVI    M,", () => { State.Memory[State.RegisterToAddress(0x05, 0x06)] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x3E, new CPUInstruction("MVI    A,", () => { State.Registers[0x00] = State.GetNextByte(State.ProgramCounter); }, 2) },
            { 0x3F, new CPUInstruction("CMC    ", () => { State.Carry = !State.Carry; }, 1) },
            { 0x40, new CPUInstruction("MOV    B,B", () => {  }, 1) },
            { 0x41, new CPUInstruction("MOV    B,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x01, 0x01); }, 1) },
            { 0x42, new CPUInstruction("MOV    B,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x01, 0x01); }, 1) },
            { 0x43, new CPUInstruction("MOV    B,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x01, 0x01); }, 1) },
            { 0x44, new CPUInstruction("MOV    B,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x01, 0x01); }, 1) },
            { 0x45, new CPUInstruction("MOV    B,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x01, 0x01); }, 1) },
            { 0x46, new CPUInstruction("MOV    B,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x01, 0x01); }, 1) },
            { 0x47, new CPUInstruction("MOV    B,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x01, 0x01); }, 1) },
            { 0x48, new CPUInstruction("MOV    C,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x02, 0x01); }, 1) },
            { 0x49, new CPUInstruction("MOV    C,C", () => {  }, 1) },
            { 0x4A, new CPUInstruction("MOV    C,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4B, new CPUInstruction("MOV    C,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4C, new CPUInstruction("MOV    C,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4D, new CPUInstruction("MOV    C,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x02, 0x01); }, 1) },
            { 0x4E, new CPUInstruction("MOV    C,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x02, 0x01); }, 1) },
            { 0x4F, new CPUInstruction("MOV    C,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x02, 0x01); }, 1) },
            { 0x50, new CPUInstruction("MOV    D,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x03, 0x01); }, 1) },
            { 0x51, new CPUInstruction("MOV    D,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x03, 0x01); }, 1) },
            { 0x52, new CPUInstruction("MOV    D,D", () => {  }, 1) },
            { 0x53, new CPUInstruction("MOV    D,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x03, 0x01); }, 1) },
            { 0x54, new CPUInstruction("MOV    D,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x03, 0x01); }, 1) },
            { 0x55, new CPUInstruction("MOV    D,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x03, 0x01); }, 1) },
            { 0x56, new CPUInstruction("MOV    D,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x03, 0x01); }, 1) },
            { 0x57, new CPUInstruction("MOV    D,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x04, 0x01); }, 1) },
            { 0x58, new CPUInstruction("MOV    E,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x04, 0x01); }, 1) },
            { 0x59, new CPUInstruction("MOV    E,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5A, new CPUInstruction("MOV    E,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5B, new CPUInstruction("MOV    E,E", () => {  }, 1) },
            { 0x5C, new CPUInstruction("MOV    E,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5D, new CPUInstruction("MOV    E,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x04, 0x01); }, 1) },
            { 0x5E, new CPUInstruction("MOV    E,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x04, 0x01); }, 1) },
            { 0x5F, new CPUInstruction("MOV    E,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x05, 0x01); }, 1) },
            { 0x60, new CPUInstruction("MOV    H,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x05, 0x01); }, 1) },
            { 0x61, new CPUInstruction("MOV    H,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x05, 0x01); }, 1) },
            { 0x62, new CPUInstruction("MOV    H,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x05, 0x01); }, 1) },
            { 0x63, new CPUInstruction("MOV    H,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x05, 0x01); }, 1) },
            { 0x64, new CPUInstruction("MOV    H,H", () => {  }, 1) },
            { 0x65, new CPUInstruction("MOV    H,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x05, 0x01); }, 1) },
            { 0x66, new CPUInstruction("MOV    H,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x05, 0x01); }, 1) },
            { 0x67, new CPUInstruction("MOV    H,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x06, 0x01); }, 1) },
            { 0x68, new CPUInstruction("MOV    L,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x06, 0x01); }, 1) },
            { 0x69, new CPUInstruction("MOV    L,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6A, new CPUInstruction("MOV    L,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6B, new CPUInstruction("MOV    L,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6C, new CPUInstruction("MOV    L,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x06, 0x01); }, 1) },
            { 0x6D, new CPUInstruction("MOV    L,L", () => {  }, 1) },
            { 0x6E, new CPUInstruction("MOV    L,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x06, 0x01); }, 1) },
            { 0x6F, new CPUInstruction("MOV    L,A", () => { Array.Copy(State.Registers, 0x00, State.Registers, 0x06, 0x01); }, 1) },
            { 0x70, new CPUInstruction("MOV    M,B", () => { Array.Copy(State.Registers, 0x01, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x71, new CPUInstruction("MOV    M,C", () => { Array.Copy(State.Registers, 0x02, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x72, new CPUInstruction("MOV    M,D", () => { Array.Copy(State.Registers, 0x03, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x73, new CPUInstruction("MOV    M,E", () => { Array.Copy(State.Registers, 0x04, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x74, new CPUInstruction("MOV    M,H", () => { Array.Copy(State.Registers, 0x05, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x75, new CPUInstruction("MOV    M,L", () => { Array.Copy(State.Registers, 0x06, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x76, new CPUInstruction("HLT    ", () => { /* Program should halt! */ }, 1) },
            { 0x77, new CPUInstruction("MOV    M,A", () => { Array.Copy(State.Registers, 0x00, State.Memory, State.RegisterToAddress(0x05, 0x06), 0x01); }, 1) },
            { 0x78, new CPUInstruction("MOV    A,B", () => { Array.Copy(State.Registers, 0x01, State.Registers, 0x00, 0x01); }, 1) },
            { 0x79, new CPUInstruction("MOV    A,C", () => { Array.Copy(State.Registers, 0x02, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7A, new CPUInstruction("MOV    A,D", () => { Array.Copy(State.Registers, 0x03, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7B, new CPUInstruction("MOV    A,E", () => { Array.Copy(State.Registers, 0x04, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7C, new CPUInstruction("MOV    A,H", () => { Array.Copy(State.Registers, 0x05, State.Registers, 0x00, 0x01); }, 1) },
            { 0x7D, new CPUInstruction("MOV    A,L", () => { Array.Copy(State.Registers, 0x06, State.Registers, 0x00, 0x01);}, 1) },
            { 0x7E, new CPUInstruction("MOV    A,M", () => { Array.Copy(State.Memory, State.RegisterToAddress(0x05, 0x06), State.Registers, 0x00, 0x01); }, 1) },
            { 0x7F, new CPUInstruction("MOV    A,A", () => {  }, 1) },
            { 0x80, new CPUInstruction("ADD    B", () => { State.DoRegisterAdd(State.Registers[0x01], ref State.Registers[0x00]); }, 1) },
            { 0x81, new CPUInstruction("ADD    C", () => { State.DoRegisterAdd(State.Registers[0x02], ref State.Registers[0x00]); }, 1) },
            { 0x82, new CPUInstruction("ADD    D", () => { State.DoRegisterAdd(State.Registers[0x03], ref State.Registers[0x00]); }, 1) },
            { 0x83, new CPUInstruction("ADD    E", () => { State.DoRegisterAdd(State.Registers[0x04], ref State.Registers[0x00]); }, 1) },
            { 0x84, new CPUInstruction("ADD    H", () => { State.DoRegisterAdd(State.Registers[0x05], ref State.Registers[0x00]); }, 1) },
            { 0x85, new CPUInstruction("ADD    L", () => { State.DoRegisterAdd(State.Registers[0x06], ref State.Registers[0x00]); }, 1) },
            { 0x86, new CPUInstruction("ADD    M", () => { State.DoRegisterAdd(State.Memory[State.RegisterToAddress(0x05, 0x06)], ref State.Registers[0x00]); }, 1) },
            { 0x87, new CPUInstruction("ADD    A", () => { State.DoRegisterAdd(State.Registers[0x00], ref State.Registers[0x00]); }, 1) },
            { 0x88, new CPUInstruction("ADC    B", () => { State.DoRegisterAdd(State.Registers[0x01], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x89, new CPUInstruction("ADC    C", () => { State.DoRegisterAdd(State.Registers[0x02], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x8A, new CPUInstruction("ADC    D", () => { State.DoRegisterAdd(State.Registers[0x03], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x8B, new CPUInstruction("ADC    E", () => { State.DoRegisterAdd(State.Registers[0x04], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x8C, new CPUInstruction("ADC    H", () => { State.DoRegisterAdd(State.Registers[0x05], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x8D, new CPUInstruction("ADC    L", () => { State.DoRegisterAdd(State.Registers[0x06], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x8E, new CPUInstruction("ADC    M", () => { State.DoRegisterAdd(State.Memory[State.RegisterToAddress(0x05, 0x06)], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x8F, new CPUInstruction("ADC    A", () => { State.DoRegisterAdd(State.Registers[0x00], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x90, new CPUInstruction("SUB    B", () => { State.DoRegisterSub(State.Registers[0x01], ref State.Registers[0x00]); }, 1) },
            { 0x91, new CPUInstruction("SUB    C", () => { State.DoRegisterSub(State.Registers[0x02], ref State.Registers[0x00]); }, 1) },
            { 0x92, new CPUInstruction("SUB    D", () => { State.DoRegisterSub(State.Registers[0x03], ref State.Registers[0x00]); }, 1) },
            { 0x93, new CPUInstruction("SUB    E", () => { State.DoRegisterSub(State.Registers[0x04], ref State.Registers[0x00]); }, 1) },
            { 0x94, new CPUInstruction("SUB    H", () => { State.DoRegisterSub(State.Registers[0x05], ref State.Registers[0x00]); }, 1) },
            { 0x95, new CPUInstruction("SUB    L", () => { State.DoRegisterSub(State.Registers[0x06], ref State.Registers[0x00]); }, 1) },
            { 0x96, new CPUInstruction("SUB    M", () => { State.DoRegisterSub(State.Memory[State.RegisterToAddress(0x05, 0x06)], ref State.Registers[0x00]); }, 1) },
            { 0x97, new CPUInstruction("SUB    A", () => { State.DoRegisterSub(State.Registers[0x00], ref State.Registers[0x00]); }, 1) },
            { 0x98, new CPUInstruction("SBB    B", () => { State.DoRegisterSub(State.Registers[0x01], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x99, new CPUInstruction("SBB    C", () => { State.DoRegisterSub(State.Registers[0x02], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x9A, new CPUInstruction("SBB    D", () => { State.DoRegisterSub(State.Registers[0x03], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x9B, new CPUInstruction("SBB    E", () => { State.DoRegisterSub(State.Registers[0x04], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x9C, new CPUInstruction("SBB    H", () => { State.DoRegisterSub(State.Registers[0x05], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x9D, new CPUInstruction("SBB    L", () => { State.DoRegisterSub(State.Registers[0x06], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x9E, new CPUInstruction("SBB    M", () => { State.DoRegisterSub(State.Memory[State.RegisterToAddress(0x05, 0x06)], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0x9F, new CPUInstruction("SBB    A", () => { State.DoRegisterSub(State.Registers[0x00], ref State.Registers[0x00], usecarry: true); }, 1) },
            { 0xA0, new CPUInstruction("ANA    B", () => { State.DoAccumulatorAnd(State.Registers[0x01], ref State.Registers[0x00]); }, 1) },
            { 0xA1, new CPUInstruction("ANA    C", () => { State.DoAccumulatorAnd(State.Registers[0x02], ref State.Registers[0x00]); }, 1) },
            { 0xA2, new CPUInstruction("ANA    D", () => { State.DoAccumulatorAnd(State.Registers[0x03], ref State.Registers[0x00]); }, 1) },
            { 0xA3, new CPUInstruction("ANA    E", () => { State.DoAccumulatorAnd(State.Registers[0x04], ref State.Registers[0x00]); }, 1) },
            { 0xA4, new CPUInstruction("ANA    H", () => { State.DoAccumulatorAnd(State.Registers[0x05], ref State.Registers[0x00]); }, 1) },
            { 0xA5, new CPUInstruction("ANA    L", () => { State.DoAccumulatorAnd(State.Registers[0x06], ref State.Registers[0x00]); }, 1) },
            { 0xA6, new CPUInstruction("ANA    M", () => { State.DoAccumulatorAnd(State.Memory[State.RegisterToAddress(0x05, 0x06)], ref State.Registers[0x00]); }, 1) },
            { 0xA7, new CPUInstruction("ANA    A", () => { State.DoAccumulatorAnd(State.Registers[0x00], ref State.Registers[0x00]); }, 1) },
            { 0xC1, new CPUInstruction("POP    B", () => {
                State.Registers[0x02] = State.Memory[State.StackPointer];
                State.Registers[0x01] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xC2, new CPUInstruction("JNZ    ", () => { State.ProgramCounter = State.Zero ? State.ProgramCounter : (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3); }, 3) },
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
            { 0xCD, new CPUInstruction("CALL   ", () => {
                State.StackPointer -= 0x02;
                Array.Copy(new byte[] { (byte)(State.ProgramCounter & 0xFF), (byte)(State.ProgramCounter / 0x100) }, 0x00, State.Memory, State.StackPointer, 0x02);
                State.ProgramCounter = (ushort)(State.GetTwoBytesJoined(State.ProgramCounter) - 3);
            }, 3) },
            { 0xD3, new CPUInstruction("OUT    ", () => { /* Does nothing for now. */ }, 2) },
            { 0xDB, new CPUInstruction("IN     ", () => { /* Does nothing for now. */ }, 2) },
            { 0xD1, new CPUInstruction("POP    D", () => {
                State.Registers[0x04] = State.Memory[State.StackPointer];
                State.Registers[0x03] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xD5, new CPUInstruction("PUSH   D", () => {
                State.StackPointer -= 0x02;
                State.Memory[State.StackPointer] = State.Registers[0x04];
                State.Memory[State.StackPointer + 1] = State.Registers[0x03];
            }, 1) },
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
                State.SetFlags(State.Memory[State.StackPointer]);
                State.Registers[0x00] = State.Memory[State.StackPointer + 1];
                State.StackPointer += 0x02;
            }, 1) },
            { 0xF3, new CPUInstruction("DI     ", () => { State.InterruptProcessingEnabled = false; }, 1) },
            { 0xF5, new CPUInstruction("PUSH   PSW", () => {
                State.StackPointer -= 0x02;
                State.Memory[State.StackPointer] = State.GetFlagByte();
                State.Memory[State.StackPointer + 1] = State.Registers[0x00];
            }, 1) },
            { 0xFB, new CPUInstruction("EI     ", () => { State.InterruptProcessingEnabled = false; }, 1) },
            { 0xFE, new CPUInstruction("CPI    ", () => {
                short result = (short)(State.Registers[0x00] - State.GetNextByte(State.ProgramCounter));
                State.FlagSet(result, State.Registers[0x00], do_carry: true);
            }, 2) }
        };

        public static Dictionary<byte, string> InstructionText = new Dictionary<byte, string>();

        public static bool Step(bool debug = false, bool printonly = false) {
            var inst = State.Memory[State.ProgramCounter];
            try {
                var instruct = Instructions[inst];
                if (debug) {
                    var outs = new StringBuilder();
                    outs.Append($"[{State.GetFlagByte().ToString("X2")} {string.Join(" ", State.Registers.Select(x => x.ToString("X2")))} {State.StackPointer.ToString("X4")} {State.Memory[State.StackPointer].ToString("X2")}]"); 
                    outs.Append($" 0x{State.ProgramCounter.ToString("X4")}: {instruct.InstructionString}");
                    if (instruct.Arity == 2) {
                        outs.Append($"${State.GetNextByte(State.ProgramCounter).ToString("X2")}");
                    } else if (instruct.Arity == 3) {
                        outs.Append($"${State.GetTwoBytesJoined(State.ProgramCounter).ToString("X4")}");
                    }
                    Console.WriteLine(outs.ToString());
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
