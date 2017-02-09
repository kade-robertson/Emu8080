using System;

namespace Emu8080
{
    public class CPUBus 
    {
        private CPU m_cpuref;

        public delegate void InterruptChangedHandler(CPU cpu, EventArgs e);
        public delegate void InputRequestedHandler(CPU cpu, EventArgs e);
        public delegate void OutputReceivedHandler(CPU bus, EventArgs e);
        public delegate void InterruptInvokedHandler(CPU bus, EventArgs e);

        public event InterruptChangedHandler InterruptChanged;
        public event InputRequestedHandler InputRequested;
        public event OutputReceivedHandler OutputReceived;
        public event InterruptInvokedHandler InterruptInvoked;

        private bool m_interrupt = false;

        public CPUBus(CPU cpuref) {
            m_cpuref = cpuref;
        }

        public bool Interrupt {
            get {
                return m_interrupt; 
            }
            set {
                m_interrupt = value;
                InterruptChanged?.Invoke(m_cpuref, null);
            }
        }

        public void TriggerInterrupt() {
            InterruptInvoked?.Invoke(m_cpuref, null);
        }

        public void RequestInput() {
            InputRequested?.Invoke(m_cpuref, null);
        }

        public void DeliverOutput() {
            OutputReceived?.Invoke(m_cpuref, null);
        }
    }
}
