using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Emu8080 
{
    public class CPU 
    {

        public byte[] Memory;
        public CPURegisters Registers;
        public CPUFlag Flag;
        public TextWriter DebugStream = Console.Out;

        public bool Step(bool debug = false) {
            var inst = Memory[Registers.PC];
            try {
                var todo = InstructionSet.Instructions[inst];
                var args = new byte[3] { Memory[Registers.PC], 0, 0 };
                var toadd = (ushort)1;
                if (todo.Arity >= 2) {
                    args[1] = Memory[Registers.PC + 1];
                    toadd++;
                }
                if (todo.Arity == 3) {
                    args[2] = Memory[Registers.PC + 2];
                    toadd++;
                }
                if (debug) {
                    DebugStream.WriteLine(GetDebugText(todo, args));
                }
                Registers.PC += toadd;
                var cycletouse = todo.Execute(this, args);
                return true;
            } catch (KeyNotFoundException) {
                Console.WriteLine($"[0x{Registers.PC.ToString("X4")}] Error: Opcode 0x{inst.ToString("X2")} not found.");
                return false;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void StackPop(out ushort outp) {
            outp = (ushort)((Memory[Registers.SP + 1] << 8) | Memory[Registers.SP]);
            Registers.SP += 2;
        }

        public bool Check() {
            foreach (KeyValuePair<byte, Instruction> k in InstructionSet.Instructions) {
                Console.WriteLine(k.Key.ToString("X2") + " : " + k.Value.Text);
            }
            return true;
        }

        public string GetDebugText(Instruction inst, byte[] args) {
            var sb = new StringBuilder();
            sb.Append(Registers.PC.ToString("X4"));
            sb.Append(": ");
            sb.Append(inst.GetPrintString(args));
            return sb.ToString();
        }

        public CPU() {
            Memory = new byte[65536];
            Registers = new CPURegisters();
            Flag = new CPUFlag();
        }

        public CPU(byte[] rom, int index = 0) {
            Memory = new byte[65536];
            Registers = new CPURegisters();
            Flag = new CPUFlag();
            Array.Copy(rom, 0, Memory, index, rom.Length);
        }

        public bool Load(byte[] data, int index = 0) {
            try {
                Array.Copy(data, 0, Memory, index, data.Length);
                return true;
            } catch {
                return false;
            }
        }
    }
}
