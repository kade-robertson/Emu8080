using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test8080.Tests 
{
    public static class DirectAddressing 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("STA", STATest()),
                new KeyValuePair<string, bool>("LDA", LDATest()),
                new KeyValuePair<string, bool>("SHLD", SHLDTest()),
                new KeyValuePair<string, bool>("LHLD", LHLDTest())
            }, "Direct addressing test results:");
        }

        static bool STATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x32, 0xB3, 0x05 },
                conditions: (cpu) => {
                    return cpu.Memory[0x5B3] == 0xFE;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFE;
                }
            );
        }

        static bool LDATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3A, 0xB3, 0x05 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFE;
                },
                setup: (cpu) => {
                    cpu.Memory[0x5B3] = 0xFE;
                }
            );
        }

        static bool SHLDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x22, 0xED, 0xFE },
                conditions: (cpu) => {
                    return cpu.Memory[0xFEED] == 0x21 && cpu.Memory[0xFEEE] == 0x43;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x4321;
                }
            );
        }

        static bool LHLDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x2A, 0xED, 0xFE },
                conditions: (cpu) => {
                    return cpu.Registers.L == 0x21 && cpu.Registers.H == 0x43;
                },
                setup: (cpu) => {
                    cpu.Memory[0xFEED] = 0x21;
                    cpu.Memory[0xFEEE] = 0x43;
                }
            );
        }
    }
}
