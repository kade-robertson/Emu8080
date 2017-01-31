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
            MOVPrintTest();
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

        static void MOVPrintTest() {
            var program = new byte[] {
                0x40,
                0x4F,
                0x72,
                0x5E
            };
            var cpu = new CPU(program);
            var counter = 0;
            while (counter < program.Length && cpu.Step()) {
                counter += 1;
            }
        }
    }
}
