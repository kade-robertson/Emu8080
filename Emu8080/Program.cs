using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Emu8080 
{
    class Program 
    {
        static void Main(string[] args) {
            var rom = File.ReadAllBytes(@"..\..\..\rom\invaders");
            CPU.State.LoadROM(rom);
            int counter = 0;
            int x = int.Parse(Console.ReadLine());
            while (CPU.Step(true, true, false, counter, x) && counter <= x) {
                counter++;
            }
            Console.WriteLine($"{CPU.State.Memory[CPU.State.StackPointer].ToString("X2")}{CPU.State.Memory[CPU.State.StackPointer + 1].ToString("X2")}");
            Console.WriteLine($"Executed {counter} instructions.");
            Console.Read();
        }
    }
}
