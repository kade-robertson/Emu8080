using Emu8080;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Debug8080 
{
    public partial class Form1 : Form
    {
        CPU cpu;
        Dictionary<ushort, KeyValuePair<Instruction, string>> instenum;
        int instr;
        Stopwatch s = new Stopwatch();

        delegate void TBCallback();

        public Form1() {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            using (var o = new OpenFileDialog()) {
                o.InitialDirectory = Directory.GetCurrentDirectory();
                o.Multiselect = false;
                if (o.ShowDialog() == DialogResult.OK) {
                    var ostream = new StringWriter();
                    cpu = new CPU(File.ReadAllBytes(o.FileName));
                    instenum = cpu.EnumerateInstructions();
                    cpu.Reset(File.ReadAllBytes(o.FileName));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            cpu.Step();
            if (checkBox1.Checked) DoInstruction();
        }

        private void button2_Click(object sender, EventArgs e) {
            var counter = 10;
            while (cpu.Step() && counter > 0) {
                if (checkBox1.Checked) Task.Run(() => DoInstruction());
                counter--;
            }
        }

        private void DoInstruction() {
            if (textBox1.InvokeRequired
                || fSign.InvokeRequired
                || fZero.InvokeRequired
                || fAuxCarry.InvokeRequired
                || fParity.InvokeRequired
                || fCarry.InvokeRequired
                || rA.InvokeRequired
                || rB.InvokeRequired
                || rC.InvokeRequired
                || rD.InvokeRequired
                || rE.InvokeRequired
                || rH.InvokeRequired
                || rL.InvokeRequired
                || rPC.InvokeRequired
                || rSP.InvokeRequired) {
                var c = new TBCallback(DoInstruction);
                Invoke(c);
            } else {
                textBox1.AppendText(cpu.GetDebugText(instenum[cpu.Registers.PC].Value) + Environment.NewLine);
                textBox1.SelectionStart = textBox1.TextLength - 2;
                textBox1.ScrollToCaret();
                fSign.BackColor = cpu.Flag.Sign ? Color.LightSlateGray : Color.White;
                fZero.BackColor = cpu.Flag.Zero ? Color.LightSlateGray : Color.White;
                fAuxCarry.BackColor = cpu.Flag.AuxCarry ? Color.LightSlateGray : Color.White;
                fParity.BackColor = cpu.Flag.Parity ? Color.LightSlateGray : Color.White;
                fCarry.BackColor = cpu.Flag.Carry ? Color.LightSlateGray : Color.White;
                rA.Text = cpu.Registers.A.ToString("X2");
                rB.Text = cpu.Registers.B.ToString("X2");
                rC.Text = cpu.Registers.C.ToString("X2");
                rD.Text = cpu.Registers.D.ToString("X2");
                rE.Text = cpu.Registers.E.ToString("X2");
                rH.Text = cpu.Registers.H.ToString("X2");
                rL.Text = cpu.Registers.L.ToString("X2");
                rPC.Text = cpu.Registers.PC.ToString("X4");
                rSP.Text = cpu.Registers.SP.ToString("X4");
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            timer1.Enabled = timer1.Enabled ? false : true;
            timer1.Interval = (int)numericUpDown1.Value;
            if (timer1.Enabled) {
                s.Start();
                timer1.Start();
                timer2.Start();
            } else {
                s.Stop();
                s.Reset();
                instr = 0;
                timer1.Stop();
                timer2.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            cpu.Step();
            instr += 1;
            if (checkBox1.Checked) DoInstruction();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void timer2_Tick(object sender, EventArgs e) {
            lblIPS.Text = $"Instructions / second: {(instr / s.Elapsed.TotalSeconds).ToString("0.##")}";
        }
    }
}
