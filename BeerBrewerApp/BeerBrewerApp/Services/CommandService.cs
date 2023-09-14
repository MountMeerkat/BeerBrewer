using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Services
{
    public interface ICommandService
    {
        public event EventHandler<MicroControllerDataEventArgs> NewTemperatureData;
        public abstract Task<bool> SetHeaterActivation(bool active);
        public abstract Task<bool> SetPumpActivation(bool active);
        public abstract Task<bool> SetTargetTempActivation(bool active);
        public abstract Task<bool> SetTargetTempValue(int temp);
        public abstract Task<bool> StartAutomationProgram(bool active);
        public abstract Task<bool> SetAutomationParameters(int meskTemp, int meskTime, int boilTemp, int boilTime);
    }
    internal class CommandService : ICommandService
    {
        public event EventHandler<MicroControllerDataEventArgs> NewTemperatureData;
        private IMicroControllerInterface microControllerInterface;
        private bool HeaterActive = false;
        private SemaphoreSlim HeaterSignal;
        private bool PumpActive = false;
        private SemaphoreSlim PumpSignal;
        private bool TargetTempActive = false;
        private SemaphoreSlim TargetTempSignal;
        private int TargetTemp = 60;
        private SemaphoreSlim TargetValueSignal;
        private int MeskTemp = 0, MeskTime = 0, BoilTemp = 0, BoilTime = 0;
        private SemaphoreSlim AutomationParametersSignal;
        private int CommandTimeout = 2000;
        private bool AutomationActive = false;
        private SemaphoreSlim AutomationActiveSignal;
        public CommandService()
        {
            microControllerInterface = new MicroControllerSpoofer();

            //Setup signals from microcontroller events
            HeaterSignal = new SemaphoreSlim(0, 1);
            PumpSignal = new SemaphoreSlim(0, 1);
            TargetTempSignal = new SemaphoreSlim(0, 1);
            TargetValueSignal = new SemaphoreSlim(0, 1);
            AutomationParametersSignal = new SemaphoreSlim(0, 1);
            AutomationActiveSignal = new SemaphoreSlim(0, 1);

            // Subscribe to microcontroller signals
            microControllerInterface.DataReceived += HandleNewData;
            microControllerInterface.HeaterStatusReceived += HandleHeaterStatus;
            microControllerInterface.PumpStatusReceived += HandlePumpStatus;
            microControllerInterface.TargetStatusReceived += HandleTargetTempStatus;
            microControllerInterface.TargetValueReceived += HandleTargetTempValue;

            // Start the microcontroller interface
            microControllerInterface.Start();
        }

        public async Task<bool> SetHeaterActivation(bool active)
        {
            microControllerInterface.SendData(MicroControllerCommands.SetHeaterPower, active ? (byte)0x01 : (byte)0x00);
            bool success = await HeaterSignal.WaitAsync(CommandTimeout);
            success &= HeaterActive == active;
            return success;
        }

        public async Task<bool> SetPumpActivation(bool active)
        {
            microControllerInterface.SendData(MicroControllerCommands.SetPumpPower, active ? (byte)0x01 : (byte)0x00);
            bool success = await PumpSignal.WaitAsync(CommandTimeout);
            success &= PumpActive == active;
            return success;
        }

        public async Task<bool> SetTargetTempActivation(bool active)
        {
            microControllerInterface.SendData(MicroControllerCommands.SetTargetTemp, active ? (byte)0x01 : (byte)0x00);
            bool success = await TargetTempSignal.WaitAsync(CommandTimeout);
            success &= TargetTempActive == active;
            return success;
        }

        public async Task<bool> SetTargetTempValue(int temp)
        {
            byte byteTemp;
            bool convertSuccessful = byte.TryParse(temp.ToString(), out byteTemp);
            if (!convertSuccessful)
                throw new InvalidDataException();

            microControllerInterface.SendData(MicroControllerCommands.SetTargetValue, byteTemp);
            bool success = await TargetValueSignal.WaitAsync(CommandTimeout);
            success &= TargetTemp == temp;

            return success;
        }

        public async Task<bool> StartAutomationProgram(bool active)
        {
            microControllerInterface.SendData(MicroControllerCommands.SetAutomationActivation, active ? (byte)0x01 : (byte)0x00);
            bool success = await AutomationActiveSignal.WaitAsync(CommandTimeout);
            success &= AutomationActive == active;
            return success;
        }

        public async Task<bool> SetAutomationParameters(int meskTemp, int meskTime, int boilTemp, int boilTime)
        {
            byte[] automationParameters = new byte[4];
            bool convertSuccessful = byte.TryParse(meskTemp.ToString(), out automationParameters[0]);
            convertSuccessful &= byte.TryParse(meskTime.ToString(), out automationParameters[1]);
            convertSuccessful &= byte.TryParse(boilTemp.ToString(), out automationParameters[2]);
            convertSuccessful &= byte.TryParse(boilTime.ToString(), out automationParameters[3]);
            if (!convertSuccessful)
                throw new InvalidDataException();

            microControllerInterface.SendData(MicroControllerCommands.SetAutomationParameters, automationParameters);
            bool success = await AutomationParametersSignal.WaitAsync(CommandTimeout);
            success &= meskTemp == MeskTemp;
            success &= meskTime == MeskTime;
            success &= boilTemp == BoilTemp;
            success &= boilTime == BoilTime;

            return success;
        }

        void HandleNewData(object sender, MicroControllerDataEventArgs args)
        {
            EventHandler<MicroControllerDataEventArgs> handler = NewTemperatureData;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        void HandleHeaterStatus(object sender, MicroControllerHeaterEventArgs args)
        {
            try
            {
                HeaterSignal.Release();
                HeaterActive = args.HeaterActive;
            }
            catch (SemaphoreFullException e) // Request timeout
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandlePumpStatus(object sender, MicroControllerPumpEventArgs args)
        {
            try
            {
                PumpSignal.Release();
                PumpActive = args.PumpActive;
            }
            catch (SemaphoreFullException e) // Request timeout
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandleTargetTempStatus(object sender, MicroControllerTargetTempEventArgs args)
        {
            try
            {
                TargetTempSignal.Release();
                TargetTempActive = args.TargetActive;
            }
            catch (SemaphoreFullException e) // Request timeout
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandleTargetTempValue(object sender, MicroControllerTargetValueEventArgs args)
        {
            try
            {
                TargetValueSignal.Release();
                TargetTemp = args.TargetTemp;
            }
            catch (SemaphoreFullException e) // Request timeout
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandleAutomationParameters(object sender, MicroControllerAutomationParametersEventArgs args)
        {
            try
            {
                AutomationParametersSignal.Release();
                MeskTemp = args.MeskTemp;
                MeskTime = args.MeskTime;
                BoilTemp = args.BoilTemp;
                BoilTime = args.BoilTime;
            }
            catch (SemaphoreFullException e) // Request timeout
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandleAutomationActivation(object sender, MicroControllerAutomationProgressEventArgs args)
        {
            try
            {
                AutomationActiveSignal.Release();
                AutomationActive = args.State != 0; //AutomationProgressStates.Off
            }
            catch (SemaphoreFullException e) // Request timeout
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
