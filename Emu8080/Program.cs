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
            int counter = 0;
            while (true) {
                var check = CPU.Step(true, false);
                counter++;
                if (!check) {
                    break;
                }
            }
            Console.WriteLine($"Executed {counter} instructions.");
            Console.Read();
        }
    }
}
