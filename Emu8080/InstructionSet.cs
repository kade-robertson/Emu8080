using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080
{
    public static class InstructionSet {

        // 0x00
        public static Instruction NOP = new Instruction() {
            Text = "NOP",
            Execute = (mem, args, reg, flag) => { return true; },
            Arity = 1,
            Cycles = 4,
        };

        // 0x02
        public static Instruction STAX_B = new Instruction() {
            Text = "STAX  B",
            Execute = (mem, args, reg, flag) => {
                mem[reg.BC] = reg.A;
                return true;
            },
            Arity = 1,
            Cycles = 7
        };

        // 0x03
        public static Instruction INR_B = new Instruction() {
            Text = "INR   B",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.B + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.B = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x04
        public static Instruction DCR_B = new Instruction() {
            Text = "DCR   B",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.B - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.B = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x0A
        public static Instruction LDAX_B = new Instruction() {
            Text = "LDAX  B",
            Execute = (mem, args, reg, flag) => {
                reg.A = mem[reg.BC];
                return true;
            },
            Arity = 1,
            Cycles = 7
        };

        // 0x0C
        public static Instruction INR_C = new Instruction() {
            Text = "INR   C",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.C + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.C = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x0D
        public static Instruction DCR_C = new Instruction() {
            Text = "DCR   C",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.C - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.C = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x12
        public static Instruction STAX_D = new Instruction() {
            Text = "STAX  D",
            Execute = (mem, args, reg, flag) => {
                mem[reg.DE] = reg.A;
                return true;
            },
            Arity = 1,
            Cycles = 7
        };

        // 0x13
        public static Instruction INR_D = new Instruction() {
            Text = "INR   H",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.D + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.D = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x14
        public static Instruction DCR_D = new Instruction() {
            Text = "DCR   D",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.D - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.D = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x1A
        public static Instruction LDAX_D = new Instruction() {
            Text = "LDAX  D",
            Execute = (mem, args, reg, flag) => {
                reg.A = mem[reg.DE];
                return true;
            },
            Arity = 1,
            Cycles = 7
        };

        // 0x1C
        public static Instruction INR_E = new Instruction() {
            Text = "INR   E",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.E + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.E = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x1D
        public static Instruction DCR_E = new Instruction() {
            Text = "DCR   E",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.E - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.E = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x23
        public static Instruction INR_H = new Instruction() {
            Text = "INR   H",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.H + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.H = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x24
        public static Instruction DCR_H = new Instruction() {
            Text = "DCR   H",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.H - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.H = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

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
            Cycles = 4
        };

        // 0x2C
        public static Instruction INR_L = new Instruction() {
            Text = "INR   L",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.L + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.L = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x2D
        public static Instruction DCR_L = new Instruction() {
            Text = "DCR   L",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.L - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.L = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x2F
        public static Instruction CMA = new Instruction() {
            Text = "CMA",
            Execute = (mem, args, reg, flag) => {
                var result = (byte)(~reg.A & 0xFF);
                return true;
            },
            Arity = 1,
            Cycles = 4,
        };

        // 0x33
        public static Instruction INR_M = new Instruction() {
            Text = "INR   M",
            Execute = (mem, args, reg, flag) => {
                var result = (mem[reg.HL] + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                mem[reg.HL] = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 10
        };

        // 0x34
        public static Instruction DCR_M = new Instruction() {
            Text = "DCR   M",
            Execute = (mem, args, reg, flag) => {
                var result = (mem[reg.HL] - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                mem[reg.HL] = (byte)result;
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
            Cycles = 4
        };

        // 0x3C
        public static Instruction INR_A = new Instruction() {
            Text = "INR   A",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.A + 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) == 0x0;
                reg.A = (byte)result;
                return true;
            },
            Arity = 1,
            Cycles = 5
        };

        // 0x3D
        public static Instruction DCR_A = new Instruction() {
            Text = "DCR   A",
            Execute = (mem, args, reg, flag) => {
                var result = (reg.A - 1) & 0xFF;
                flag.Sign = (result & 0x80) != 0;
                flag.Zero = (result == 0);
                flag.Parity = Utils.ParityTable[result] == 1;
                flag.AuxCarry = (result & 0xF) != 0xF;
                reg.A = (byte)result;
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

        // 0x40 - 0x7F
        public static Instruction MOV = new Instruction() {
            Text = "MOV    ",
            Execute = (mem, args, reg, flag) => {
                byte tomov = 0;
                var retval = false;
                switch ((args[0] & 0x7)) {
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
            LowCycles = 5
        };

        public static Dictionary<byte, Instruction> Instructions = new Dictionary<byte, Instruction>() {
            { 0x00, NOP    }, { 0x02, STAX_B }, { 0x03, INR_B  }, { 0x04, DCR_B  },
            { 0x0C, INR_C  }, { 0x0D, DCR_C  },
            { 0x12, STAX_D }, { 0x13, INR_D  }, { 0x14, DCR_D  },
            { 0x1C, INR_E  }, { 0x1D, DCR_E  },
            { 0x23, INR_H  }, { 0x24, DCR_H  }, { 0x27, DAA    },
            { 0x2C, INR_L  }, { 0x2D, DCR_L  }, { 0x2F, CMA    },
            { 0x33, INR_M  }, { 0x34, DCR_M  }, { 0x37, STC    },
            { 0x3C, INR_A  }, { 0x3D, DCR_A  }, { 0x3F, CMC    },
            { 0x40, MOV    }, { 0x41, MOV    }, { 0x42, MOV    }, { 0x43, MOV    }, { 0x44, MOV    }, { 0x45, MOV    }, { 0x46, MOV    }, { 0x47, MOV    },
            { 0x48, MOV    }, { 0x49, MOV    }, { 0x4A, MOV    }, { 0x4B, MOV    }, { 0x4C, MOV    }, { 0x4D, MOV    }, { 0x4E, MOV    }, { 0x4F, MOV    },
            { 0x50, MOV    }, { 0x51, MOV    }, { 0x52, MOV    }, { 0x53, MOV    }, { 0x54, MOV    }, { 0x55, MOV    }, { 0x56, MOV    }, { 0x57, MOV    },
            { 0x58, MOV    }, { 0x59, MOV    }, { 0x5A, MOV    }, { 0x5B, MOV    }, { 0x5C, MOV    }, { 0x5D, MOV    }, { 0x5E, MOV    }, { 0x5F, MOV    },
            { 0x60, MOV    }, { 0x61, MOV    }, { 0x62, MOV    }, { 0x63, MOV    }, { 0x64, MOV    }, { 0x65, MOV    }, { 0x66, MOV    }, { 0x67, MOV    },
            { 0x68, MOV    }, { 0x69, MOV    }, { 0x6A, MOV    }, { 0x6B, MOV    }, { 0x6C, MOV    }, { 0x6D, MOV    }, { 0x6E, MOV    }, { 0x6F, MOV    },
            { 0x70, MOV    }, { 0x71, MOV    }, { 0x72, MOV    }, { 0x73, MOV    }, { 0x74, MOV    }, { 0x75, MOV    }, { 0x77, MOV    },
            { 0x78, MOV    }, { 0x79, MOV    }, { 0x7A, MOV    }, { 0x7B, MOV    }, { 0x7C, MOV    }, { 0x7D, MOV    }, { 0x7E, MOV    }, { 0x7F, MOV    },
        };
    }
}
