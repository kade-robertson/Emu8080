using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Emu8080 
{
    class Program {
        static void Main(string[] args) {
            /*var rom = File.ReadAllBytes(@"..\..\..\rom\invaders");
            var cpu = new CPU(rom);
            while (cpu.Step()) {
                Console.WriteLine(cpu.CPURegisters.PC);
            }*/
            UnderflowTest();
            OverflowTest();
            DAATest();
            MOVTest();
            Console.Read();
        }

        static void UnderflowTest() {
            var program = new byte[] {
                0x3D // DCR A
            };
            var cpu = new CPU(program);
            var counter = 0;
            while (counter < program.Length && cpu.Step()) {
                counter += 1;
            }
            Debug.Assert(cpu.Registers.A == 0xFF);
            Debug.Assert(cpu.Flag.Sign == true);
            Debug.Assert(cpu.Flag.Parity == true);
            Console.WriteLine("Underflow test succeeded!");
        }

        static void OverflowTest() {
            var program = new byte[] {
                0x3C // INR A
            };
            var cpu = new CPU(program);
            cpu.Registers.A = 0xFF;
            var counter = 0;
            while (counter < program.Length && cpu.Step()) {
                counter += 1;
            }
            Debug.Assert(cpu.Registers.A == 0x0);
            Debug.Assert(cpu.Flag.Parity == true);
            Debug.Assert(cpu.Flag.AuxCarry == true);
            Debug.Assert(cpu.Flag.Zero == true);
            Console.WriteLine("Overflow test succeeded!");
        }

        static void DAATest() {
            var program = new byte[] {
                0x27 // DAA
            };
            var cpu = new CPU(program);
            cpu.Registers.A = 0x9B;
            var counter = 0;
            while (counter < program.Length && cpu.Step()) {
                counter += 1;
            }
            Debug.Assert(cpu.Registers.A == 0x1);
            Debug.Assert(cpu.Flag.Carry == true);
            Debug.Assert(cpu.Flag.AuxCarry == true);
            Console.WriteLine("DAA test succeeded!");
        }

        static void MOVTest() {
            var program = new byte[] {
                0x40, // MOV B,B
                0x51, // MOV D,C
                0x62, // MOV H,D
                0x73, // MOV M,E
                0x57  // MOV D,A
            };
            var cpu = new CPU(program);
            cpu.Registers.B = 0xCA;
            cpu.Registers.C = 0xFE;
            cpu.Registers.E = 0xBA;
            cpu.Registers.A = 0xBE;
            var counter = 0;
            while (counter < program.Length && cpu.Step()) {
                counter += 1;
            }
            Debug.Assert(cpu.Registers.B == 0xCA);
            Debug.Assert(cpu.Registers.D == 0xBE);
            Debug.Assert(cpu.Registers.H == 0xFE);
            Debug.Assert(cpu.Memory[cpu.Registers.HL] == 0xBA);
            Console.WriteLine("MOV test succeeded!");
        }
    }
}
