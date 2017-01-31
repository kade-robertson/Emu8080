using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080
{
    public static class InstructionSet {

        public static Dictionary<byte, Instruction> Instructions = new Dictionary<byte, Instruction>() {
            { 0x00, NOP }   , { 0x03, INR_B } , { 0x0C, INR_C } ,
            { 0x13, INR_D } , { 0x1C, INR_E } ,
            { 0x23, INR_H } , { 0x2C, INR_L } ,
            { 0x33, INR_M } , { 0x37, STC }   , { 0x3C, INR_A } , { 0x3F, CMC }
        };

        // 0x00
        public static Instruction NOP = new Instruction() {
            Text = "NOP",
            Execute = (mem, args, reg, flag) => { return true; },
            Arity = 1,
            Cycles = 4,
        };

        // 0x03
        public static Instruction INR_B = new Instruction() {
            Text = "INR   B",
            Execute = (mem, args, reg, flag) => {
                var result = reg.B + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.B == 0xF);
                reg.B = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x0C
        public static Instruction INR_C = new Instruction() {
            Text = "INR   C",
            Execute = (mem, args, reg, flag) => {
                var result = reg.C + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.C == 0xF);
                reg.C = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x13
        public static Instruction INR_D = new Instruction() {
            Text = "INR   H",
            Execute = (mem, args, reg, flag) => {
                var result = reg.D + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.D == 0xF);
                reg.D = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x1C
        public static Instruction INR_E = new Instruction() {
            Text = "INR   E",
            Execute = (mem, args, reg, flag) => {
                var result = reg.E + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.E == 0xF);
                reg.E = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x23
        public static Instruction INR_H = new Instruction() {
            Text = "INR   H",
            Execute = (mem, args, reg, flag) => {
                var result = reg.H + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.H == 0xF);
                reg.H = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x2C
        public static Instruction INR_L = new Instruction() {
            Text = "INR   L",
            Execute = (mem, args, reg, flag) => {
                var result = reg.L + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.L == 0xF);
                reg.L = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x33
        public static Instruction INR_M = new Instruction() {
            Text = "INR   M",
            Execute = (mem, args, reg, flag) => {
                var result = mem[reg.HL] + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (mem[reg.HL] == 0xF);
                mem[reg.HL] = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 10
        };

        // 0x37
        public static Instruction STC = new Instruction() {
            Text = "STC",
            Execute = (mem, args, reg, flag) => { flag.Carry = true; return true; },
            Arity = 1,
            Cycles = 4,
        };

        // 0x3C
        public static Instruction INR_A = new Instruction() {
            Text = "INR   A",
            Execute = (mem, args, reg, flag) => {
                var result = reg.A + 1;
                flag.Sign = (result >> 7) == 1;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result & 0xFF] == 1;
                flag.AuxCarry = (reg.A == 0xF);
                reg.A = (byte)(result & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x3F
        public static Instruction CMC = new Instruction() {
            Text = "CMC",
            Execute = (mem, args, reg, flag) => { flag.Carry = !flag.Carry; return true; },
            Arity = 1,
            Cycles = 4
        };
    }
}
