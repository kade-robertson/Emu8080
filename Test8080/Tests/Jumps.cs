using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test8080.Tests
{
    public static class Jumps 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("PCHL", PCHLTest()),
                new KeyValuePair<string, bool>("JMP", JMPTest()),
                new KeyValuePair<string, bool>("JNZ", JNZTest()),
                new KeyValuePair<string, bool>("JZ", JZTest()),
                new KeyValuePair<string, bool>("JNC", JNCTest()),
                new KeyValuePair<string, bool>("JC", JCTest()),
                new KeyValuePair<string, bool>("JPO", JPOTest()),
                new KeyValuePair<string, bool>("JPE", JPETest()),
                new KeyValuePair<string, bool>("JP", JPTest()),
                new KeyValuePair<string, bool>("JM", JMTest())
            }, "Jump test results:");
        }

        static bool PCHLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE9 },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0x413E;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x413E;
                }
            );
        }

        static bool JMPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC3, 0x00, 0x3E },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0x3E00;
                }
            );
        }

        static bool JNZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC2, 0xFE, 0xCA },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xCAFE;
                },
                setup: (cpu) => {
                    cpu.Flag.Zero = false;
                }
            );
        }

        static bool JZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xCA, 0xBE, 0xBA },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xBABE;
                },
                setup: (cpu) => {
                    cpu.Flag.Zero = true;
                }
            );
        }

        static bool JNCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xD2, 0xAD, 0xDE },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xDEAD;
                },
                setup: (cpu) => {
                    cpu.Flag.Carry = false;
                }
            );
        }

        static bool JCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xDA, 0xEF, 0xBE },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xBEEF;
                },
                setup: (cpu) => {
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool JPOTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE2, 0x0F, 0xD0 },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xD00F;
                },
                setup: (cpu) => {
                    cpu.Flag.Parity = false;
                }
            );
        }

        static bool JPETest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xEA, 0x0D, 0xF0 },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xF00D;
                },
                setup: (cpu) => {
                    cpu.Flag.Parity = true;
                }
            );
        }

        static bool JPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF2, 0xED, 0x0F },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xFED;
                },
                setup: (cpu) => {
                    cpu.Flag.Sign = false;
                }
            );
        }

        static bool JMTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xFA, 0xAF, 0xDE },
                conditions: (cpu) => {
                    return cpu.Registers.PC == 0xDEAF;
                },
                setup: (cpu) => {
                    cpu.Flag.Sign = true;
                }
            );
        }
    }
}
