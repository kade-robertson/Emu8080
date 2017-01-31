﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emu8080 
{
    public class CPU 
    {

        public byte[] Memory;
        public CPURegisters Registers;
        public CPUFlag Flag;

        public bool Step(bool debug = true) {
            var inst = Memory[Registers.PC];
            try {
                var todo = InstructionSet.Instructions[inst];
                var args = new byte[3] { Memory[Registers.PC], 0, 0 };
                if (todo.Arity == 2) {
                    args[1] = Memory[++Registers.PC];
                }
                if (todo.Arity == 3) {
                    args[2] = Memory[++Registers.PC];
                }
                Registers.PC++;
                if (debug) {
                    Console.WriteLine(GetDebugText(todo, args));
                }
                var cycletouse = todo.Execute(Memory, args, Registers, Flag);
                return true;
            } catch (Exception ex) {
                Console.WriteLine($"[0x{Registers.PC.ToString("X4")}] Error: Opcode 0x{inst.ToString("X2")} not found.");
                return false;
            }
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
            sb.Append(inst.Text);
            // special print rules
            if (inst == InstructionSet.MOV) {
                sb.Append(Utils.RegisterFromBinary((byte)((args[0] & 0x3F) >> 3)));
                sb.Append(",");
                sb.Append(Utils.RegisterFromBinary((byte)(args[0] & 0x7)));
            } else if (inst == InstructionSet.INR) {
                sb.Append(Utils.RegisterFromBinary((byte)((args[0] & 0x3F) >> 3)));
            }
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
