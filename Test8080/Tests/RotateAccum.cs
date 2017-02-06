using System.Collections.Generic;

namespace Test8080.Tests 
{
    public static class RotateAccum 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("RLC / RRC", RLCRRCTest()),
                new KeyValuePair<string, bool>("RAL / RAR", RALRARTest())
            }, "Rotate accumulator test results: ");
        }

        static bool RLCRRCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x07, 0x80, 0x0F },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0x7C && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x72;
                    cpu.Registers.B = 0x14;
                }
            );
        }

        static bool RALRARTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x17, 0x78, 0x1F },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xB5 && !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xB5;
                    cpu.Registers.B = 0x6A;
                }
            );
        }
    }
}
