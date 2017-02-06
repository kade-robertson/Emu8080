using System.Collections.Generic;

namespace Test8080.Tests 
{
    public static class SingleRegister 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("DCR", DCRTest()),
                new KeyValuePair<string, bool>("INR", INRTest()),
                new KeyValuePair<string, bool>("CMA", CMATest()),
                new KeyValuePair<string, bool>("DAA", DAATest())
            }, "Single register test results:");
        }

        static bool DCRTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3D },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFF && cpu.Flag.Sign && cpu.Flag.Parity;
                }
            );
        }

        static bool INRTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3C },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x00 && cpu.Flag.Zero && cpu.Flag.Parity && cpu.Flag.AuxCarry;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFF;
                }
            );
        }

        static bool CMATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x2F },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xAE;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x51;
                }
            );
        }

        static bool DAATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x27 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x01 && cpu.Flag.AuxCarry && cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x9B;
                }
            );
        }
    }
}
