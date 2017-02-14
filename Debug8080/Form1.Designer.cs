namespace Debug8080 {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnLoad = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.fSign = new System.Windows.Forms.Panel();
            this.fZero = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fAuxCarry = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.fParity = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.fCarry = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.rA = new System.Windows.Forms.Label();
            this.rB = new System.Windows.Forms.Label();
            this.rC = new System.Windows.Forms.Label();
            this.rE = new System.Windows.Forms.Label();
            this.rD = new System.Windows.Forms.Label();
            this.rL = new System.Windows.Forms.Label();
            this.rH = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.rSP = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.rPC = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblIPS = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(12, 126);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(435, 329);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(93, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Run One";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(159, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(61, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Run Ten";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "S";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Z";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "-";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(105, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "-";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "-";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "AC";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "P";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(171, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "C";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fSign
            // 
            this.fSign.BackColor = System.Drawing.Color.White;
            this.fSign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fSign.Location = new System.Drawing.Point(15, 33);
            this.fSign.Name = "fSign";
            this.fSign.Size = new System.Drawing.Size(16, 16);
            this.fSign.TabIndex = 4;
            // 
            // fZero
            // 
            this.fZero.BackColor = System.Drawing.Color.White;
            this.fZero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fZero.Location = new System.Drawing.Point(37, 33);
            this.fZero.Name = "fZero";
            this.fZero.Size = new System.Drawing.Size(16, 16);
            this.fZero.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(59, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(16, 16);
            this.panel1.TabIndex = 6;
            // 
            // fAuxCarry
            // 
            this.fAuxCarry.BackColor = System.Drawing.Color.White;
            this.fAuxCarry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fAuxCarry.Location = new System.Drawing.Point(81, 33);
            this.fAuxCarry.Name = "fAuxCarry";
            this.fAuxCarry.Size = new System.Drawing.Size(16, 16);
            this.fAuxCarry.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(103, 33);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(16, 16);
            this.panel3.TabIndex = 6;
            // 
            // fParity
            // 
            this.fParity.BackColor = System.Drawing.Color.White;
            this.fParity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fParity.Location = new System.Drawing.Point(125, 33);
            this.fParity.Name = "fParity";
            this.fParity.Size = new System.Drawing.Size(16, 16);
            this.fParity.TabIndex = 6;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.LightSlateGray;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel5.Location = new System.Drawing.Point(147, 33);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(16, 16);
            this.panel5.TabIndex = 6;
            // 
            // fCarry
            // 
            this.fCarry.BackColor = System.Drawing.Color.White;
            this.fCarry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fCarry.ForeColor = System.Drawing.SystemColors.Control;
            this.fCarry.Location = new System.Drawing.Point(169, 33);
            this.fCarry.Name = "fCarry";
            this.fCarry.Size = new System.Drawing.Size(16, 16);
            this.fCarry.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(226, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 23);
            this.button3.TabIndex = 16;
            this.button3.Text = "Run With Delay";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // rA
            // 
            this.rA.AutoSize = true;
            this.rA.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rA.Location = new System.Drawing.Point(12, 35);
            this.rA.Name = "rA";
            this.rA.Size = new System.Drawing.Size(21, 14);
            this.rA.TabIndex = 17;
            this.rA.Text = "00";
            // 
            // rB
            // 
            this.rB.AutoSize = true;
            this.rB.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rB.Location = new System.Drawing.Point(31, 35);
            this.rB.Name = "rB";
            this.rB.Size = new System.Drawing.Size(21, 14);
            this.rB.TabIndex = 18;
            this.rB.Text = "00";
            // 
            // rC
            // 
            this.rC.AutoSize = true;
            this.rC.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rC.Location = new System.Drawing.Point(48, 35);
            this.rC.Name = "rC";
            this.rC.Size = new System.Drawing.Size(21, 14);
            this.rC.TabIndex = 19;
            this.rC.Text = "00";
            // 
            // rE
            // 
            this.rE.AutoSize = true;
            this.rE.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rE.Location = new System.Drawing.Point(84, 35);
            this.rE.Name = "rE";
            this.rE.Size = new System.Drawing.Size(21, 14);
            this.rE.TabIndex = 21;
            this.rE.Text = "00";
            // 
            // rD
            // 
            this.rD.AutoSize = true;
            this.rD.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rD.Location = new System.Drawing.Point(67, 35);
            this.rD.Name = "rD";
            this.rD.Size = new System.Drawing.Size(21, 14);
            this.rD.TabIndex = 20;
            this.rD.Text = "00";
            // 
            // rL
            // 
            this.rL.AutoSize = true;
            this.rL.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rL.Location = new System.Drawing.Point(120, 35);
            this.rL.Name = "rL";
            this.rL.Size = new System.Drawing.Size(21, 14);
            this.rL.TabIndex = 23;
            this.rL.Text = "00";
            // 
            // rH
            // 
            this.rH.AutoSize = true;
            this.rH.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rH.Location = new System.Drawing.Point(103, 35);
            this.rH.Name = "rH";
            this.rH.Size = new System.Drawing.Size(21, 14);
            this.rH.TabIndex = 22;
            this.rH.Text = "00";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 18);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(14, 13);
            this.label16.TabIndex = 24;
            this.label16.Text = "A";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(35, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(14, 13);
            this.label17.TabIndex = 25;
            this.label17.Text = "B";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(51, 18);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(14, 13);
            this.label18.TabIndex = 26;
            this.label18.Text = "C";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(87, 18);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(13, 13);
            this.label19.TabIndex = 28;
            this.label19.Text = "E";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(70, 18);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(15, 13);
            this.label20.TabIndex = 27;
            this.label20.Text = "D";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(124, 18);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(12, 13);
            this.label21.TabIndex = 30;
            this.label21.Text = "L";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(106, 18);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(15, 13);
            this.label22.TabIndex = 29;
            this.label22.Text = "H";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(336, 12);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(45, 22);
            this.numericUpDown1.TabIndex = 31;
            this.numericUpDown1.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.fSign);
            this.groupBox1.Controls.Add(this.fZero);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.fAuxCarry);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.fParity);
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.fCarry);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 61);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Flags";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.rSP);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.rPC);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.rA);
            this.groupBox2.Controls.Add(this.rB);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.rC);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.rD);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.rE);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.rH);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.rL);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Location = new System.Drawing.Point(218, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(229, 60);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Registers";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(194, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(19, 13);
            this.label11.TabIndex = 34;
            this.label11.Text = "SP";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rSP
            // 
            this.rSP.AutoSize = true;
            this.rSP.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rSP.Location = new System.Drawing.Point(186, 35);
            this.rSP.Name = "rSP";
            this.rSP.Size = new System.Drawing.Size(35, 14);
            this.rSP.TabIndex = 33;
            this.rSP.Text = "0000";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(155, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 13);
            this.label10.TabIndex = 32;
            this.label10.Text = "PC";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rPC
            // 
            this.rPC.AutoSize = true;
            this.rPC.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rPC.Location = new System.Drawing.Point(147, 35);
            this.rPC.Name = "rPC";
            this.rPC.Size = new System.Drawing.Size(35, 14);
            this.rPC.TabIndex = 31;
            this.rPC.Text = "0000";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(387, 16);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(55, 17);
            this.checkBox1.TabIndex = 34;
            this.checkBox1.Text = "Print?";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // lblIPS
            // 
            this.lblIPS.AutoSize = true;
            this.lblIPS.Location = new System.Drawing.Point(12, 108);
            this.lblIPS.Name = "lblIPS";
            this.lblIPS.Size = new System.Drawing.Size(118, 13);
            this.lblIPS.TabIndex = 35;
            this.lblIPS.Text = "Instructions / second:";
            // 
            // timer2
            // 
            this.timer2.Interval = 333;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(460, 468);
            this.Controls.Add(this.lblIPS);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnLoad);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Debug8080";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel fSign;
        private System.Windows.Forms.Panel fZero;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel fAuxCarry;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel fParity;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel fCarry;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label rA;
        private System.Windows.Forms.Label rB;
        private System.Windows.Forms.Label rC;
        private System.Windows.Forms.Label rE;
        private System.Windows.Forms.Label rD;
        private System.Windows.Forms.Label rL;
        private System.Windows.Forms.Label rH;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label rSP;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label rPC;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblIPS;
        private System.Windows.Forms.Timer timer2;
    }
}

