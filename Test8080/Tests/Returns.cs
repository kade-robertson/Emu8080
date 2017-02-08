using System.Collections.Generic;

namespace Test8080.Tests
{
    public static class Returns 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("RET", RETTest()),
                new KeyValuePair<string, bool>("RNZ", RNZTest()),
                new KeyValuePair<string, bool>("RZ", RZTest()),
                new KeyValuePair<string, bool>("RNC", RNCTest()),
                new KeyValuePair<string, bool>("RC", RCTest()),
                new KeyValuePair<string, bool>("RPO", RPOTest()),
                new KeyValuePair<string, bool>("RPE", RPETest()),
                new KeyValuePair<string, bool>("RP", RPTest()),
                new KeyValuePair<string, bool>("RM", RMTest())
            }, "Return test results:");
        }

        static bool RETTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC9, 0x76, 0x76, 0x3E, 0x10 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x10;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                }
            );
        }

        static bool RNZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC0, 0x76, 0x76, 0x3E, 0x11 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x11;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Zero = false;
                }
            );
        }

        static bool RZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC8, 0x76, 0x76, 0x3E, 0x12 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x12;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Zero = true;
                }
            );
        }

        static bool RNCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xD0, 0x76, 0x76, 0x3E, 0x13 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x13;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Carry = false;
                }
            );
        }

        static bool RCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xD8, 0x76, 0x76, 0x3E, 0x14 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x14;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool RPOTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE0, 0x76, 0x76, 0x3E, 0x15 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x15;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Parity = false;
                }
            );
        }

        static bool RPETest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE8, 0x76, 0x76, 0x3E, 0x16 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x16;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Parity = true;
                }
            );
        }

        static bool RPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF0, 0x76, 0x76, 0x3E, 0x17 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x17;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Sign = false;
                }
            );
        }

        static bool RMTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF8, 0x76, 0x76, 0x3E, 0x18 },
                conditions: (cpu) => {
                    return cpu.Registers.SP == 0xFF00 && cpu.Registers.A == 0x18;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0xFEFE;
                    cpu.Memory[cpu.Registers.SP] = 0x03;
                    cpu.Flag.Sign = true;
                }
            );
        }
    }
}
