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
            Console.Read();
        }

        static void UnderflowTest() {
            var program = new byte[] {
                0x3D, // DCR A
            };
            var cpu = new CPU(program);
            var counter = 0;
            while (cpu.Step() && counter < program.Length) {
                counter += 1;
            }
            Debug.Assert(cpu.CPURegisters.A == 0xFF);
            Debug.Assert(cpu.CPUFlag.Sign == true);
            Debug.Assert(cpu.CPUFlag.Parity == true);
            Console.WriteLine("Underflow test succeeded!");
        }

        static void OverflowTest() {
            var program = new byte[] {
                0x3C, // INR A
            };
            var cpu = new CPU(program);
            cpu.CPURegisters.A = 0xFF;
            var counter = 0;
            while (cpu.Step() && counter < program.Length) {
                counter += 1;
            }
            Debug.Assert(cpu.CPURegisters.A == 0x0);
            Debug.Assert(cpu.CPUFlag.Parity == true);
            Debug.Assert(cpu.CPUFlag.AuxCarry == true);
            Debug.Assert(cpu.CPUFlag.Zero == true);
            Console.WriteLine("Overflow test succeeded!");
        }

        static void DAATest() {
            var program = new byte[] {
                0x27, // DAA
            };
            var cpu = new CPU(program);
            cpu.CPURegisters.A = 0x9B;
            var counter = 0;
            while (cpu.Step() && counter < program.Length) {
                counter += 1;
            }
            Debug.Assert(cpu.CPURegisters.A == 0x1);
            Debug.Assert(cpu.CPUFlag.Carry == true);
            Debug.Assert(cpu.CPUFlag.AuxCarry == true);
            Console.WriteLine("DAA test succeeded!");
        }
    }
}
