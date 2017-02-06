using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test8080.Tests 
{
    public static class SingleRegister 
    {
        public static bool TestAll() {
            var dcrpass = DCRTest();
            var inrpass = INRTest();
            var cmapass = CMATest();
            var daapass = DAATest();

            Console.WriteLine($"Single register test results:");

            Console.Write(" - DCR Test : ");
            TestUtils.PassOrFailPrint(dcrpass);

            Console.Write(" - INR Test : ");
            TestUtils.PassOrFailPrint(inrpass);

            Console.Write(" - CMA Test : ");
            TestUtils.PassOrFailPrint(cmapass);

            Console.Write(" - DAA Test : ");
            TestUtils.PassOrFailPrint(daapass);

            return dcrpass && inrpass && cmapass && daapass;
        }

        static bool DCRTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3D },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xFF) { Console.WriteLine("DCR FAIL: A != 0xFF"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("DCR FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("DCR FAIL: PARITY != TRUE"); return false; }
                    return true;
                }
            );
        }

        static bool INRTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3C },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x00) { Console.WriteLine("INR FAIL: A != 0x00"); return false; }
                    if (!cpu.Flag.Zero) { Console.WriteLine("INR FAIL: ZERO != TRUE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("INR FAIL: PARITY != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("INR FAIL: AUXCARRY != TRUE"); return false; }
                    return true;
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
                    if (cpu.Registers.A != 0xAE) { Console.WriteLine("CMA FAIL: A != 0xAE"); return false; }
                    return true;
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
                    if (cpu.Registers.A != 0x01) { Console.WriteLine("DAA FAIL: A != 0x01"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("DAA FAIL: AUXCARRY != TRUE"); return false; }
                    if (!cpu.Flag.Carry) { Console.WriteLine("DAA FAIL: CARRY != TRUE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x9B;
                }
            );
        }
    }
}
