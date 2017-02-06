using System.Collections.Generic;

namespace Test8080.Tests 
 {
    public static class RegOrMemToAccum 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("ADD", ADDTest()),
                new KeyValuePair<string, bool>("ADC", ADCTest()),
                new KeyValuePair<string, bool>("SUB", SUBTest()),
                new KeyValuePair<string, bool>("SBB", SBBTest()),
                new KeyValuePair<string, bool>("ANA", ANATest()),
                new KeyValuePair<string, bool>("XRA", XRATest()),
                new KeyValuePair<string, bool>("ORA", ORATest()),
                new KeyValuePair<string, bool>("CMP", CMPTest())
            }, "Register or Memory to Accumulator test results:");
        }

        static bool ADDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x82 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x9A && cpu.Flag.Parity && cpu.Flag.Sign && cpu.Flag.AuxCarry && !cpu.Flag.Zero && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.D = 0x2E;
                    cpu.Registers.A = 0x6C;
                }
            );
        }

        static bool ADCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x89 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x80 && !cpu.Flag.Parity && cpu.Flag.Sign && cpu.Flag.AuxCarry && !cpu.Flag.Zero && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.C = 0x3D;
                    cpu.Registers.A = 0x42;
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool SUBTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x97 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x00 && cpu.Flag.Parity && !cpu.Flag.Sign && cpu.Flag.AuxCarry && cpu.Flag.Zero && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x3E;
                }
            );
        }

        static bool SBBTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x9D },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x01 && !cpu.Flag.Parity && !cpu.Flag.Sign && cpu.Flag.AuxCarry && !cpu.Flag.Zero && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.L = 0x02;
                    cpu.Registers.A = 0x04;
                    cpu.Flag.Carry = true;
                }
            );
        }

        static bool ANATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xA1 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x0C && !cpu.Flag.Carry && !cpu.Flag.Zero && cpu.Flag.Parity;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFC;
                    cpu.Registers.C = 0x0F;
                }
            );
        }

        static bool XRATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xA8 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xD3 && !cpu.Flag.Carry && !cpu.Flag.AuxCarry && cpu.Flag.Sign && !cpu.Flag.Parity;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFF;
                    cpu.Registers.B = 0x2C;
                }
            );
        }

        static bool ORATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xB1 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x3F && !cpu.Flag.Carry && !cpu.Flag.Sign && cpu.Flag.Parity;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x33;
                    cpu.Registers.C = 0x0F;
                }
            );
        }

        static bool CMPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xBB },
                conditions: (cpu) => {
                    return cpu.Flag.Carry && !cpu.Flag.Zero;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x02;
                    cpu.Registers.E = 0x05;
                }
            );
        }
    }
}
