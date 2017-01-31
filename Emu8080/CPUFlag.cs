using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emu8080 
{
    public enum FlagBits {
        Carry = 0x01,
        Parity = 0x04,
        AuxCarry = 0x10,
        Zero = 0x40,
        Sign = 0x80
    }

    public class CPUFlag
    {
        public bool Carry = false;
        public bool AuxCarry = false;
        public bool Sign = false;
        public bool Zero = false;
        public bool Parity = false;

        public byte FlagByte {
            get {
                byte outb = 0x02; // this bit is always set, bits 3 and 5 are never set
                if (Carry) {
                    outb |= (byte)FlagBits.Carry;
                } else {
                    outb &= (~(byte)(FlagBits.Carry) & 0xFF);
                }
                if (Parity) {
                    outb |= (byte)FlagBits.Parity;
                } else {
                    outb &= (~(byte)(FlagBits.Parity) & 0xFF);
                }
                if (AuxCarry) {
                    outb |= (byte)FlagBits.AuxCarry;
                } else {
                    outb &= (~(byte)(FlagBits.AuxCarry) & 0xFF);
                }
                if (Zero) {
                    outb |= (byte)FlagBits.Zero;
                } else {
                    outb &= (~(byte)(FlagBits.Zero) & 0xFF);
                }
                if (Sign) {
                    outb |= (byte)FlagBits.Sign;
                } else {
                    outb &= (~(byte)(FlagBits.Sign) & 0xFF);
                }
                return outb;
            } set {
                Carry = (value & (byte)FlagBits.Carry) > 0;
                Parity = (value & (byte)FlagBits.Parity) > 0;
                AuxCarry = (value & (byte)FlagBits.AuxCarry) > 0;
                Zero = (value & (byte)FlagBits.Zero) > 0;
                Sign = (value & (byte)FlagBits.Sign) > 0;
            }
        }
    }
}
