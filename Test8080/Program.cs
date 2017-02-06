using System;
using Test8080.Tests;

namespace Test8080 
{
    class Program 
    {
        static void Main(string[] args) 
        {
            // Carry Bit Instructions
            CarryBit.TestAll();

            // Single Register Instructions
            SingleRegister.TestAll();

            // Data Transfer Instructions
            MOVTest();
            STAXLDAXTest();

            // Register or Memory to Accumulator Instructions
            ADDTest();
            ADCTest();
            SUBTest();
            SBBTest();
            ANATest();
            XRATest();
            ORATest();
            CMPTest();

            // Rotate Accumulator Instructions
            RLCRRCTest();
            RALRARTest();

            // Register Pair Instructions
            PUSHTest();
            POPTest();
            DADTest();
            INXTest();
            DCXTest();
            XCHGTest();
            XTHLTest();
            SPHLTest();

            // Immediate Instructions
            ADITest();
            ACITest();
            SUITest();
            SBITest();
            ANITest();
            XRITest();
            ORITest();
            CPITest();

            // Direct Addressing Instructions
            STATest();
            LDATest();
            SHLDTest();
            LHLDTest();

            // Jump Instructions
            PCHLTest();
            JMPTest();
            JNZTest();
            JZTest();
            JNCTest();
            JCTest();
            JPOTest();
            JPETest();
            JPTest();
            JMTest();

            Console.Read();
        }

        static bool MOVTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x40, 0x51, 0x62, 0x73, 0x57 },
                conditions: (cpu) => {
                    if (cpu.Registers.B != 0xCA) { Console.WriteLine("MOV FAIL: B != 0xCA"); return false; }
                    if (cpu.Registers.D != 0xBE) { Console.WriteLine("MOV FAIL: D != 0xBE"); return false; }
                    if (cpu.Registers.H != 0xFE) { Console.WriteLine("MOV FAIL: H != 0xFE"); return false; }
                    if (cpu.Memory[cpu.Registers.HL] != 0xBA) { Console.WriteLine("MOV FAIL: M != 0xBA"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.B = 0xCA;
                    cpu.Registers.C = 0xFE;
                    cpu.Registers.E = 0xBA;
                    cpu.Registers.A = 0xBE;
                },
                goodmsg: "MOV test succeeded!"   
            );
        }

