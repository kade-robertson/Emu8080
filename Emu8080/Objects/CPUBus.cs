using System;

namespace Emu8080
{
    public class CPUBus 
    {
        public delegate void InterruptChangedHandler(CPUBus bus, EventArgs e);
        public delegate void InputRequestedHandler(CPUBus bus, EventArgs e);
        public delegate void OutputReceivedHandler(CPUBus bus, EventArgs e);

        public event InterruptChangedHandler InterruptChanged;
        public event InputRequestedHandler InputRequested;
        public event OutputReceivedHandler OutputReceived;

        private bool m_interrupt = false;

        public bool Interrupt {
            get {
                return m_interrupt; 
            }
            set {
                m_interrupt = value;
                InterruptChanged?.Invoke(this, null);
            }
        }

        public void RequestInput() {
            InputRequested?.Invoke(this, null);
        }

        public void DeliverOutput() {
            OutputReceived?.Invoke(this, null);
        }
    }
}
