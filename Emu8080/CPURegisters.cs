using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080 
{
    public class CPURegisters 
    {
        public byte A; // accumulator
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;

        public ushort PC; // program counter
        public ushort SP; // stack pointer

        public ushort BC {
            get {
                return (ushort)((B << 8) | C);
            } set {
                B = (byte)(value >> 8);
                C = (byte)(value & 0xFF);
            }
        }

        public ushort DE {
            get {
                return (ushort)((D << 8) | E);
            }
            set {
                D = (byte)(value >> 8);
                E = (byte)(value & 0xFF);
            }
        }

        public ushort HL {
            get {
                return (ushort)((H << 8) | L);
            }
            set {
                H = (byte)(value >> 8);
                L = (byte)(value & 0xFF);
            }
        }

        public CPURegisters() {
            BC = 0;
            DE = 0;
            HL = 0;
            A = 0;
            PC = 0;
            SP = 0xF000;
        }
    }
}