        static bool STAXLDAXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x04, 0x0C, 0x0A, 0x3C, 0x14, 0x14, 0x1C, 0x12 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x01) { Console.WriteLine("STAX/LDAX FAIL: A != 0x01"); return false; }
                    if (cpu.Registers.BC != 0x101) { Console.WriteLine("STAX/LDAX FAIL: BC != 0x101"); return false; }
                    if (cpu.Memory[cpu.Registers.DE] != 0x01) { Console.WriteLine("STAX/LDAX FAIL: (DE) != 0x01"); return false; }
                    return true;
                },
                goodmsg: "STAX / LDAX test succeeded!"
            );
        }

        static bool ADDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x82 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x9A) { Console.WriteLine("ADD FAIL: A != 0x9A"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("ADD FAIL: PARITY != TRUE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("ADD FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("ADD FAIL: AUXCARRY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ADD FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ADD FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.D = 0x2E;
                    cpu.Registers.A = 0x6C;
                },
                goodmsg: "ADD test succeeded!"
            );
        }

        static bool ADCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x89 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x80) { Console.WriteLine("ADC FAIL: A != 0x80"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("ADC FAIL: PARITY != FALSE"    ); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("ADC FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("ADC FAIL: AUXCARRY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ADC FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ADC FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.C = 0x3D;
                    cpu.Registers.A = 0x42;
                    cpu.Flag.Carry = true;
                },
                goodmsg: "ADC test succeeded!"
            );
        }

        static bool SUBTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x97 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x00) { Console.WriteLine("SUB FAIL: A != 0x00"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("SUB FAIL: PARITY != TRUE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("SUB FAIL: SIGN != FALSE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("SUB FAIL: AUXCARRY != TRUE"); return false; }
                    if (!cpu.Flag.Zero) { Console.WriteLine("SUB FAIL: ZERO != TRUE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("SUB FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x3E;
                },
                goodmsg: "SUB test succeeded!"
            );
        }

        static bool SBBTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x9D },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x01) { Console.WriteLine("SBB FAIL: A != 0x01"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("SBB FAIL: PARITY != FALSE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("SBB FAIL: SIGN != FALSE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("SBB FAIL: AUXCARRY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("SBB FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("SBB FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.L = 0x02;
                    cpu.Registers.A = 0x04;
                    cpu.Flag.Carry = true;
                },
                goodmsg: "SBB test succeeded!"
            );
        }

        static bool ANATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xA1 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x0C) { Console.WriteLine("ANA FAIL: A != 0x0C"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ANA FAIL: CARRY != FALSE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ANA FAIL: ZERO != FALSE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("ANA FAIL: PARITY != TRUE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFC;
                    cpu.Registers.C = 0x0F;
                },
                goodmsg: "ANA test succeeded!"
            );
        }

        static bool XRATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xA8 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xD3) { }
                    if (cpu.Flag.Carry) { Console.WriteLine("XRA FAIL: A != 0xD3"); return false; }
                    if (cpu.Flag.AuxCarry) { Console.WriteLine("XRA FAIL: AUXCARRY != FALSE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("XRA FAIL: SIGN != TRUE"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("XRA FAIL: PARITY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFF;
                    cpu.Registers.B = 0x2C;
                },
                goodmsg: "XRA test succeeded!"
            );
        }

        static bool ORATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xB1 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x3F) { Console.WriteLine("ORA FAIL: A != 0x3F"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ORA FAIL: CARRY != FALSE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("ORA FAIL: SIGN != FALSE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("ORA FAIL: PARITY != TRUE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x33;
                    cpu.Registers.C = 0x0F;
                },
                goodmsg: "ORA test succeeded!"
            );
        }

        static bool CMPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xBB },
                conditions: (cpu) => {
                    if (!cpu.Flag.Carry) { Console.WriteLine("CMP FAIL: CARRY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("CMP FAIL: ZERO != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x02;
                    cpu.Registers.E = 0x05;
                },
                goodmsg: "CMP test succeeded!"
            );
        }

        static bool RLCRRCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x07, 0x80, 0x0F },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x7C) { Console.WriteLine("RLC/RRC FAIL: A != 0x7C"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("RLC/RRC FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x72;
                    cpu.Registers.B = 0x14;
                },
                goodmsg: "RLC / RRC test succeeded!"    
            );
        }

        static bool RALRARTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x17, 0x78, 0x1F },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xB5) { Console.WriteLine("RLC/RRC FAIL: A != 0xB5"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("RLC/RRC FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xB5;
                    cpu.Registers.B = 0x6A;
                },
                goodmsg: "RAL / RAR test succeeded!"
            );
        }

        static bool PUSHTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF5 },
                conditions: (cpu) => {
                    if (cpu.Memory[0x5029] != 0x1F) { Console.WriteLine("PUSH FAIL: $5029 != 0x1F"); return false; }
                    if (cpu.Memory[0x5028] != 0x47) { Console.WriteLine("PUSH FAIL: $5028 != 0x1F"); return false; }
                    if (cpu.Registers.SP != 0x5028) { Console.WriteLine("PUSH FAIL: SP != 0x5028"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x1F;
                    cpu.Flag.Carry = true;
                    cpu.Flag.Parity = true;
                    cpu.Flag.Zero = true;
                    cpu.Flag.AuxCarry = false;
                    cpu.Flag.Sign = false;
                    cpu.Registers.SP = 0x502A;
                },
                goodmsg: "PUSH test succeeded!"    
            );
        }

        static bool POPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF1 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xFF) { Console.WriteLine("POP FAIL: A != 0xFF"); return false; }
                    if (cpu.Registers.SP != 0x2C02) { Console.WriteLine("POP FAIL: SP != 0x2C02"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("POP FAIL: SIGN != TRUE"); return false; }
                    if (!cpu.Flag.Zero) { Console.WriteLine("POP FAIL: ZERO != TRUE"); return false; }
                    if (cpu.Flag.AuxCarry) { Console.WriteLine("POP FAIL: AUXCARRY != FALSE"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("POP FAIL: PARITY != FALSE"); return false; }
                    if (!cpu.Flag.Carry) { Console.WriteLine("POP FAIL: CARRY != TRUE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x2C00;
                    cpu.Memory[0x2C00] = 0xC3;
                    cpu.Memory[0x2C01] = 0xFF;
                },
                goodmsg: "POP test succeeded!"
            );
        }

        static bool DADTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x09 },
                conditions: (cpu) => {
                    if (cpu.Registers.HL != 0xD51A) { Console.WriteLine("DAD FAIL: HL != 0xD51A"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("DAD FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.BC = 0x339F;
                    cpu.Registers.HL = 0xA17B;
                },
                goodmsg: "DAD test succeeded!"    
            );
        }

        static bool INXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x13, 0x33 },
                conditions: (cpu) => {
                    if (cpu.Registers.DE != 0x3900) { Console.WriteLine("INX FAIL: DE != 0x3900"); return false; }
                    if (cpu.Registers.SP != 0x0000) { Console.WriteLine("INX FAIL: SP != 0x0000"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.DE = 0x38FF;
                    cpu.Registers.SP = 0xFFFF;
                },
                goodmsg: "INX test succeeded!"
            );
        }

        static bool DCXTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x2B },
                conditions: (cpu) => {
                    if (cpu.Registers.HL != 0x97FF) { Console.WriteLine("DCX FAIL: DE != 0x97FF"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x9800;
                },
                goodmsg: "DCX test succeeded!"
            );
        }

        static bool XCHGTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xEB },
                conditions: (cpu) => {
                    if (cpu.Registers.DE != 0x00FF) { Console.WriteLine("XCHG FAIL: DE != 0x00FF"); return false; }
                    if (cpu.Registers.HL != 0x3355) { Console.WriteLine("XCHG FAIL: HL != 0x3355"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.DE = 0x3355;
                    cpu.Registers.HL = 0x00FF;
                },
                goodmsg: "XCHG test succeeded!"  
            );
        }

        static bool XTHLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE3 },
                conditions: (cpu) => {
                    if (cpu.Registers.HL != 0x0DF0) { Console.WriteLine("XTHL FAIL: HL != 0x0DF0"); return false; }
                    if (cpu.Memory[0x10AD] != 0x3C) { Console.WriteLine("XCHG FAIL: $10AD != 0x3C"); return false; }
                    if (cpu.Memory[0x10AE] != 0x0B) { Console.WriteLine("XCHG FAIL: $10AE != 0x0B"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.SP = 0x10AD;
                    cpu.Registers.HL = 0x0B3C;
                    cpu.Memory[0x10AD] = 0xF0;
                    cpu.Memory[0x10AE] = 0x0D;
                },
                goodmsg: "XTHL test succeeded!"
            );
        }

        static bool SPHLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF9 },
                conditions: (cpu) => {
                    if (cpu.Registers.SP != 0x506C) { Console.WriteLine("SPHL FAIL: SP != 0x506C"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x506C;
                },
                goodmsg: "SPHL test succeeded!"
            );
        }

        static bool ADITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC6, 0x42, 0xC6, 0xBE },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x14) { Console.WriteLine("ADI FAIL: A != 0x14"); return false; }
                    if (!cpu.Flag.Carry) { Console.WriteLine("ADI FAIL: CARRY != TRUE"); return false; }
                    if (!cpu.Flag.AuxCarry) { Console.WriteLine("ADI FAIL: AUXCARRY != TRUE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("ADI FAIL: PARITY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ADI FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("ADI FAIL: SIGN != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x14;
                },
                goodmsg: "ADI test succeeded!"
            );
        }

        static bool ACITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xCE, 0xBE, 0xCE, 0x42 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x57) { Console.WriteLine("ACI FAIL: A != 0x57"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("ACI FAIL: CARRY != FALSE"); return false; }
                    if (cpu.Flag.AuxCarry) { Console.WriteLine("ACI FAIL: AUXCARRY != FALSE"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("ACI FAIL: PARITY != FALSE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ACI FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("ACI FAIL: SIGN != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x56;
                },
                goodmsg: "ACI test succeeded!"
            );
        }

        static bool SUITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xD6, 0x01 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xFF) { Console.WriteLine("SUI FAIL: A != 0xFF"); return false; }
                    if (!cpu.Flag.Carry) { Console.WriteLine("SUI FAIL: CARRY != TRUE"); return false; }
                    if (cpu.Flag.AuxCarry) { Console.WriteLine("SUI FAIL: AUXCARRY != FALSE"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("SUI FAIL: PARITY != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("SUI FAIL: ZERO != FALSE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("SUI FAIL: SIGN != TRUE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x00;
                },
                goodmsg: "SUI test succeeded!"
            );
        }

        static bool SBITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xDE, 0x01 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xFE) { Console.WriteLine("SBI FAIL: A != 0xFF"); return false; }
                    if (!cpu.Flag.Carry) { Console.WriteLine("SBI FAIL: CARRY != TRUE"); return false; }
                    if (cpu.Flag.AuxCarry) { Console.WriteLine("SBI FAIL: AUXCARRY != FALSE"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("SBI FAIL: PARITY != FALSE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("SBI FAIL: ZERO != FALSE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("SBI FAIL: SIGN != TRUE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x00;
                    cpu.Flag.Carry = true;
                },
                goodmsg: "SBI test succeeded!"
            );
        }

        static bool ANITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x79, 0xE6, 0x0F },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x0A) { Console.WriteLine("ANI FAIL: A != 0x0A"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("ANI FAIL: PARITY != TRUE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("ANI FAIL: SIGN != FALSE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ANI FAIL: ZERO != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.C = 0x3A;
                },
                goodmsg: "ANI test succeeded!"
            );
        }

        static bool XRITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xEE, 0x81 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xBA) { Console.WriteLine("XRI FAIL: A != 0xBA"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("XRI FAIL: PARITY != FALSE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("XRI FAIL: SIGN != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("XRI FAIL: ZERO != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x3B;
                },
                goodmsg: "XRI test succeeded!"
            );
        }

        static bool ORITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x79, 0xF6, 0x0F },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xBF) { Console.WriteLine("ORI FAIL: A != 0xBF"); return false; }
                    if (cpu.Flag.Parity) { Console.WriteLine("ORI FAIL: PARITY != FALSE"); return false; }
                    if (!cpu.Flag.Sign) { Console.WriteLine("ORI FAIL: SIGN != TRUE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("ORI FAIL: ZERO != FALSE"); return false; }
                    return true;
                }, 
                setup: (cpu) => {
                    cpu.Registers.A = 0x3B;
                    cpu.Registers.C = 0xB5;
                },
                goodmsg: "ORI test succeeded!"
            );
        }

        static bool CPITest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xFE, 0x40 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0x4A) { Console.WriteLine("CPI FAIL: A != 0x4A"); return false; }
                    if (!cpu.Flag.Parity) { Console.WriteLine("CPI FAIL: PARITY != TRUE"); return false; }
                    if (cpu.Flag.Sign) { Console.WriteLine("CPI FAIL: SIGN != FALSE"); return false; }
                    if (cpu.Flag.Zero) { Console.WriteLine("CPI FAIL: ZERO != FALSE"); return false; }
                    if (cpu.Flag.Carry) { Console.WriteLine("CPI FAIL: CARRY != FALSE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0x4A;
                },
                goodmsg: "CPI test succeeded!"
            );
        }

        static bool STATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x32, 0xB3, 0x05 },
                conditions: (cpu) => {
                    if (cpu.Memory[0x5B3] != 0xFE) { Console.WriteLine("STA FAIL: $05B3 != 0xFE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.A = 0xFE;
                },
                goodmsg: "STA test succeeded!"
            );
        }

        static bool LDATest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x3A, 0xB3, 0x05 },
                conditions: (cpu) => {
                    if (cpu.Registers.A != 0xFE) { Console.WriteLine("LDA FAIL: A != 0xFE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Memory[0x5B3] = 0xFE;
                },
                goodmsg: "LDA test succeeded!"
            );
        }

        static bool SHLDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x22, 0xED, 0xFE },
                conditions: (cpu) => {
                    if (cpu.Memory[0xFEED] != 0x21) { Console.WriteLine("SHLD FAIL: $FEED != 0x21"); return false; }
                    if (cpu.Memory[0xFEEE] != 0x43) { Console.WriteLine("SHLD FAIL: $FEEE != 0x43"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x4321;
                },
                goodmsg: "SHLD test succeeded!"
            );
        }

        static bool LHLDTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0x2A, 0xED, 0xFE },
                conditions: (cpu) => {
                    if (cpu.Registers.L != 0x21) { Console.WriteLine("LHLD FAIL: L != 0x21"); return false; }
                    if (cpu.Registers.H != 0x43) { Console.WriteLine("LHLD FAIL: H != 0x43"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Memory[0xFEED] = 0x21;
                    cpu.Memory[0xFEEE] = 0x43;
                },
                goodmsg: "LHLD test succeeded!"
            );
        }

        static bool PCHLTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE9 },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0x413E) { Console.WriteLine("PCHL FAIL: PC != 0x413E"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Registers.HL = 0x413E;
                },
                goodmsg: "PCHL test succeeded!"
            );
        }

        static bool JMPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC3, 0x00, 0x3E },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0x3E00) { Console.WriteLine("JMP FAIL: PC != 0x3E00"); return false; }
                    return true;
                },
                goodmsg: "JMP test succeeded!"
            );
        }

        static bool JNZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xC2, 0xFE, 0xCA },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xCAFE) { Console.WriteLine("JNZ FAIL: PC != 0xCAFE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Zero = false;
                },
                goodmsg: "JNZ test succeeded!"
            );
        }

        static bool JZTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xCA, 0xBE, 0xBA },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xBABE) { Console.WriteLine("JZ FAIL: PC != 0xBABE"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Zero = true;
                },
                goodmsg: "JZ test succeeded!"
            );
        }

        static bool JNCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xD2, 0xAD, 0xDE },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xDEAD) { Console.WriteLine("JNC FAIL: PC != 0xDEAD"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Carry = false;
                },
                goodmsg: "JNC test succeeded!"
            );
        }

        static bool JCTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xDA, 0xEF, 0xBE },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xBEEF) { Console.WriteLine("JC FAIL: PC != 0xBEEF"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Carry = true;
                },
                goodmsg: "JC test succeeded!"
            );
        }

        static bool JPOTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xE2, 0x0F, 0xD0 },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xD00F) { Console.WriteLine("JPO FAIL: PC != 0xD00F"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Parity = false;
                },
                goodmsg: "JPO test succeeded!"
            );
        }

        static bool JPETest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xEA, 0x0D, 0xF0 },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xF00D) { Console.WriteLine("JPE FAIL: PC != 0xF00D"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Parity = true;
                },
                goodmsg: "JPE test succeeded!"
            );
        }

        static bool JPTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xF2, 0xED, 0x0F },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xFED) { Console.WriteLine("JP FAIL: PC != 0xFED"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Sign = false;
                },
                goodmsg: "JM test succeeded!"
            );
        }

        static bool JMTest() {
            return Harness.CheckConditions(
                program: new byte[] { 0xFA, 0xAF, 0xDE },
                conditions: (cpu) => {
                    if (cpu.Registers.PC != 0xDEAF) { Console.WriteLine("JM FAIL: PC != 0xDEAF"); return false; }
                    return true;
                },
                setup: (cpu) => {
                    cpu.Flag.Sign = true;
                },
                goodmsg: "JM test succeeded!"
            );
        }
    }
}
