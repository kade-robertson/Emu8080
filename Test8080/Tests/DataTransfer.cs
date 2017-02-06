using System.Collections.Generic;

namespace Test8080.Tests
{
    public static class DataTransfer 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("MOV", MOVTest()),
                new KeyValuePair<string, bool>("STAX / LDAX", STAXLDAXTest())
            }, "Data transfer test results:");
        }

        static bool MOVTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x40, 0x51, 0x62, 0x73, 0x57 },
                conditions: (cpu) => {
                    return cpu.Registers.B == 0xCA && cpu.Registers.D == 0xBE && cpu.Registers.H == 0xFE && cpu.Memory[cpu.Registers.HL] == 0xBA;
                },
                setup: (cpu) => {
                    cpu.Registers.B = 0xCA;
                    cpu.Registers.C = 0xFE;
                    cpu.Registers.E = 0xBA;
                    cpu.Registers.A = 0xBE;
                }
            );
        }

        static bool STAXLDAXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x04, 0x0C, 0x0A, 0x3C, 0x14, 0x14, 0x1C, 0x12 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x01 && cpu.Registers.BC == 0x101 && cpu.Memory[cpu.Registers.DE] == 0x01;
                }
            );
        }
    }
}
