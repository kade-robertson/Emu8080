using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Emu8080;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

namespace Play8080 
{
    class Program 
    {
        static CPU cpu;

        static readonly ushort SCREEN_WIDTH = 224;
        static readonly ushort SCREEN_HEIGHT = 256;

        static readonly byte START_VALUE = 0x4;
        static readonly byte SHOOT_VALUE = 0x10;
        static readonly byte LEFT_VALUE = 0x20;
        static readonly byte RIGHT_VALUE = 0x40;

        static void Main(string[] args) {
            var rompath = @"C:\Users\krobertson\Documents\GitHub\ROM\invaders";
            cpu = new CPU(File.ReadAllBytes(rompath));
            var gamew = new GameWindow(224, 256, GraphicsMode.Default, "Play8080", GameWindowFlags.FixedWindow, DisplayDevice.Default);
            gamew.TargetUpdateFrequency = 60;
            gamew.VSync = VSyncMode.On;
            gamew.Keyboard.KeyDown += ProcessInput;
            gamew.Keyboard.KeyUp += ClearInput;
            gamew.RenderFrame += OnRenderFrame;
            gamew.Run();
            while (true) {
                cpu.Step();
            }
            Console.Read();
        }

        static void DoInterrupt() {
            cpu.Bus.TriggerInterrupt(0xCF); // RST 8
            Thread.Sleep(8);
            cpu.Bus.TriggerInterrupt(0xD7); // RST 10
        }

        static void ProcessInput(object sender, KeyboardKeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                    cpu.IO.InPort1 |= LEFT_VALUE;
                    break;
                case Key.Right:
                    cpu.IO.InPort1 |= RIGHT_VALUE;
                    break;
                case Key.Z:
                    cpu.IO.InPort1 |= START_VALUE;
                    break;
                case Key.X:
                    cpu.IO.InPort1 |= SHOOT_VALUE;
                    break;
            }
        }

        static void ClearInput(object sender, KeyboardKeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                    cpu.IO.InPort1 &= (byte)(~LEFT_VALUE & 0xFF);
                    break;
                case Key.Right:
                    cpu.IO.InPort1 &= (byte)(~RIGHT_VALUE & 0xFF);
                    break;
                case Key.Z:
                    cpu.IO.InPort1 &= (byte)(~START_VALUE & 0xFF);
                    break;
                case Key.X:
                    cpu.IO.InPort1 &= (byte)(~SHOOT_VALUE & 0xFF);
                    break;
            }
        }

        static void OnRenderFrame(object sender, FrameEventArgs e) {
            var obj = (GameWindow)sender;

            if (cpu.Bus.Interrupt) {
                DoInterrupt();
            }

            var screen = Marshal.UnsafeAddrOfPinnedArrayElement(cpu.Memory.Skip(0x2400).Take(0x4000 - 0x2400).Reverse().ToArray(), 0);
            var screeni = new Bitmap(SCREEN_HEIGHT, SCREEN_WIDTH, 32, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, screen);
            screeni.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var screend = screeni.LockBits(new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, SCREEN_WIDTH, SCREEN_HEIGHT, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, screend.Scan0);
            screeni.UnlockBits(screend);
            obj.Title = "Play8080 FPS: " + (1f / e.Time).ToString("0.");
            obj.SwapBuffers();
        }
    }
}
