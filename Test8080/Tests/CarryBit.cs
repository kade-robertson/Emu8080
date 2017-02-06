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
            return TestUtils.DoTests(new List<KeyValuePair<string, bool>>() {
                new KeyValuePair<string, bool>("CMC", CMCTest()),
                new KeyValuePair<string, bool>("STC", STCTest())
            }, "Carry bit test results:");
        }

        public static bool CMCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3F },
                conditions: (cpu) => {
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
