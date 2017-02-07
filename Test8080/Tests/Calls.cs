using System.Collections.Generic;

namespace Test8080.Tests 
{
    public static class Calls 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("CALL", CALLTest()),
                new KeyValuePair<string, bool>("CNZ", CNZTest()),
                new KeyValuePair<string, bool>("CZ", CZTest()),
                new KeyValuePair<string, bool>("CNC", CNCTest()),
                new KeyValuePair<string, bool>("CC", CCTest()),
                new KeyValuePair<string, bool>("CPO", CPOTest()),
                new KeyValuePair<string, bool>("CPE", CPETest()),
                new KeyValuePair<string, bool>("CP", CPTest()),
                new KeyValuePair<string, bool>("CM", CMTest())
            }, "Call test results:");
        }

        static bool CALLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0xCD, 0x12, 0x34 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x06 && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x3412;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                }
            );
        }

        static bool CNZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0xC4, 0x0F, 0x0F },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x07 && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x0F0F;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                }
            );
        }

        static bool CZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xCC, 0xEE, 0x0F },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x08 && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x0FEE;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                    cpu.Flag.Zero = true;
                }
            );
        }

        static bool CNCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD4, 0x12, 0x11 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x09 && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x1112;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                }
            );
        }

        static bool CCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDC, 0xF0, 0x17 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x0A && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x17F0;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool CPOTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD4, 0x98, 0x21 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x0B && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x2198;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                }
            );
        }

        static bool CPETest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDC, 0x33, 0x33 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x0C && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x3333;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool CPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD4, 0x12, 0x22 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x0D && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x2212;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                }
            );
        }

        static bool CMTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDC, 0x34, 0x12 },
                conditions: (cpu) => {
                    return cpu.Memory[0x7EFF] == 0x00 && cpu.Memory[0x7EFE] == 0x11 && cpu.Registers.SP == 0x7EFE && cpu.Registers.PC == 0x1234;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                    cpu.Flag.Carry = true;
                }
            );
        }
    }
}
