using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test8080.Tests 
{
    public static class CarryBit
    {
        public static bool TestAll() {
            var cmcpass = CMCTest();
            var stcpass = STCTest();

            Console.WriteLine($"Carry bit test results:");

            Console.Write(" - CMC Test : ");
            TestUtils.PassOrFailPrint(cmcpass);

            Console.Write(" - STC Test : ");
            TestUtils.PassOrFailPrint(stcpass);

            return cmcpass && stcpass;
        }

        public static bool CMCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3F },
                conditions: (cpu) => {
                    if (cpu.Flag.Carry) { return false; }
                    return !cpu.Flag.Carry;
                },
                setup: (cpu) => {
                    cpu.Flag.Carry = true;
                }
            );
        }

        public static bool STCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x37 },
                conditions: (cpu) => {
                    return cpu.Flag.Carry;
                }
            );
        }
    }
}
