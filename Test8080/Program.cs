using System;
using Emu8080;
using System.Collections.Generic;
using System.Text;

namespace Test8080 
{
    class Program 
    {
        static void Main(string[] args) 
        {
            DCRTest();
            INRTest();

            Console.Read();
        }

        static bool DCRTest() {
            return Harness.CheckConditions(
                new byte[] { 0x3D },
                (cpu) => {
                    if (cpu.Registers.A != 0xFF) { Console.WriteLine("DCR FAIL: A != 0xFF"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("DCR FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("DCR FAIL: PARITY != TRUE"); return false; }
                    return true;
                },
                goodmsg: "DCR test succeeded!"
            );
        }

        static bool INRTest() {
            return Harness.CheckConditions(
                new byte[] { 0x3C },
                (cpu) => {
                    if (cpu.Registers.A != 0x00) { Console.WriteLine("INR FAIL: A != 0x00"); return false; }
                    if (!cpu.Flag.Zero) { Console.WriteLine("INR FAIL: ZERO != TRUE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("INR FAIL: PARITY != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("INR FAIL: AUXCARRY != TRUE"); return false; }
                    return true;
                },
                (cpu) => {
                    cpu.Registers.A = 0xFF;
                },
                "INR test succeeded!"
            );
        }
    }
}
