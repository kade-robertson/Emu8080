using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test8080.Tests 
{
    public static class Restarts 
    {
        public static bool TestAll() {
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("RST 0", RST0Test()),
                new KeyValuePair<string, bool>("RST 1", RST1Test())
            }, "Restart test results:");
        }

        static bool RST0Test() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3E, 0xFF, 0xC3, 0x08, 0x00, 0x00, 0x00, 0xC7, 0x00 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFF && cpu.Memory[0x7EFE] == 0x08;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                    cpu.Registers.PC = 0x07;
                }
            );
        }

        static bool RST1Test() {
            return Harness.CheckConditions(
                program: new byte[] { 0xCF, 0x3E, 0xFE, 0xC3, 0x09, 0x00, 0x00, 0x00, 0xC9, 0x00 },
                conditions: (cpu) => {
                    return cpu.Registers.A == 0xFE && cpu.Registers.SP == 0x7F00;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x7F00;
                }
            );
        }
    }
}
