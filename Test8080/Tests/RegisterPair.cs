using System.Collections.Generic;

namespace Test8080.Tests 
{
    public static class RegisterPair 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("PUSH", PUSHTest()),
                new KeyValuePair<string, bool>("POP", POPTest()),
                new KeyValuePair<string, bool>("DAD", DADTest()),
                new KeyValuePair<string, bool>("INX", INXTest()),
                new KeyValuePair<string, bool>("DCX", DCXTest()),
                new KeyValuePair<string, bool>("XCHG", XCHGTest()),
                new KeyValuePair<string, bool>("XTHL", XTHLTest()),
                new KeyValuePair<string, bool>("SPHL", SPHLTest())
            }, "Register pair test results:");
        }

        static bool PUSHTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF5 },
                conditions: (cpu) => {
                    return cpu.Memory[0x5029] == 0x1F && cpu.Memory[0x5028] == 0x47 && cpu.Registers.SP == 0x5028;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x1F;
                    cpu.Flag.Carry = true;
                    cpu.Flag.Parity = true;
                    cpu.Flag.Zero = true;
                    cpu.Flag.AuxCarry = false;
                    cpu.Flag.Sign = false;
                    cpu.Registers.SP = 0x502A;
                }
            );
        }

        static bool POPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF1 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFF && cpu.Registers.SP == 0x2C02 && cpu.Flag.Sign && cpu.Flag.Zero && !cpu.Flag.AuxCarry && !cpu.Flag.Parity && cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x2C00;
                    cpu.Memory[0x2C00] = 0xC3;
                    cpu.Memory[0x2C01] = 0xFF;
                }
            );
        }

        static bool DADTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x09 },
                conditions: (cpu) => {
                    return cpu.Registers.HL == 0xD51A && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.BC = 0x339F;
                    cpu.Registers.HL = 0xA17B;
                }
            );
        }

        static bool INXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x13, 0x33 },
                conditions: (cpu) => {
                    return cpu.Registers.DE == 0x3900 && cpu.Registers.SP == 0x0000;
                },
                setup: (cpu) => {
                    cpu.Registers.DE = 0x38FF;
                    cpu.Registers.SP = 0xFFFF;
                }
            );
        }

        static bool DCXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x2B },
                conditions: (cpu) => {
                    return cpu.Registers.HL == 0x97FF;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x9800;
                }
            );
        }

        static bool XCHGTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xEB },
                conditions: (cpu) => {
                    return cpu.Registers.DE == 0x00FF && cpu.Registers.HL == 0x3355;
                },
                setup: (cpu) => {
                    cpu.Registers.DE = 0x3355;
                    cpu.Registers.HL = 0x00FF;
                }
            );
        }

        static bool XTHLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE3 },
                conditions: (cpu) => {
                    return cpu.Registers.HL == 0x0DF0 && cpu.Memory[0x10AD] == 0x3C && cpu.Memory[0x10AE] == 0x0B;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x10AD;
                    cpu.Registers.HL = 0x0B3C;
                    cpu.Memory[0x10AD] = 0xF0;
                    cpu.Memory[0x10AE] = 0x0D;
                }
            );
        }

        static bool SPHLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF9 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0x506C;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x506C;
                }
            );
        }
    }
}
