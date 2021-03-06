﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Emu8080 
{
    public class CPU 
    {

        public byte[] Memory;
        public ushort ROMSize;
        public CPURegisters Registers;
        public CPUFlag Flag;
        public CPUBus Bus;
        public CPUIO IO;
        public TextWriter DebugStream = Console.Out;
        public bool HasBeenHalted = false;

        public bool Step(bool debug = false, bool no_work = false) {
            var inst = Memory[Registers.PC];
            try {
                if (!HasBeenHalted || !Bus.Interrupt) {
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
                    if (!no_work) todo.Execute(this, args);
                }
                return true;
            } catch (KeyNotFoundException) {
                Console.WriteLine($"[0x{Registers.PC.ToString("X4")}] Error: Opcode 0x{inst.ToString("X2")} not found.");
                return false;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public Dictionary<ushort, KeyValuePair<Instruction, string>> EnumerateInstructions() {
            var ret = new Dictionary<ushort, KeyValuePair<Instruction, string>>();
            try {
                while (Registers.PC < ROMSize) {
                    var todo = InstructionSet.Instructions[Memory[Registers.PC]];
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
                    ret.Add(Registers.PC, new KeyValuePair<Instruction, string>(todo, todo.GetPrintString(args)));
                    Registers.PC += toadd;
                }
            } catch (Exception ex) {

            }
            return ret;
        }

        public void StackPush(ushort inp) {
            Memory[Registers.SP - 1] = (byte)(inp >> 8);
            Memory[Registers.SP - 2] = (byte)(inp & 0xFF);
            Registers.SP -= 2;
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

        public string GetDebugText(string instps) {
            var sb = new StringBuilder();
            sb.Append(Registers.PC.ToString("X4"));
            sb.Append(": ");
            sb.Append(instps);
            return sb.ToString();
        }

        public CPU() {
            Memory = new byte[65536];
            Registers = new CPURegisters();
            Flag = new CPUFlag();
            IO = new CPUIO();
            Bus = new CPUBus();
            Bus.InterruptInvoked += InterruptHandler;
        }

        public CPU(byte[] rom, int index = 0) : this() {
            Array.Copy(rom, 0, Memory, index, rom.Length);
            ROMSize = (ushort)rom.Length;
        }

        public void Reset(byte[] rom) {
            Memory = new byte[65536];
            Registers = new CPURegisters();
            Flag = new CPUFlag();
            IO = new CPUIO();
            Bus = new CPUBus();
            Bus.InterruptInvoked += InterruptHandler;
            Array.Copy(rom, 0, Memory, 0, rom.Length);
            ROMSize = (ushort)rom.Length;
        }

        public bool Load(byte[] data, int index = 0) {
            try {
                Array.Copy(data, 0, Memory, index, data.Length);
                ROMSize = (ushort)data.Length;
                return true;
            } catch {
                return false;
            }
        }

        public void InterruptHandler(byte inst) {
            var todo = InstructionSet.Instructions[inst];
            var args = new byte[1] { Memory[Registers.PC] };
            todo.Execute(this, args);
            HasBeenHalted = false;
        }
    }
}
