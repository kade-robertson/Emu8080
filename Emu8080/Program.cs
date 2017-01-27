using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Emu8080 
{
    class Program 
    {
        static void Main(string[] args) {
            var rom = File.ReadAllBytes(@"C:\Users\krobertson\Documents\GitHub\Emu8080\Emu8080\rom\invaders");
            CPU.State.LoadROM(rom);
            while (CPU.Step(true, false)) {
                // work
            }
            Console.Read();
        }
    }
}
