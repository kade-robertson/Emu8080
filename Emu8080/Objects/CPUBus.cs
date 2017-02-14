using System;

namespace Emu8080
{
    public class CPUBus 
    {
        public delegate void InterruptChangedHandler();
        public delegate void InputRequestedHandler();
        public delegate void OutputReceivedHandler();
        public delegate void InterruptInvokedHandler(byte inst);

        public event InterruptChangedHandler InterruptChanged;
        public event InputRequestedHandler InputRequested;
        public event OutputReceivedHandler OutputReceived;
        public event InterruptInvokedHandler InterruptInvoked;

        private bool m_interrupt = false;

        public bool Interrupt {
            get {
                return m_interrupt; 
            }
            set {
                m_interrupt = value;
                InterruptChanged?.Invoke();
            }
        }

        public void TriggerInterrupt(byte inst) {
            InterruptInvoked?.Invoke(inst);
        }

        public void RequestInput() {
            InputRequested?.Invoke();
        }

        public void DeliverOutput() {
            OutputReceived?.Invoke();
        }
    }
}
