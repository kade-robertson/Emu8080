using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080 
{
    public class CPUIO 
    {
        public byte OutPort2 = 0x0;
        public byte OutPort3 = 0x0;
        public ushort OutPort4 = 0x0;
        public byte OutPort5 = 0x0;
        public byte InPort1 = 0x0;
        public byte InPort2 = 0x83;

        public void SetOutput(byte device, byte value) {
            switch (device) {
                case 1: OutPort2 = value; break;
                case 2: OutPort3 = value; break;
                case 3: OutPort4 = (ushort)((value << 8) | (OutPort4 >> 8)); break;
                case 4: OutPort5 = value; break;
            }
        }

        public byte GetInput(byte device) {
            switch (device) {
                case 1: return InPort1;
                case 2: return InPort2;
                case 3: return (byte)((OutPort4 << OutPort2) >> 8);
            }
            return 0x0;
        }
    }
}
