using System;
using Test8080.Tests;

namespace Test8080 
{
    class Program 
    {
        static void Main(string[] args) {
            CarryBit.TestAll();
            SingleRegister.TestAll();
            DataTransfer.TestAll();
            RegOrMemToAccum.TestAll();
            RotateAccum.TestAll();
            RegisterPair.TestAll();
            Immediates.TestAll();
            DirectAddressing.TestAll();
            Jumps.TestAll();

            Console.Read();
        }
    }
}
