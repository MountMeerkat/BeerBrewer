using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Services
{
    public enum MicroControllerCommands : byte
    {
        // Receive Commands
        TempData = 0x01,
        HeaterStatus = 0x02,
        PumpStatus = 0x03,
        TargetTempStatus = 0x04,
        TargetTempValue = 0x05,
        AutomationParameters = 0x06,
        AutomationProgressState = 0x07,

        // Send Commands
        StartDataStream = 0xf1,
        StopDataStream = 0xf2,
        SetHeaterPower = 0xf3,
        SetPumpPower = 0xf4,
        SetTargetTemp = 0xf5,
        SetTargetValue = 0xf6,
        SetAutomationParameters = 0xF7,
        SetAutomationActivation = 0xF8
    }

    public enum AutomationProgressStates : byte
    {
        Off = 0x00,
        HeatingMesk = 0x01,
        Mesking = 0x02,
        HeatingBoil = 0x03,
        Boiling = 0x04,
        Cooling = 0x05
    }
    public class MicroControllerDataEventArgs : EventArgs
    {
        public float TempA { get; set; }
        public float TempB { get; set; }
        public float TempC { get; set; }
        public DateTime TimeReceived { get; set; }
    }

    public class MicroControllerTimeoutEventArgs : EventArgs
    {
        public DateTime LastMessageTime { get; set; }
        public DateTime TimeChecked { get; set; }
    }
    public class MicroControllerHeaterEventArgs : EventArgs
    {
        public bool HeaterActive { get; set; }
        public DateTime TimeReceived { get; set; }
    }

    public class MicroControllerPumpEventArgs : EventArgs
    {
        public bool PumpActive { get; set; }
        public DateTime TimeReceived { get; set; }
    }

    public class MicroControllerTargetTempEventArgs : EventArgs
    {
        public bool TargetActive { get; set; }
        public DateTime TimeReceived { get; set; }
    }
    public class MicroControllerTargetValueEventArgs : EventArgs
    {
        public int TargetTemp { get; set; }
        public DateTime TimeReceived { get; set; }
    }
    public class MicroControllerAutomationParametersEventArgs : EventArgs
    {
        public int MeskTemp { get; set; }
        public int MeskTime { get; set; }
        public int BoilTemp { get; set; }
        public int BoilTime { get; set; }
        public DateTime TimeReceived { get; set; }
    }
    public class MicroControllerAutomationProgressEventArgs : EventArgs
    {
        public int State { get; set; }
        public DateTime TimeReceived { get; set; }
    }
    internal interface IMicroControllerInterface
    {
        event EventHandler<MicroControllerTimeoutEventArgs> ControllerTimeout;
        event EventHandler<MicroControllerDataEventArgs> DataReceived;
        event EventHandler<MicroControllerHeaterEventArgs> HeaterStatusReceived;
        event EventHandler<MicroControllerPumpEventArgs> PumpStatusReceived;
        event EventHandler<MicroControllerTargetTempEventArgs> TargetStatusReceived;
        event EventHandler<MicroControllerTargetValueEventArgs> TargetValueReceived;
        event EventHandler<MicroControllerAutomationParametersEventArgs> AutomationParmetersReceived;
        event EventHandler<MicroControllerAutomationProgressEventArgs> AutomationProgressReceived;

        abstract void Start();
        abstract void Stop();
        abstract void SendData(MicroControllerCommands command, byte data);
        abstract void SendData(MicroControllerCommands command, byte[] data);
    }
}
