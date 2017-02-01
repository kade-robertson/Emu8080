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
            DAATest();
            MOVTest();
            STAXLDAXTest();
            ADDTest();
            ADCTest();

            Console.Read();
        }

        static bool DCRTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3D },
                conditions: (cpu) => {
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
                },
                goodmsg: "INR test succeeded!"
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
                },
                goodmsg: "DAA test succeeded!"
            );
        }

        static bool MOVTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x40, 0x51, 0x62, 0x73, 0x57 },
                conditions: (cpu) => {
                    if (cpu.Registers.B != 0xCA) { Console.WriteLine("MOV FAIL: B != 0xCA"); return false; }
                    if (cpu.Registers.D != 0xBE) { Console.WriteLine("MOV FAIL: D != 0xBE"); return false; }
                    if (cpu.Registers.H != 0xFE) { Console.WriteLine("MOV FAIL: H != 0xFE"); return false; }
                    if (cpu.Memory[cpu.Registers.HL] != 0xBA) { Console.WriteLine("MOV FAIL: M != 0xBA"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.B = 0xCA;
                    cpu.Registers.C = 0xFE;
                    cpu.Registers.E = 0xBA;
                    cpu.Registers.A = 0xBE;
                },
                goodmsg: "MOV test succeeded!"   
            );
        }

        static bool STAXLDAXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x04, 0x0C, 0x0A, 0x3C, 0x14, 0x14, 0x1C, 0x12 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x01) { Console.WriteLine("STAX/LDAX FAIL: A != 0x01"); return false; }
                    if (cpu.Registers.BC != 0x101) { Console.WriteLine("STAX/LDAX FAIL: BC != 0x101"); return false; }
                    if (cpu.Memory[cpu.Registers.DE] != 0x01) { Console.WriteLine("STAX/LDAX FAIL: (DE) != 0x01"); return false; }
                    return true;
                },
                goodmsg: "STAX / LDAX test succeeded!"
            );
        }

        static bool ADDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x82 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x9A) { Console.WriteLine("ADD FAIL: A != 0x9A"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("ADD FAIL: PARITY != TRUE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("ADD FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("ADD FAIL: AUXCARRY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ADD FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ADD FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.D = 0x2E;
                    cpu.Registers.A = 0x6C;
                },
                goodmsg: "ADD test succeeded!"
            );
        }

        static bool ADCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x89 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x80) { Console.WriteLine("ADD FAIL: A != 0x80"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("ADD FAIL: PARITY != FALSE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("ADD FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("ADD FAIL: AUXCARRY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ADD FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ADD FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.C = 0x3D;
                    cpu.Registers.A = 0x42;
                    cpu.Flag.Carry = true;
                },
                goodmsg: "ADC test succeeded!"
            );
        }
    }
}
