﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Thank God for http://altairclone.com/downloads/manuals/8080%20Programmers%20Manual.pdf

namespace Emu8080
{
    public static class InstructionSet {

        // NOP - No Operation
        // 0x00
        public static Instruction NOP = new Instruction() {
            Text = "NOP",
            Execute = (mem, args, reg, flag) => { return true; },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "NOP";
            }
        };

        // STAX - Store Accumulator
        // 0x02, 0x12
        public static Instruction STAX = new Instruction() {
            Text = "STAX",
            Execute = (mem, args, reg, flag) => {
                var oreg = 0;
                switch (args[0] & 0x10) {
                    case 0x00: oreg = reg.BC; break;
                    case 0x10: oreg = reg.DE; break;
                }
                mem[oreg] = reg.A;
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
            Execute = (mem, args, reg, flag) => {
                switch ((args[0] >> 4) & 0xF) {
                    case 0: reg.BC++; break;
                    case 1: reg.DE++; break;
                    case 2: reg.HL++; break;
                    case 3: reg.SP = (ushort)((reg.SP + 1) & 0xFFFF); break;
                }
                return true;
            },
            Cycles = 5,
            GetPrintString = (args) => {
                return $"INX    {Utils.RegisterPairFromBinary((byte)((args[0] >> 4) & 0xF), "SP")}";
            }
        };

        // RLC - Rotate Accumulator Left
        // 0x07
        public static Instruction RLC = new Instruction() {
            Text = "RLC",
            Execute = (mem, args, reg, flag) => {
                var hob = (reg.A >> 7);
                flag.Carry = hob == 1;
                reg.A = (byte)(((reg.A << 1) & 0xFF) | hob);
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
            Execute = (mem, args, reg, flag) => {
                var result = (int)reg.HL;
                var oreg = (ushort)0;
                switch ((args[0] >> 4) & 0xF) {
                    case 0: oreg = reg.BC; break;
                    case 1: oreg = reg.DE; break;
                    case 2: oreg = reg.HL; break;
                    case 3: oreg = reg.SP; break;
                }
                result = reg.HL + oreg;
                flag.Carry = (result > 0xFFFF);
                reg.HL = (ushort)(result & 0xFFFF);
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
            Execute = (mem, args, reg, flag) => {
                var oreg = 0;
                switch (args[0] & 0x10) {
                    case 0x00: oreg = reg.BC; break;
                    case 0x10: oreg = reg.DE; break;
                }
                reg.A = mem[oreg];
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
            Execute = (mem, args, reg, flag) => {
                switch ((args[0] >> 4) & 0xF) {
                    case 0: reg.BC--; break;
                    case 1: reg.DE--; break;
                    case 2: reg.HL--; break;
                    case 3: reg.SP = (ushort)((reg.SP - 1) & 0xFFFF); break;
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
            Execute = (mem, args, reg, flag) => {
                var lob = (reg.A & 0x1);
                flag.Carry = lob == 1;
                reg.A = (byte)((reg.A >> 1) | (lob << 7));
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
            Execute = (mem, args, reg, flag) => {
                var hob = (reg.A >> 7);
                reg.A = (byte)(((reg.A << 1) & 0xFF) | (flag.Carry ? 1 : 0));
                flag.Carry = hob == 1;
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
            Execute = (mem, args, reg, flag) => {
                var lob = (reg.A & 0x1);
                reg.A = (byte)((reg.A >> 1) | (flag.Carry ? 0x80 : 0));
                flag.Carry = lob == 1;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return "RAR";
            }
        };

        // DAA - Decimal Adjust Accumulator
        // 0x27
        public static Instruction DAA = new Instruction() {
            Text = "DAA",
            Execute = (mem, args, reg, flag) => {
                var low = reg.A & 0xF;
                var result = reg.A;
                if (low > 9 || flag.AuxCarry) {
                    result += 6;
                    flag.AuxCarry = true;
                }
                var high = result >> 4;
                if (high > 9 || flag.Carry) {
                    result += 6 << 4;
                    flag.Carry = true;
                }
                result = (byte)(result & 0xFF);
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                reg.A = result;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"DAA";
            }
        };

        // CMA - Complement Accumulator
        // 0x2F
        public static Instruction CMA = new Instruction() {
            Text = "CMA",
            Execute = (mem, args, reg, flag) => {
                var result = (byte)(~reg.A & 0xFF);
                reg.A = result;
                return true;
            },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"CMA";
            }
        };

        // STC - Set Carry
        // 0x37
        public static Instruction STC = new Instruction() {
            Text = "STC",
            Execute = (mem, args, reg, flag) => { flag.Carry = true; return true; },
            Arity = 1,
            Cycles = 4,
            GetPrintString = (args) => {
                return $"STC";
            }
        };

        // CMC - Complement Carry
        // 0x3F
        public static Instruction CMC = new Instruction() {
            Text = "CMC",
            Execute = (mem, args, reg, flag) => { flag.Carry = !flag.Carry; return true; },
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
            Execute = (mem, args, reg, flag) => {
                byte toinc = 0;
                var retval = false;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: toinc = reg.B; break;
                    case 1: toinc = reg.C; break;
                    case 2: toinc = reg.D; break;
                    case 3: toinc = reg.E; break;
                    case 4: toinc = reg.H; break;
                    case 5: toinc = reg.L; break;
                    case 6: toinc = mem[reg.HL]; retval = true; break;
                    case 7: toinc = reg.A; break;
                }
                toinc = (byte)((toinc + 1) & 0xFF);
                flag.Sign = (toinc & 0x80) != 0;
                flag.Zero = (toinc == 0);
                flag.Parity = Utils.ParityTable[toinc] == 1;
                flag.AuxCarry = (toinc & 0xF) != 0xF;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: reg.B = toinc; break;
                    case 1: reg.C = toinc; break;
                    case 2: reg.D = toinc; break;
                    case 3: reg.E = toinc; break;
                    case 4: reg.H = toinc; break;
                    case 5: reg.L = toinc; break;
                    case 6: mem[reg.HL] = toinc; break;
                    case 7: reg.A = toinc; break;
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
            Execute = (mem, args, reg, flag) => {
                byte toinc = 0;
                var retval = false;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: toinc = reg.B; break;
                    case 1: toinc = reg.C; break;
                    case 2: toinc = reg.D; break;
                    case 3: toinc = reg.E; break;
                    case 4: toinc = reg.H; break;
                    case 5: toinc = reg.L; break;
                    case 6: toinc = mem[reg.HL]; retval = true; break;
                    case 7: toinc = reg.A; break;
                }
                toinc = (byte)((toinc - 1) & 0xFF);
                flag.Sign = (toinc & 0x80) != 0;
                flag.Zero = (toinc == 0);
                flag.Parity = Utils.ParityTable[toinc] == 1;
                flag.AuxCarry = (toinc & 0xF) != 0xF;
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: reg.B = toinc; break;
                    case 1: reg.C = toinc; break;
                    case 2: reg.D = toinc; break;
                    case 3: reg.E = toinc; break;
                    case 4: reg.H = toinc; break;
                    case 5: reg.L = toinc; break;
                    case 6: mem[reg.HL] = toinc; break;
                    case 7: reg.A = toinc; break;
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
            Execute = (mem, args, reg, flag) => {
                byte tomov = 0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: tomov = reg.B; break;
                    case 1: tomov = reg.C; break;
                    case 2: tomov = reg.D; break;
                    case 3: tomov = reg.E; break;
                    case 4: tomov = reg.H; break;
                    case 5: tomov = reg.L; break;
                    case 6: tomov = mem[reg.HL]; retval = true; break;
                    case 7: tomov = reg.A; break;
                }
                switch ((args[0] & 0x3F) >> 3) {
                    case 0: reg.B = tomov; break;
                    case 1: reg.C = tomov; break;
                    case 2: reg.D = tomov; break;
                    case 3: reg.E = tomov; break;
                    case 4: reg.H = tomov; break;
                    case 5: reg.L = tomov; break;
                    case 6: mem[reg.HL] = tomov; retval = true; break;
                    case 7: reg.A = tomov; break;
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

        // ADD - Add Register or Memory To Accumulator
        // 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87
        public static Instruction ADD = new Instruction() {
            Text = "ADD",
            Execute = (mem, args, reg, flag) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var result = reg.A + oreg;
                flag.Carry = (result > 0xFF);
                result = (byte)(result & 0xFF);
                flag.Zero = (result == 0);
                flag.Sign = (result >> 7) == 1;
                flag.AuxCarry = ((reg.A & 0xF) + (oreg & 0xF)) > 0xF;
                flag.Parity = Utils.ParityTable[result] == 1;
                reg.A = (byte)result;
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
            Execute = (mem, args, reg, flag) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var carryamt = flag.Carry ? 1 : 0;
                var result = reg.A + oreg + carryamt;
                flag.Carry = (result > 0xFF);
                result = (byte)(result & 0xFF);
                flag.Zero = (result == 0);
                flag.Sign = (result >> 7) == 1;
                flag.AuxCarry = ((reg.A & 0xF) + (oreg & 0xF) + carryamt) > 0xF;
                flag.Parity = Utils.ParityTable[result] == 1;
                reg.A = (byte)result;
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
            Execute = (mem, args, reg, flag) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var twocomp = (~oreg + 1) & 0xFF;
                var result = reg.A + ((~oreg + 1) & 0xFF);
                flag.Carry = (result <= 0xFF);
                result = (byte)(result & 0xFF);
                flag.Zero = (result == 0);
                flag.Sign = (result >> 7) == 1;
                flag.AuxCarry = ((reg.A & 0xF) + (twocomp & 0xF)) > 0xF;
                flag.Parity = Utils.ParityTable[result] == 1;
                reg.A = (byte)result;
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
            Execute = (mem, args, reg, flag) => {
                var retval = false;
                var oreg = (byte)0;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var carryamt = flag.Carry ? 1 : 0;
                var twocomp = (~(oreg + carryamt) + 1) & 0xFF;
                var result = reg.A + twocomp;
                flag.Carry = (result <= 0xFF);
                result = (byte)(result & 0xFF);
                flag.Zero = (result == 0);
                flag.Sign = (result >> 7) == 1;
                flag.AuxCarry = ((reg.A & 0xF) + (twocomp & 0xF)) > 0xF;
                flag.Parity = Utils.ParityTable[result] == 1;
                reg.A = (byte)result;
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
            Execute = (mem, args, reg, flag) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var result = (byte)(reg.A & oreg);
                flag.Carry = false;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.Sign = (result >> 7) == 1;
                reg.A = result;
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
            Execute = (mem, args, reg, flag) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var result = (byte)(reg.A ^ oreg);
                flag.Carry = false;
                flag.AuxCarry = false;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.Sign = (result >> 7) == 1;
                reg.A = result;
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
            Execute = (mem, args, reg, flag) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                var result = (byte)(reg.A | oreg);
                flag.Carry = false;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.Sign = (result >> 7) == 1;
                reg.A = result;
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
            Execute = (mem, args, reg, flag) => {
                var oreg = (byte)0;
                var retval = false;
                switch (args[0] & 0x7) {
                    case 0: oreg = reg.B; break;
                    case 1: oreg = reg.C; break;
                    case 2: oreg = reg.D; break;
                    case 3: oreg = reg.E; break;
                    case 4: oreg = reg.H; break;
                    case 5: oreg = reg.L; break;
                    case 6: oreg = mem[reg.HL]; retval = true; break;
                    case 7: oreg = reg.A; break;
                }
                oreg = (byte)((~oreg + 1) & 0xFF);
                var result = reg.A + oreg;
                flag.Carry = !(result > 0xFF);
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.Sign = ((result & 0xFF) >> 7) == 1;
                flag.AuxCarry = ((reg.A & 0xF) + (oreg & 0xF)) > 0xF;
                return retval;
            },
            Arity = 1,
            Cycles = 7,
            LowCycles = 4,
            GetPrintString = (args) => {
                return $"CMP    {Utils.RegisterFromBinary((byte)(args[0] & 0x7))}";
            }
        };

        // PUSH - Push Data Onto Stack
        // 0xC5, 0xD5, 0xE5, 0xF5
        public static Instruction PUSH = new Instruction() {
            Text = "PUSH",
            Execute = (mem, args, reg, flag) => {
                switch (((args[0] & 0x3F) >> 4)) {
                    case 0:
                        mem[reg.SP - 1] = reg.B;
                        mem[reg.SP - 2] = reg.C;
                        break;
                    case 1:
                        mem[reg.SP - 1] = reg.D;
                        mem[reg.SP - 2] = reg.E;
                        break;
                    case 2:
                        mem[reg.SP - 1] = reg.H;
                        mem[reg.SP - 2] = reg.L;
                        break;
                    case 3:
                        mem[reg.SP - 1] = reg.A;
                        mem[reg.SP - 2] = flag.FlagByte;
                        break;
                }
                reg.SP -= 2;
                return true;
            },
            Arity = 1,
            Cycles = 11,
            GetPrintString = (args) => {
                return $"PUSH   {Utils.RegisterPairFromBinary((byte)((args[0] & 0x3F) >> 4))}";
            }
        };

        // POP - Pop Data Off Stack
        // 0xC1, 0xD1, 0xE1, 0xF1
        public static Instruction POP = new Instruction() {
            Text = "POP",
            Execute = (mem, args, reg, flag) => {
                switch (((args[0] & 0x3F) >> 4)) {
                    case 0:
                        reg.B = mem[reg.SP + 1];
                        reg.C = mem[reg.SP];
                        break;
                    case 1:
                        reg.D = mem[reg.SP + 1];
                        reg.E = mem[reg.SP];
                        break;
                    case 2:
                        reg.H = mem[reg.SP + 1];
                        reg.L = mem[reg.SP];
                        break;
                    case 3:
                        reg.A = mem[reg.SP + 1];
                        flag.FlagByte = mem[reg.SP];
                        break;
                }
                reg.SP += 2;
                return true;
            },
            Arity = 1,
            Cycles = 10,
            GetPrintString = (args) => {
                return $"POP    {Utils.RegisterPairFromBinary((byte)((args[0] & 0x3F) >> 4))}";
            }
        };

        // XTHL - Exchange Stack
        // 0xE3
        public static Instruction XTHL = new Instruction() {
            Text = "XTHL",
            Execute = (mem, args, reg, flag) => {
                byte temp1 = mem[reg.SP];
                byte temp2 = mem[reg.SP + 1];
                mem[reg.SP] = reg.L;
                mem[reg.SP + 1] = reg.H;
                reg.L = temp1;
                reg.H = temp2;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "XTHL";
            }
        };

        // XCHG - Exchange Registers
        // 0xEB
        public static Instruction XCHG = new Instruction() {
            Text = "XCHG",
            Execute = (mem, args, reg, flag) => {
                ushort temp = reg.HL;
                reg.HL = reg.DE;
                reg.DE = temp;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "XCHG";
            }
        };

        // SPHL - Load SP From H And L
        // 0xF9
        public static Instruction SPHL = new Instruction() {
            Text = "SPHL",
            Execute = (mem, args, reg, flag) => {
                reg.SP = reg.HL;
                return true;
            },
            Arity = 1,
            Cycles = 5,
            GetPrintString = (args) => {
                return "SPHL";
            }
        };

        public static Dictionary<byte, Instruction> Instructions = new Dictionary<byte, Instruction>() {
            { 0x00, NOP  }, { 0x02, STAX }, { 0x03, INX  }, { 0x04, INR  }, { 0x05, DCR  }, { 0x07, RLC  },
            { 0x09, DAD  }, { 0x0A, LDAX }, { 0x0B, DCX  }, { 0x0C, INR  }, { 0x0D, DCR  }, { 0x0F, RRC  },
            { 0x12, STAX }, { 0x13, INX  }, { 0x14, INR  }, { 0x15, DCR  }, { 0x17, RAL  },
            { 0x19, DAD  }, { 0x1A, LDAX }, { 0x1B, DCX  }, { 0x1C, INR  }, { 0x1D, DCR  }, { 0x1F, RAR  },
            { 0x23, INX  }, { 0x24, INR  }, { 0x25, DCR  }, { 0x27, DAA  },
            { 0x29, DAD  }, { 0x2B, DCX  }, { 0x2C, INR  }, { 0x2D, DCR  }, { 0x2F, CMA  },
            { 0x33, INX  }, { 0x34, INR  }, { 0x35, DCR  }, { 0x37, STC  },
            { 0x39, DAD  }, { 0x3B, DCX  }, { 0x3C, INR  }, { 0x3D, DCR  }, { 0x3F, CMC  },
            { 0x40, MOV  }, { 0x41, MOV  }, { 0x42, MOV  }, { 0x43, MOV  }, { 0x44, MOV  }, { 0x45, MOV  }, { 0x46, MOV  }, { 0x47, MOV  },
            { 0x48, MOV  }, { 0x49, MOV  }, { 0x4A, MOV  }, { 0x4B, MOV  }, { 0x4C, MOV  }, { 0x4D, MOV  }, { 0x4E, MOV  }, { 0x4F, MOV  },
            { 0x50, MOV  }, { 0x51, MOV  }, { 0x52, MOV  }, { 0x53, MOV  }, { 0x54, MOV  }, { 0x55, MOV  }, { 0x56, MOV  }, { 0x57, MOV  },
            { 0x58, MOV  }, { 0x59, MOV  }, { 0x5A, MOV  }, { 0x5B, MOV  }, { 0x5C, MOV  }, { 0x5D, MOV  }, { 0x5E, MOV  }, { 0x5F, MOV  },
            { 0x60, MOV  }, { 0x61, MOV  }, { 0x62, MOV  }, { 0x63, MOV  }, { 0x64, MOV  }, { 0x65, MOV  }, { 0x66, MOV  }, { 0x67, MOV  },
            { 0x68, MOV  }, { 0x69, MOV  }, { 0x6A, MOV  }, { 0x6B, MOV  }, { 0x6C, MOV  }, { 0x6D, MOV  }, { 0x6E, MOV  }, { 0x6F, MOV  },
            { 0x70, MOV  }, { 0x71, MOV  }, { 0x72, MOV  }, { 0x73, MOV  }, { 0x74, MOV  }, { 0x75, MOV  }, { 0x77, MOV  },
            { 0x78, MOV  }, { 0x79, MOV  }, { 0x7A, MOV  }, { 0x7B, MOV  }, { 0x7C, MOV  }, { 0x7D, MOV  }, { 0x7E, MOV  }, { 0x7F, MOV  },
            { 0x80, ADD  }, { 0x81, ADD  }, { 0x82, ADD  }, { 0x83, ADD  }, { 0x84, ADD  }, { 0x85, ADD  }, { 0x86, ADD  }, { 0x87, ADD  },
            { 0x88, ADC  }, { 0x89, ADC  }, { 0x8A, ADC  }, { 0x8B, ADC  }, { 0x8C, ADC  }, { 0x8D, ADC  }, { 0x8E, ADC  }, { 0x8F, ADC  },
            { 0x90, SUB  }, { 0x91, SUB  }, { 0x92, SUB  }, { 0x93, SUB  }, { 0x94, SUB  }, { 0x95, SUB  }, { 0x96, SUB  }, { 0x97, SUB  },
            { 0x98, SBB  }, { 0x99, SBB  }, { 0x9A, SBB  }, { 0x9B, SBB  }, { 0x9C, SBB  }, { 0x9D, SBB  }, { 0x9E, SBB  }, { 0x9F, SBB  },
            { 0xA0, ANA  }, { 0xA1, ANA  }, { 0xA2, ANA  }, { 0xA3, ANA  }, { 0xA4, ANA  }, { 0xA5, ANA  }, { 0xA6, ANA  }, { 0xA7, ANA  },
            { 0xA8, XRA  }, { 0xA9, XRA  }, { 0xAA, XRA  }, { 0xAB, XRA  }, { 0xAC, XRA  }, { 0xAD, XRA  }, { 0xAE, XRA  }, { 0xAF, XRA  },
            { 0xB0, ORA  }, { 0xB1, ORA  }, { 0xB2, ORA  }, { 0xB3, ORA  }, { 0xB4, ORA  }, { 0xB5, ORA  }, { 0xB6, ORA  }, { 0xB7, ORA  },
            { 0xB8, CMP  }, { 0xB9, CMP  }, { 0xBA, CMP  }, { 0xBB, CMP  }, { 0xBC, CMP  }, { 0xBD, CMP  }, { 0xBE, CMP  }, { 0xBF, CMP  },
            { 0xC1, POP  }, { 0xC5, PUSH },
            { 0xD1, POP  }, { 0xD5, PUSH },
            { 0xE1, POP  }, { 0xE3, XTHL }, { 0xE5, PUSH },
            { 0xEB, XCHG },
            { 0xF1, POP  }, { 0xF5, PUSH }, { 0xF9, SPHL },
        };
    }
}
