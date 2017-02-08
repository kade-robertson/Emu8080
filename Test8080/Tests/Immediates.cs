using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test8080.Tests 
{
    public static class Immediates 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("LXI", LXITest()),
                new KeyValuePair<string, bool>("MVI", MVITest()),
                new KeyValuePair<string, bool>("ADI", ADITest()),
                new KeyValuePair<string, bool>("ACI", ACITest()),
                new KeyValuePair<string, bool>("SUI", SUITest()),
                new KeyValuePair<string, bool>("SBI", SBITest()),
                new KeyValuePair<string, bool>("ANI", ANITest()),
                new KeyValuePair<string, bool>("XRI", XRITest()),
                new KeyValuePair<string, bool>("ORI", ORITest()),
                new KeyValuePair<string, bool>("CPI", CPITest())
            }, "Immediate test results:");
        }

        static bool LXITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x01, 0xAA, 0xAB, 0x11, 0xED, 0xFE, 0x21, 0xFE, 0xCA, 0x31, 0x0D, 0xF0 },
                conditions: (cpu) => {
                    return cpu.Registers.BC == 0xABAA && cpu.Registers.DE == 0xFEED && cpu.Registers.HL == 0xCAFE && cpu.Registers.SP == 0xF00D;
                }
            );
        }

        static bool MVITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x06, 0xFE, 0x1E, 0x20, 0x36, 0x3E, 0x3E, 0xF2 },
                conditions: (cpu) => {
                    return cpu.Registers.B == 0xFE && cpu.Registers.E == 0x20 && cpu.Memory[0x200] == 0x3E && cpu.Registers.A == 0xF2;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x200;
                }
            );
        }

        static bool ADITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC6, 0x42, 0xC6, 0xBE },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x14 && cpu.Flag.Carry && cpu.Flag.AuxCarry && cpu.Flag.Parity && !cpu.Flag.Zero && !cpu.Flag.Sign;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x14;
                }
            );
        }

        static bool ACITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xCE, 0xBE, 0xCE, 0x42 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x57 && !cpu.Flag.Carry && !cpu.Flag.AuxCarry && !cpu.Flag.Parity && !cpu.Flag.Zero && !cpu.Flag.Sign;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x56;
                }
            );
        }

        static bool SUITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xD6, 0x01 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFF && cpu.Flag.Carry && !cpu.Flag.AuxCarry && cpu.Flag.Parity && !cpu.Flag.Zero && cpu.Flag.Sign;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x00;
                }
            );
        }

        static bool SBITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xDE, 0x01 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFE && cpu.Flag.Carry && !cpu.Flag.AuxCarry && !cpu.Flag.Parity && !cpu.Flag.Zero && cpu.Flag.Sign;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x00;
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool ANITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x79, 0xE6, 0x0F },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x0A && cpu.Flag.Parity && !cpu.Flag.Sign && !cpu.Flag.Zero;
                },
                setup: (cpu) => {
                    cpu.Registers.C = 0x3A;
                }
            );
        }

        static bool XRITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xEE, 0x81 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xBA && !cpu.Flag.Parity && cpu.Flag.Sign && !cpu.Flag.Zero;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x3B;
                }
            );
        }
        static bool ORITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x79, 0xF6, 0x0F },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xBF && !cpu.Flag.Parity && cpu.Flag.Sign && !cpu.Flag.Zero;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x3B;
                    cpu.Registers.C = 0xB5;
                }
            );
        }

        static bool CPITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xFE, 0x40 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x4A && cpu.Flag.Parity && !cpu.Flag.Sign && !cpu.Flag.Zero && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x4A;
                }
            );
        }
    }
}
