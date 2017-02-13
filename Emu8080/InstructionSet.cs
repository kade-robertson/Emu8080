using System.Collections.Generic;

// Thank God for http://altairclone.com/downloads/manuals/8080%20Programmers%20Manual.pdf

namespace Emu8080
{
    public static class InstructionSet {

        // NOP - No Operation
        // 0x00 (0x08, 0x10, 0x18, 0x20, 0x28, 0x30, 0x38)
        public static Instruction NOP = new Instruction() {
            Text = "NOP",
            Execute = (cpu, args) => { return true; },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "NOP";
            }
        };

        // LXI - Load Register Pair Immediate
        // 0x01, 0x11, 0x21, 0x31
        public static Instruction LXI = new Instruction() {
            Text = "",
            Execute = (cpu, args) => {
                switch ((args[0] >> 4) & 0x3) {
                    case 0:
                        cpu.Registers.B = args[2];
                        cpu.Registers.C = args[1];
                        break;
                    case 1:
                        cpu.Registers.D = args[2];
                        cpu.Registers.E = args[1];
                        break;
                    case 2:
                        cpu.Registers.H = args[2];
                        cpu.Registers.L = args[1];
                        break;
                    case 3:
                        cpu.Registers.SP = (ushort)((args[2] << 8) | args[1]);
                        break;
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"LXI    {Utils.RegisterPairFromBinary((byte)((args[0] >> 4) & 0x3), "SP")},#${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // STAX - Store Accumulator
        // 0x02, 0x12
        public static Instruction STAX = new Instruction() {
            Text = "STAX",
            Execute = (cpu, args) => {
                var oreg = 0;
                switch (args[0] & 0x10) {
                    case 0x00: oreg = cpu.Registers.BC; break;
                    case 0x10: oreg = cpu.Registers.DE; break;
                }
                cpu.Memory[oreg] = cpu.Registers.A;
                return true;
            },
            Arity = 1,
            Cycles = 7,
            GetPrintString = (args) => {
                var touse = (args[0] & 0x10) == 0x10 ? 'D' : 'B';
                return $"STAX   {touse}";
            }
        };

        // INX - Increment Register Pair
        // 0x03, 0x13, 0x23, 0x33
        public static Instruction INX = new Instruction() {
            Text = "INX",
            Execute = (cpu, args) => {
                switch ((args[0] >> 4) & 0xF) {
                    case 0: cpu.Registers.BC++; break;
                    case 1: cpu.Registers.DE++; break;
                    case 2: cpu.Registers.HL++; break;
                    case 3: cpu.Registers.SP = (ushort)((cpu.Registers.SP + 1) & 0xFFFF); break;
                }
                return true;
            },
            Cycles = 5,
            GetPrintString = (args) => {
                return $"INX    {Utils.RegisterPairFromBinary((byte)((args[0] >> 4) & 0xF), "SP")}";
            }
        };

        // MVI - Move Immediate Data
        // 0x06, 0x0E, 0x16, 0x1E, 0x26, 0x2E, 0x36, 0x3E
        public static Instruction MVI = new Instruction() {
            Text = "MVI",
            Execute = (cpu, args) => {
                switch ((args[0] >> 3) & 0x7) {
                    case 0: cpu.Registers.B = args[1]; break;
                    case 1: cpu.Registers.C = args[1]; break;
                    case 2: cpu.Registers.D = args[1]; break;
                    case 3: cpu.Registers.E = args[1]; break;
                    case 4: cpu.Registers.H = args[1]; break;
                    case 5: cpu.Registers.L = args[1]; break;
                    case 6: cpu.Memory[cpu.Registers.HL] = args[1]; return true;
                    case 7: cpu.Registers.A = args[1]; break;
                }
                return false;
            },
            Arity = 2,
            Cycles = 10,
            LowCycles = 7,
            GetPrintString = (args) => {
                return $"MVI    {Utils.RegisterFromBinary((byte)((args[0] >> 3) & 0x7))},#${args[1].ToString("X2")}";
            }
        };

        // RLC - Rotate Accumulator Left
        // 0x07
        public static Instruction RLC = new Instruction() {
            Text = "RLC",
            Execute = (cpu, args) => {
                var hob = (cpu.Registers.A >> 7);
                cpu.Flag.Carry = hob == 1;
                cpu.Registers.A = (byte)(((cpu.Registers.A << 1) & 0xFF) | hob);
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "RLC";
            }
        };

        // DAD - Double Add
        // 0x09, 0x19, 0x29, 0x39
        public static Instruction DAD = new Instruction() {
            Text = "DAD",
            Execute = (cpu, args) => {
                var result = (int)cpu.Registers.HL;
                var oreg = (ushort)0;
                switch ((args[0] >> 4) & 0xF) {
                    case 0: oreg = cpu.Registers.BC; break;
                    case 1: oreg = cpu.Registers.DE; break;
                    case 2: oreg = cpu.Registers.HL; break;
                    case 3: oreg = cpu.Registers.SP; break;
                }
                result = cpu.Registers.HL + oreg;
                cpu.Flag.Carry = (result > 0xFFFF);
                cpu.Registers.HL = (ushort)(result & 0xFFFF);
                return true;
            },
            Arity = 1,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"DAD    {Utils.RegisterPairFromBinary((byte)((args[0] >> 4) & 0xF), "SP")}";
            }
        };

        // LDAX - Load Accumulator
        // 0x0A, 0x1A
        public static Instruction LDAX = new Instruction() {
            Text = "LDAX",
            Execute = (cpu, args) => {
                var oreg = 0;
                switch (args[0] & 0x10) {
                    case 0x00: oreg = cpu.Registers.BC; break;
                    case 0x10: oreg = cpu.Registers.DE; break;
                }
                cpu.Registers.A = cpu.Memory[oreg];
                return true;
            },
            Arity = 1,
            Cycles = 7,
            GetPrintString = (args) => {
                var touse = (args[0] & 0x10) == 0x10 ? 'D' : 'B';
                return $"LDAX   {touse}";
            }
        };

        // DCX - Increment Register Pair
        // 0x0B, 0x1B, 0x2B, 0x3B
        public static Instruction DCX = new Instruction() {
            Text = "DCX",
            Execute = (cpu, args) => {
                switch ((args[0] >> 4) & 0xF) {
                    case 0: cpu.Registers.BC--; break;
                    case 1: cpu.Registers.DE--; break;
                    case 2: cpu.Registers.HL--; break;
                    case 3: cpu.Registers.SP = (ushort)((cpu.Registers.SP - 1) & 0xFFFF); break;
                }
                return true;
            },
            Cycles = 5,
            GetPrintString = (args) => {
                return $"DCX    {Utils.RegisterPairFromBinary((byte)((args[0] >> 4) & 0xF), "SP")}";
            }
        };

        // RRC - Rotate Accumulator Right
        // 0x0F
        public static Instruction RRC = new Instruction() {
            Text = "RRC",
            Execute = (cpu, args) => {
                var lob = (cpu.Registers.A & 0x1);
                cpu.Flag.Carry = lob == 1;
                cpu.Registers.A = (byte)((cpu.Registers.A >> 1) | (lob << 7));
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "RRC";
            }
        };

        // RAL - Rotate Accumulator Left Through Carry
        // 0x17
        public static Instruction RAL = new Instruction() {
            Text = "RAL",
            Execute = (cpu, args) => {
                var hob = (cpu.Registers.A >> 7);
                cpu.Registers.A = (byte)(((cpu.Registers.A << 1) & 0xFF) | (cpu.Flag.Carry ? 1 : 0));
                cpu.Flag.Carry = hob == 1;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "RAL";
            }
        };

        // RAR - Rotate Accumulator Right Through Carry
        // 0x1F
        public static Instruction RAR = new Instruction() {
            Text = "RAR",
            Execute = (cpu, args) => {
                var lob = (cpu.Registers.A & 0x1);
                cpu.Registers.A = (byte)((cpu.Registers.A >> 1) | (cpu.Flag.Carry ? 0x80 : 0));
                cpu.Flag.Carry = lob == 1;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "RAR";
            }
        };

        // SHLD - Store H and L Direct
        // 0x22
        public static Instruction SHLD = new Instruction() {
            Text = "SHLD",
            Execute = (cpu, args) => {
                var addr = (args[2] << 8) | args[1];
                cpu.Memory[addr] = cpu.Registers.L;
                cpu.Memory[addr + 1] = cpu.Registers.H;
                return true;
            },
            Arity = 3,
            Cycles = 16,
            GetPrintString = (args) => {
                return $"SHLD   ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // DAA - Decimal Adjust Accumulator
        // 0x27
        public static Instruction DAA = new Instruction() {
            Text = "DAA",
            Execute = (cpu, args) => {
                var low = cpu.Registers.A & 0xF;
                var result = cpu.Registers.A;
                if (low > 9 || cpu.Flag.AuxCarry) {
                    result += 6;
                    cpu.Flag.AuxCarry = true;
                }
                var high = result >> 4;
                if (high > 9 || cpu.Flag.Carry) {
                    result += 6 << 4;
                    cpu.Flag.Carry = true;
                }
                result = (byte)(result & 0xFF);
                cpu.Flag.Sign = (result & 0x80) != 0;
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Registers.A = result;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"DAA";
            }
        };

        // LHLD - Load H and L Direct
        // 0x2A
        public static Instruction LHLD = new Instruction() {
            Text = "LHLD",
            Execute = (cpu, args) => {
                var addr = (args[2] << 8) | args[1];
                cpu.Registers.L = cpu.Memory[addr];
                cpu.Registers.H = cpu.Memory[addr + 1];
                return true;
            },
            Arity = 3,
            Cycles = 16,
            GetPrintString = (args) => {
                return $"LHLD   ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // CMA - Complement Accumulator
        // 0x2F
        public static Instruction CMA = new Instruction() {
            Text = "CMA",
            Execute = (cpu, args) => {
                var result = (byte)(~cpu.Registers.A & 0xFF);
                cpu.Registers.A = result;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"CMA";
            }
        };

        // STA - Store Accumulator Direct
        // 0x32
        public static Instruction STA = new Instruction() {
            Text = "STA",
            Execute = (cpu, args) => {
                var addr = (args[2] << 8) | args[1];
                cpu.Memory[addr] = cpu.Registers.A;
                return true;
            },
            Arity = 3,
            Cycles = 13,
            GetPrintString = (args) => {
                return $"STA    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // STC - Set Carry
        // 0x37
        public static Instruction STC = new Instruction() {
            Text = "STC",
            Execute = (cpu, args) => { cpu.Flag.Carry = true; return true; },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"STC";
            }
        };

        // LDA - Load Accumulator Direct
        // 0x3A
        public static Instruction LDA = new Instruction() {
            Text = "LDA",
            Execute = (cpu, args) => {
                var addr = (args[2] << 8) | args[1];
                cpu.Registers.A = cpu.Memory[addr];
                return true;
            },
            Arity = 3,
            Cycles = 13,
            GetPrintString = (args) => {
                return $"LDA    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // CMC - Complement Carry
        // 0x3F
        public static Instruction CMC = new Instruction() {
            Text = "CMC",
            Execute = (cpu, args) => { cpu.Flag.Carry = !cpu.Flag.Carry; return true; },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"CMC";
            }
        };

        // INR - Increment Register or Memory
        // 0x04, 0x0C, 0x14, 0x1C, 0x24, 0x2C, 0x34, 0x3C
        public static Instruction INR = new Instruction() {
            Text = "INR",
            Execute = (cpu, args) => {
                byte toinc = 0;
                var retval = false;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: toinc = cpu.Registers.B; break;
                    case 1: toinc = cpu.Registers.C; break;
                    case 2: toinc = cpu.Registers.D; break;
                    case 3: toinc = cpu.Registers.E; break;
                    case 4: toinc = cpu.Registers.H; break;
                    case 5: toinc = cpu.Registers.L; break;
                    case 6: toinc = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: toinc = cpu.Registers.A; break;
                }
                toinc = (byte)((toinc + 1) & 0xFF);
                cpu.Flag.Sign = (toinc & 0x80) != 0;
                cpu.Flag.Zero = (toinc == 0);
                cpu.Flag.Parity = Utils.ParityTable[toinc] == 1;
                cpu.Flag.AuxCarry = (toinc & 0xF) != 0xF;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: cpu.Registers.B = toinc; break;
                    case 1: cpu.Registers.C = toinc; break;
                    case 2: cpu.Registers.D = toinc; break;
                    case 3: cpu.Registers.E = toinc; break;
                    case 4: cpu.Registers.H = toinc; break;
                    case 5: cpu.Registers.L = toinc; break;
                    case 6: cpu.Memory[cpu.Registers.HL] = toinc; break;
                    case 7: cpu.Registers.A = toinc; break;
                }
                return retval;
            },
            Arity = 1,
            Cycles = 10,
            LowCycles = 5,
            GetPrintString = (args) => {
                return $"INR    {Utils.RegisterFromBinary((byte)((args[0] & 0x3F) >> 3))}";
            }
        };

        // DCR - Decrement Register or Memory
        // 0x05, 0x0D, 0x15, 0x1D, 0x25, 0x2D, 0x35, 0x3D
        public static Instruction DCR = new Instruction() {
            Text = "DCR",
            Execute = (cpu, args) => {
                byte toinc = 0;
                var retval = false;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: toinc = cpu.Registers.B; break;
                    case 1: toinc = cpu.Registers.C; break;
                    case 2: toinc = cpu.Registers.D; break;
                    case 3: toinc = cpu.Registers.E; break;
                    case 4: toinc = cpu.Registers.H; break;
                    case 5: toinc = cpu.Registers.L; break;
                    case 6: toinc = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: toinc = cpu.Registers.A; break;
                }
                toinc = (byte)((toinc - 1) & 0xFF);
                cpu.Flag.Sign = (toinc & 0x80) != 0;
                cpu.Flag.Zero = (toinc == 0);
                cpu.Flag.Parity = Utils.ParityTable[toinc] == 1;
                cpu.Flag.AuxCarry = (toinc & 0xF) != 0xF;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: cpu.Registers.B = toinc; break;
                    case 1: cpu.Registers.C = toinc; break;
                    case 2: cpu.Registers.D = toinc; break;
                    case 3: cpu.Registers.E = toinc; break;
                    case 4: cpu.Registers.H = toinc; break;
                    case 5: cpu.Registers.L = toinc; break;
                    case 6: cpu.Memory[cpu.Registers.HL] = toinc; break;
                    case 7: cpu.Registers.A = toinc; break;
                }
                return retval;
            },
            Arity = 1,
            Cycles = 10,
            LowCycles = 5,
            GetPrintString = (args) => {
                return $"DCR    {Utils.RegisterFromBinary((byte)((args[0] & 0x3F) >> 3))}";
            }
        };

        // MOV - Move Instruction
        // 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47
        // 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F
        // 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57
        // 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F
        // 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67
        // 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F
        // 0x70, 0x71, 0x72, 0x73, 0x74, 0x75,     , 0x77
        // 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F
        public static Instruction MOV = new Instruction() {
            Text = "MOV",
            Execute = (cpu, args) => {
                byte tomov = 0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: tomov = cpu.Registers.B; break;
                    case 1: tomov = cpu.Registers.C; break;
                    case 2: tomov = cpu.Registers.D; break;
                    case 3: tomov = cpu.Registers.E; break;
                    case 4: tomov = cpu.Registers.H; break;
                    case 5: tomov = cpu.Registers.L; break;
                    case 6: tomov = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: tomov = cpu.Registers.A; break;
                }
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: cpu.Registers.B = tomov; break;
                    case 1: cpu.Registers.C = tomov; break;
                    case 2: cpu.Registers.D = tomov; break;
                    case 3: cpu.Registers.E = tomov; break;
                    case 4: cpu.Registers.H = tomov; break;
                    case 5: cpu.Registers.L = tomov; break;
                    case 6: cpu.Memory[cpu.Registers.HL] = tomov; retval = true; break;
                    case 7: cpu.Registers.A = tomov; break;
                }
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 5,
            GetPrintString = (args) => {
                return $"MOV    {Utils.RegisterFromBinary((byte)((args[0] & 0x3F) >> 3))},{Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // HLT - Halt
        // 0x76
        public static Instruction HLT = new Instruction() {
            Text = "HLT",
            Execute = (cpu, args) => {
                cpu.HasBeenHalted = true;
                return true;
            },
            Arity = 1,
            Cycles = 7,
            GetPrintString = (args) => {
                return "HLT";
            }
        };

        // ADD - Add Register or Memory To Accumulator
        // 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87
        public static Instruction ADD = new Instruction() {
            Text = "ADD",
            Execute = (cpu, args) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var result = cpu.Registers.A + oreg;
                cpu.Flag.Carry = (result > 0xFF);
                result = (byte)(result & 0xFF);
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (oreg & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Registers.A = (byte)result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"ADD    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // ADC - Add Register or Memory To Accumulator With Carry
        // 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F
        public static Instruction ADC = new Instruction() {
            Text = "ADC",
            Execute = (cpu, args) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var carryamt = cpu.Flag.Carry ? 1 : 0;
                var result = cpu.Registers.A + oreg + carryamt;
                cpu.Flag.Carry = (result > 0xFF);
                result = (byte)(result & 0xFF);
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (oreg & 0xF) + carryamt) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Registers.A = (byte)result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"ADC    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // SUB - Subtract Register or Memory To Accumulator
        // 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97
        public static Instruction SUB = new Instruction() {
            Text = "SUB",
            Execute = (cpu, args) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var twocomp = (~oreg + 1) & 0xFF;
                var result = cpu.Registers.A + ((~oreg + 1) & 0xFF);
                cpu.Flag.Carry = (result <= 0xFF);
                result = (byte)(result & 0xFF);
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (twocomp & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Registers.A = (byte)result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"SUB    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // SBB - Subtract Register or Memory To Accumulator With Carry
        // 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F
        public static Instruction SBB = new Instruction() {
            Text = "SBB",
            Execute = (cpu, args) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var carryamt = cpu.Flag.Carry ? 1 : 0;
                var twocomp = (~(oreg + carryamt) + 1) & 0xFF;
                var result = cpu.Registers.A + twocomp;
                cpu.Flag.Carry = (result <= 0xFF);
                result = (byte)(result & 0xFF);
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (twocomp & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Registers.A = (byte)result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"SBB    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // ANA - Logical AND Register or Memory With Accumulator
        // 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7
        public static Instruction ANA = new Instruction() {
            Text = "ANA",
            Execute = (cpu, args) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var result = (byte)(cpu.Registers.A & oreg);
                cpu.Flag.Carry = false;
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Registers.A = result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"ANA    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // XRA - Logical XOR Register or Memory With Accumulator
        // 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF
        public static Instruction XRA = new Instruction() {
            Text = "XRA",
            Execute = (cpu, args) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var result = (byte)(cpu.Registers.A ^ oreg);
                cpu.Flag.Carry = false;
                cpu.Flag.AuxCarry = false;
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Registers.A = result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"XRA    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // ORA - Logical OR Register or Memory With Accumulator
        // 0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7
        public static Instruction ORA = new Instruction() {
            Text = "ORA",
            Execute = (cpu, args) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                var result = (byte)(cpu.Registers.A | oreg);
                cpu.Flag.Carry = false;
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Registers.A = result;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"ORA    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // CMP - Compare Register or Memory With Accumulator
        // 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF
        public static Instruction CMP = new Instruction() {
            Text = "CMP",
            Execute = (cpu, args) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = cpu.Registers.B; break;
                    case 1: oreg = cpu.Registers.C; break;
                    case 2: oreg = cpu.Registers.D; break;
                    case 3: oreg = cpu.Registers.E; break;
                    case 4: oreg = cpu.Registers.H; break;
                    case 5: oreg = cpu.Registers.L; break;
                    case 6: oreg = cpu.Memory[cpu.Registers.HL]; retval = true; break;
                    case 7: oreg = cpu.Registers.A; break;
                }
                oreg = (byte)((~oreg + 1) & 0xFF);
                var result = cpu.Registers.A + oreg;
                cpu.Flag.Carry = !(result > 0xFF);
                cpu.Flag.Zero = (result == 0);
                cpu.Flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                cpu.Flag.Sign = ((result & 0xFF) >> 7) == 1;
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (oreg & 0xF)) > 0xF;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"CMP    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // RNZ - Return If Not Zero
        // 0xC0
        public static Instruction RNZ = new Instruction() {
            Text = "RNZ",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Zero) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RNZ";
            }
        };

        // POP - Pop Data Off Stack
        // 0xC1, 0xD1, 0xE1, 0xF1
        public static Instruction POP = new Instruction() {
            Text = "POP",
            Execute = (cpu, args) => {
                switch (((args[0] & 0x3F) >> 4)) {
                    case 0:
                        cpu.Registers.B = cpu.Memory[cpu.Registers.SP + 1];
                        cpu.Registers.C = cpu.Memory[cpu.Registers.SP];
                        break;
                    case 1:
                        cpu.Registers.D = cpu.Memory[cpu.Registers.SP + 1];
                        cpu.Registers.E = cpu.Memory[cpu.Registers.SP];
                        break;
                    case 2:
                        cpu.Registers.H = cpu.Memory[cpu.Registers.SP + 1];
                        cpu.Registers.L = cpu.Memory[cpu.Registers.SP];
                        break;
                    case 3:
                        cpu.Registers.A = cpu.Memory[cpu.Registers.SP + 1];
                        cpu.Flag.FlagByte = cpu.Memory[cpu.Registers.SP];
                        break;
                }
                cpu.Registers.SP += 2;
                return true;
            },
            Arity = 1,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"POP    {Utils.RegisterPairFromBinary((byte)((args[0] & 0x3F) >> 4))}";
            }
        };

        // JNZ - Jump If Not Zero
        // 0xC2
        public static Instruction JNZ = new Instruction() {
            Text = "JNZ",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Zero) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JNZ    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // JMP - Jump
        // 0xC3 (0xCB)
        public static Instruction JMP = new Instruction() {
            Text = "JMP",
            Execute = (cpu, args) => {
                cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JMP    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // CNZ - Call If Not Zero
        // 0xC4
        public static Instruction CNZ = new Instruction() {
            Text = "CNZ",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Zero) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CNZ    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // PUSH - Push Data Onto Stack
        // 0xC5, 0xD5, 0xE5, 0xF5
        public static Instruction PUSH = new Instruction() {
            Text = "PUSH",
            Execute = (cpu, args) => {
                switch (((args[0] & 0x3F) >> 4)) {
                    case 0:
                        cpu.Memory[cpu.Registers.SP - 1] = cpu.Registers.B;
                        cpu.Memory[cpu.Registers.SP - 2] = cpu.Registers.C;
                        break;
                    case 1:
                        cpu.Memory[cpu.Registers.SP - 1] = cpu.Registers.D;
                        cpu.Memory[cpu.Registers.SP - 2] = cpu.Registers.E;
                        break;
                    case 2:
                        cpu.Memory[cpu.Registers.SP - 1] = cpu.Registers.H;
                        cpu.Memory[cpu.Registers.SP - 2] = cpu.Registers.L;
                        break;
                    case 3:
                        cpu.Memory[cpu.Registers.SP - 1] = cpu.Registers.A;
                        cpu.Memory[cpu.Registers.SP - 2] = cpu.Flag.FlagByte;
                        break;
                }
                cpu.Registers.SP -= 2;
                return true;
            },
            Arity = 1,
            Cycles = 11,
            GetPrintString = (args) => {
                return $"PUSH   {Utils.RegisterPairFromBinary((byte)((args[0] & 0x3F) >> 4))}";
            }
        };

        // ADI - Add Immediate To Accumulator
        // 0xC6
        public static Instruction ADI = new Instruction() {
            Text = "ADI",
            Execute = (cpu, args) => {
                var result = cpu.Registers.A + args[1];
                cpu.Flag.Carry = (result > 0xFF);
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (args[1] & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                cpu.Flag.Sign = ((result & 0xFF) >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = (byte)(result & 0xFF);
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"ADI    #${args[1].ToString("X2")}";
            }
        };

        // RST - Restart
        // 0xC7, 0xCF, 0xD7, 0xDF, 0xE7, 0xEF, 0xF7, 0xFF
        public static Instruction RST = new Instruction() {
            Text = "RST",
            Execute = (cpu, args) => {
                cpu.StackPush(cpu.Registers.PC);
                cpu.Registers.PC = (ushort)((((args[0] >> 3) & 0x7) << 3));
                return true;
            },
            Arity = 1,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"RST    {(args[0] >> 3) & 0x7}";
            }
        };

        // RZ - Return If Zero
        // 0xC8
        public static Instruction RZ = new Instruction() {
            Text = "RZ",
            Execute = (cpu, args) => {
                if (cpu.Flag.Zero) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RZ";
            }
        };

        // RET - Return
        // 0xC9 (0xD9)
        public static Instruction RET = new Instruction() {
            Text = "RET",
            Execute = (cpu, args) => {
                cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                cpu.Registers.SP += 2;
                return true;
            },
            Arity = 1,
            Cycles = 10,
            GetPrintString = (args) => {
                return "RET";
            }
        };

        // JZ - Jump If Zero
        // 0xCA
        public static Instruction JZ = new Instruction() {
            Text = "JZ",
            Execute = (cpu, args) => {
                if (cpu.Flag.Zero) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JZ     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // CZ - Call If Zero
        // 0xCC
        public static Instruction CZ = new Instruction() {
            Text = "CZ",
            Execute = (cpu, args) => {
                if (cpu.Flag.Zero) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CZ     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // CALL - Call Subroutine
        // 0xCD (0xDD, 0xED, 0xFD)
        public static Instruction CALL = new Instruction() {
            Text = "CALL",
            Execute = (cpu, args) => {
                cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                cpu.Registers.SP -= 2;
                cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                return true;
            },
            Arity = 3,
            Cycles = 17,
            GetPrintString = (args) => {
                return $"CALL   ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // ACI - Add Immediate To Accumulator With Carry
        // 0xCE
        public static Instruction ACI = new Instruction() {
            Text = "ACI",
            Execute = (cpu, args) => {
                var carryb = cpu.Flag.Carry ? 1 : 0;
                var result = cpu.Registers.A + args[1] + carryb;
                cpu.Flag.Carry = (result > 0xFF);
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (args[1] & 0xF) + carryb) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                cpu.Flag.Sign = ((result & 0xFF) >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = (byte)(result & 0xFF);
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"ACI    #${args[1].ToString("X2")}";
            }
        };

        // RNC - Return If No Carry
        // 0xD0
        public static Instruction RNC = new Instruction() {
            Text = "RNC",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Carry) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RNC";
            }
        };

        // JNC - Jump If No Carry
        // 0xD2
        public static Instruction JNC = new Instruction() {
            Text = "JNC",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Carry) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JNC    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // OUT - Output
        // 0xD3
        public static Instruction OUT = new Instruction() {
            Text = "IN",
            Execute = (cpu, args) => {
                cpu.IO.SetOutput(args[1], cpu.Registers.A);
                cpu.Bus.DeliverOutput();
                return true;
            },
            Arity = 2,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"OUT    {args[1].ToString("X2")}";
            }
        };

        // CNC - Call If No Carry
        // 0xD4
        public static Instruction CNC = new Instruction() {
            Text = "CNC",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Carry) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CNC    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // SUI - Subtract Immediate From Accumulator
        // 0xD6
        public static Instruction SUI = new Instruction() {
            Text = "SUI",
            Execute = (cpu, args) => {
                var twocomp = ((~args[1] + 1) & 0xFF);
                var result = cpu.Registers.A + twocomp;
                cpu.Flag.Carry = (result <= 0xFF);
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (twocomp & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                cpu.Flag.Sign = ((result & 0xFF) >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = (byte)(result & 0xFF);
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"SUI    #${args[1].ToString("X2")}";
            }
        };

        // RC - Return If Carry
        // 0xD8
        public static Instruction RC = new Instruction() {
            Text = "RC",
            Execute = (cpu, args) => {
                if (cpu.Flag.Carry) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RC";
            }
        };

        // JC - Jump If Carry
        // 0xDA
        public static Instruction JC = new Instruction() {
            Text = "JC",
            Execute = (cpu, args) => {
                if (cpu.Flag.Carry) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JC     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // IN - Input
        // 0xDB
        public static Instruction IN = new Instruction() {
            Text = "IN",
            Execute = (cpu, args) => {
                cpu.Bus.RequestInput();
                cpu.Registers.A = cpu.IO.GetInput(args[1]);
                return true;
            },
            Arity = 2,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"IN     {args[1].ToString("X2")}";
            }
        };

        // CC - Call If Carry
        // 0xDC
        public static Instruction CC = new Instruction() {
            Text = "CC",
            Execute = (cpu, args) => {
                if (cpu.Flag.Carry) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CC     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // SBI - Subtract Immediate From Accumulator With Borrow
        // 0xDE
        public static Instruction SBI = new Instruction() {
            Text = "SBI",
            Execute = (cpu, args) => {
                var carryb = cpu.Flag.Carry ? 1 : 0;
                var twocomp = ((~(args[1] + carryb) + 1) & 0xFF);
                var result = cpu.Registers.A + twocomp;
                cpu.Flag.Carry = (result <= 0xFF);
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (twocomp & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                cpu.Flag.Sign = ((result & 0xFF) >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = (byte)(result & 0xFF);
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"SBI    #${args[1].ToString("X2")}";
            }
        };

        // RPO - Return If Parity Odd
        // 0xE0
        public static Instruction RPO = new Instruction() {
            Text = "RPO",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Parity) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RPO";
            }
        };

        // JPO - Jump If Parity Odd
        // 0xE2
        public static Instruction JPO = new Instruction() {
            Text = "JPO",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Parity) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JPO    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // XTHL - Exchange Stack
        // 0xE3
        public static Instruction XTHL = new Instruction() {
            Text = "XTHL",
            Execute = (cpu, args) => {
                byte temp1 = cpu.Memory[cpu.Registers.SP];
                byte temp2 = cpu.Memory[cpu.Registers.SP + 1];
                cpu.Memory[cpu.Registers.SP] = cpu.Registers.L;
                cpu.Memory[cpu.Registers.SP + 1] = cpu.Registers.H;
                cpu.Registers.L = temp1;
                cpu.Registers.H = temp2;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "XTHL";
            }
        };

        // CPO - Call If Parity Odd
        // 0xE4
        public static Instruction CPO = new Instruction() {
            Text = "CPO",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Parity) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CPO    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // ANI - And Immediate With Accumulator
        // 0xE6
        public static Instruction ANI = new Instruction() {
            Text = "ANI",
            Execute = (cpu, args) => {
                var result = (byte)(cpu.Registers.A & args[1]);
                cpu.Flag.Carry = false;
                cpu.Flag.AuxCarry = ((cpu.Registers.A | args[1]) & 0x08) != 0;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = result;
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"ANI    #${args[1].ToString("X2")}";
            }
        };

        // RPE - Return If Parity Even
        // 0xE8
        public static Instruction RPE = new Instruction() {
            Text = "RPE",
            Execute = (cpu, args) => {
                if (cpu.Flag.Parity) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RPE";
            }
        };

        // JPE - Jump If Parity Even
        // 0xEA
        public static Instruction JPE = new Instruction() {
            Text = "JPE",
            Execute = (cpu, args) => {
                if (cpu.Flag.Parity) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JPE    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // XCHG - Exchange Registers
        // 0xEB
        public static Instruction XCHG = new Instruction() {
            Text = "XCHG",
            Execute = (cpu, args) => {
                ushort temp = cpu.Registers.HL;
                cpu.Registers.HL = cpu.Registers.DE;
                cpu.Registers.DE = temp;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "XCHG";
            }
        };

        // CPE - Call If Parity Even
        // 0xEC
        public static Instruction CPE = new Instruction() {
            Text = "CPE",
            Execute = (cpu, args) => {
                if (cpu.Flag.Parity) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CPE    ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // XRI - Xor Immediate With Accumulator
        // 0xEE
        public static Instruction XRI = new Instruction() {
            Text = "XRI",
            Execute = (cpu, args) => {
                var result = (byte)(cpu.Registers.A ^ args[1]);
                cpu.Flag.Carry = false;
                cpu.Flag.AuxCarry = ((cpu.Registers.A | args[1]) & 0x08) != 0;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = result;
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"XRI    #${args[1].ToString("X2")}";
            }
        };

        // PCHL - Load Program Counter
        // 0xE9
        public static Instruction PCHL = new Instruction() {
            Text = "PCHL",
            Execute = (cpu, args) => {
                cpu.Registers.PC = cpu.Registers.HL;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "PCHL";
            }
        };

        // RP - Return If Positive
        // 0xF0
        public static Instruction RP = new Instruction() {
            Text = "RP",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Sign) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RP";
            }
        };

        // JP - Jump If Positive
        // 0xF2
        public static Instruction JP = new Instruction() {
            Text = "JP",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Sign) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JP     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // DI - Disable Interrupts
        // 0xF3
        public static Instruction DI = new Instruction() {
            Text = "DI",
            Execute = (cpu, args) => {
                cpu.Bus.Interrupt = false;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "DI";
            }
        };

        // CP - Call If Positive
        // 0xF4
        public static Instruction CP = new Instruction() {
            Text = "CP",
            Execute = (cpu, args) => {
                if (!cpu.Flag.Sign) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CP     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // ORI - Or Immediate With Accumulator
        // 0xF6
        public static Instruction ORI = new Instruction() {
            Text = "ORI",
            Execute = (cpu, args) => {
                var result = (byte)(cpu.Registers.A | args[1]);
                cpu.Flag.Carry = false;
                cpu.Flag.AuxCarry = (result & 0x08) != 0;
                cpu.Flag.Parity = Utils.ParityTable[result] == 1;
                cpu.Flag.Sign = (result >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                cpu.Registers.A = result;
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"ORI    #${args[1].ToString("X2")}";
            }
        };

        // RM - Return If Minus
        // 0xF8
        public static Instruction RM = new Instruction() {
            Text = "RM",
            Execute = (cpu, args) => {
                if (cpu.Flag.Sign) {
                    cpu.Registers.PC = (ushort)((cpu.Memory[cpu.Registers.SP + 1] << 8) | cpu.Memory[cpu.Registers.SP]);
                    cpu.Registers.SP += 2;
                    return true;
                }
                return false;
            },
            Arity = 1,
            Cycles = 11,
            LowCycles = 5,
            GetPrintString = (args) => {
                return "RM";
            }
        };

        // SPHL - Load SP From H And L
        // 0xF9
        public static Instruction SPHL = new Instruction() {
            Text = "SPHL",
            Execute = (cpu, args) => {
                cpu.Registers.SP = cpu.Registers.HL;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "SPHL";
            }
        };

        // JM - Jump If Minus
        // 0xFA
        public static Instruction JM = new Instruction() {
            Text = "JM",
            Execute = (cpu, args) => {
                if (cpu.Flag.Sign) {
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                }
                return true;
            },
            Arity = 3,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"JM     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // EI - Enable Interrupts
        // 0xFB
        public static Instruction EI = new Instruction() {
            Text = "EI",
            Execute = (cpu, args) => {
                cpu.Bus.Interrupt = true;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "EI";
            }
        };

        // CM - Call If Minus
        // 0xFC
        public static Instruction CM = new Instruction() {
            Text = "CM",
            Execute = (cpu, args) => {
                if (cpu.Flag.Sign) {
                    cpu.Memory[cpu.Registers.SP - 1] = (byte)(cpu.Registers.PC >> 8);
                    cpu.Memory[cpu.Registers.SP - 2] = (byte)(cpu.Registers.PC & 0xFF);
                    cpu.Registers.SP -= 2;
                    cpu.Registers.PC = (ushort)((args[2] << 8) | args[1]);
                    return true;
                }
                return false;
            },
            Arity = 3,
            Cycles = 17,
            LowCycles = 11,
            GetPrintString = (args) => {
                return $"CM     ${args[2].ToString("X2")}{args[1].ToString("X2")}";
            }
        };

        // CPI - Compare Immediate With Accumulator
        // 0xFE
        public static Instruction CPI = new Instruction() {
            Text = "CPI",
            Execute = (cpu, args) => {
                var twocomp = (byte)((~args[1] + 1) & 0xFF);
                var result = cpu.Registers.A + twocomp;
                cpu.Flag.Carry = !(result > 0xFF);
                cpu.Flag.AuxCarry = ((cpu.Registers.A & 0xF) + (twocomp & 0xF)) > 0xF;
                cpu.Flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                cpu.Flag.Sign = ((result & 0xFF) >> 7) == 1;
                cpu.Flag.Zero = (result == 0);
                return true;
            },
            Arity = 2,
            Cycles = 7,
            GetPrintString = (args) => {
                return $"CPI    #${args[1].ToString("X2")}";
            }
        };

        public static Dictionary<byte, Instruction> Instructions = new Dictionary<byte, Instruction>() {
            { 0x00, NOP  }, { 0x01, LXI  }, { 0x02, STAX }, { 0x03, INX  }, { 0x04, INR  }, { 0x05, DCR  }, { 0x06, MVI  }, { 0x07, RLC  },
            { 0x08, NOP  }, { 0x09, DAD  }, { 0x0A, LDAX }, { 0x0B, DCX  }, { 0x0C, INR  }, { 0x0D, DCR  }, { 0x0E, MVI  }, { 0x0F, RRC  },
            { 0x10, NOP  }, { 0x11, LXI  }, { 0x12, STAX }, { 0x13, INX  }, { 0x14, INR  }, { 0x15, DCR  }, { 0x16, MVI  }, { 0x17, RAL  },
            { 0x18, NOP  }, { 0x19, DAD  }, { 0x1A, LDAX }, { 0x1B, DCX  }, { 0x1C, INR  }, { 0x1D, DCR  }, { 0x1E, MVI  }, { 0x1F, RAR  },
            { 0x20, NOP  }, { 0x21, LXI  }, { 0x22, SHLD }, { 0x23, INX  }, { 0x24, INR  }, { 0x25, DCR  }, { 0x26, MVI  }, { 0x27, DAA  },
            { 0x28, NOP  }, { 0x29, DAD  }, { 0x2A, LHLD }, { 0x2B, DCX  }, { 0x2C, INR  }, { 0x2D, DCR  }, { 0x2E, MVI  }, { 0x2F, CMA  },
            { 0x30, NOP  }, { 0x31, LXI  }, { 0x32, STA  }, { 0x33, INX  }, { 0x34, INR  }, { 0x35, DCR  }, { 0x36, MVI  }, { 0x37, STC  },
            { 0x38, NOP  }, { 0x39, DAD  }, { 0x3A, LDA  }, { 0x3B, DCX  }, { 0x3C, INR  }, { 0x3D, DCR  }, { 0x3E, MVI  }, { 0x3F, CMC  },
            { 0x40, MOV  }, { 0x41, MOV  }, { 0x42, MOV  }, { 0x43, MOV  }, { 0x44, MOV  }, { 0x45, MOV  }, { 0x46, MOV  }, { 0x47, MOV  },
            { 0x48, MOV  }, { 0x49, MOV  }, { 0x4A, MOV  }, { 0x4B, MOV  }, { 0x4C, MOV  }, { 0x4D, MOV  }, { 0x4E, MOV  }, { 0x4F, MOV  },
            { 0x50, MOV  }, { 0x51, MOV  }, { 0x52, MOV  }, { 0x53, MOV  }, { 0x54, MOV  }, { 0x55, MOV  }, { 0x56, MOV  }, { 0x57, MOV  },
            { 0x58, MOV  }, { 0x59, MOV  }, { 0x5A, MOV  }, { 0x5B, MOV  }, { 0x5C, MOV  }, { 0x5D, MOV  }, { 0x5E, MOV  }, { 0x5F, MOV  },
            { 0x60, MOV  }, { 0x61, MOV  }, { 0x62, MOV  }, { 0x63, MOV  }, { 0x64, MOV  }, { 0x65, MOV  }, { 0x66, MOV  }, { 0x67, MOV  },
            { 0x68, MOV  }, { 0x69, MOV  }, { 0x6A, MOV  }, { 0x6B, MOV  }, { 0x6C, MOV  }, { 0x6D, MOV  }, { 0x6E, MOV  }, { 0x6F, MOV  },
            { 0x70, MOV  }, { 0x71, MOV  }, { 0x72, MOV  }, { 0x73, MOV  }, { 0x74, MOV  }, { 0x75, MOV  }, { 0x76, HLT  }, { 0x77, MOV  },
            { 0x78, MOV  }, { 0x79, MOV  }, { 0x7A, MOV  }, { 0x7B, MOV  }, { 0x7C, MOV  }, { 0x7D, MOV  }, { 0x7E, MOV  }, { 0x7F, MOV  },
            { 0x80, ADD  }, { 0x81, ADD  }, { 0x82, ADD  }, { 0x83, ADD  }, { 0x84, ADD  }, { 0x85, ADD  }, { 0x86, ADD  }, { 0x87, ADD  },
            { 0x88, ADC  }, { 0x89, ADC  }, { 0x8A, ADC  }, { 0x8B, ADC  }, { 0x8C, ADC  }, { 0x8D, ADC  }, { 0x8E, ADC  }, { 0x8F, ADC  },
            { 0x90, SUB  }, { 0x91, SUB  }, { 0x92, SUB  }, { 0x93, SUB  }, { 0x94, SUB  }, { 0x95, SUB  }, { 0x96, SUB  }, { 0x97, SUB  },
            { 0x98, SBB  }, { 0x99, SBB  }, { 0x9A, SBB  }, { 0x9B, SBB  }, { 0x9C, SBB  }, { 0x9D, SBB  }, { 0x9E, SBB  }, { 0x9F, SBB  },
            { 0xA0, ANA  }, { 0xA1, ANA  }, { 0xA2, ANA  }, { 0xA3, ANA  }, { 0xA4, ANA  }, { 0xA5, ANA  }, { 0xA6, ANA  }, { 0xA7, ANA  },
            { 0xA8, XRA  }, { 0xA9, XRA  }, { 0xAA, XRA  }, { 0xAB, XRA  }, { 0xAC, XRA  }, { 0xAD, XRA  }, { 0xAE, XRA  }, { 0xAF, XRA  },
            { 0xB0, ORA  }, { 0xB1, ORA  }, { 0xB2, ORA  }, { 0xB3, ORA  }, { 0xB4, ORA  }, { 0xB5, ORA  }, { 0xB6, ORA  }, { 0xB7, ORA  },
            { 0xB8, CMP  }, { 0xB9, CMP  }, { 0xBA, CMP  }, { 0xBB, CMP  }, { 0xBC, CMP  }, { 0xBD, CMP  }, { 0xBE, CMP  }, { 0xBF, CMP  },
            { 0xC0, RNZ  }, { 0xC1, POP  }, { 0xC2, JNZ  }, { 0xC3, JMP  }, { 0xC4, CNZ  }, { 0xC5, PUSH }, { 0xC6, ADI  }, { 0xC7, RST  },
            { 0xC8, RZ   }, { 0xC9, RET  }, { 0xCA, JZ   }, { 0xCB, JMP  }, { 0xCC, CZ   }, { 0xCD, CALL }, { 0xCE, ACI  }, { 0xCF, RST  },
            { 0xD0, RNC  }, { 0xD1, POP  }, { 0xD2, JNC  }, { 0xD3, OUT  }, { 0xD4, CNC  }, { 0xD5, PUSH }, { 0xD6, SUI  }, { 0xD7, RST  },
            { 0xD8, RC   }, { 0xD9, RET  }, { 0xDA, JC   }, { 0xDB, IN   }, { 0xDC, CC   }, { 0xDD, CALL }, { 0xDE, SBI  }, { 0xDF, RST  },
            { 0xE0, RPO  }, { 0xE1, POP  }, { 0xE2, JPO  }, { 0xE3, XTHL }, { 0xE4, CPO  }, { 0xE5, PUSH }, { 0xE6, ANI  }, { 0xE7, RST  },
            { 0xE8, RPE  }, { 0xE9, PCHL }, { 0xEA, JPE  }, { 0xEB, XCHG }, { 0xEC, CPE  }, { 0xED, CALL }, { 0xEE, XRI  }, { 0xEF, RST  },
            { 0xF0, RP   }, { 0xF1, POP  }, { 0xF2, JP   }, { 0xF3, DI   }, { 0xF4, CP   }, { 0xF5, PUSH }, { 0xF6, ORI  }, { 0xF7, RST  },
            { 0xF8, RM   }, { 0xF9, SPHL }, { 0xFA, JM   }, { 0xFB, EI   }, { 0xFC, CM   }, { 0xFD, CALL }, { 0xFE, CPI  }, { 0xFF, RST  },
        };
    }
}