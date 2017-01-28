using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Emu8080 
{
    class Program 
    {
        static void Main(string[] args) {
            var rom = File.ReadAllBytes(@"rom\invaders");
            CPU.State.LoadROM(rom);
            int counter = 0;
            while (CPU.Step(true, false)) {
                counter++;
            }
            Console.WriteLine($"Executed {counter} instructions.");
            Console.Read();
        }
    }
}
