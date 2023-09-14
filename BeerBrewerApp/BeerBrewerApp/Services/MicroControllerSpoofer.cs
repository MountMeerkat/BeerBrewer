using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Services
{
    internal class MicroControllerSpoofer : IMicroControllerInterface
    {
        public event EventHandler<MicroControllerTimeoutEventArgs> ControllerTimeout;
        public event EventHandler<MicroControllerDataEventArgs> DataReceived;
        public event EventHandler<MicroControllerHeaterEventArgs> HeaterStatusReceived;
        public event EventHandler<MicroControllerPumpEventArgs> PumpStatusReceived;
        public event EventHandler<MicroControllerTargetTempEventArgs> TargetStatusReceived;
        public event EventHandler<MicroControllerTargetValueEventArgs> TargetValueReceived;
        public event EventHandler<MicroControllerAutomationProgressEventArgs> AutomationProgressReceived;
        public event EventHandler<MicroControllerAutomationParametersEventArgs> AutomationParmetersReceived;

        private bool _running;
        private Random _random;
        private Thread _dataSpooferThread;
        private float _dataCoreValue = 0;
        private int _dataIncrement = 2;
        private int _minResponseDelay = 10;
        private int _maxResponseDelay = 500;

        public MicroControllerSpoofer()
        {
            _running = false;
            _random = new Random();
            _dataSpooferThread = new Thread(new ThreadStart(IncrementalSampleDataGenerator));
        }

        public void Start()
        {
            _running = true;
            _dataSpooferThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _dataSpooferThread.Join();
        }

        private void IncrementalSampleDataGenerator()
        {
            while (_running)
            {
                if (_dataCoreValue > 100)
                    _dataIncrement = Math.Abs(_dataIncrement) * -1;
                else if (_dataCoreValue < 0)
                    _dataIncrement = Math.Abs(_dataIncrement);
                _dataCoreValue = _dataCoreValue + (float)_random.NextDouble() * (_dataIncrement);
                float b = 0;
                float c = 0;
                MicroControllerDataEventArgs args = new MicroControllerDataEventArgs
                {
                    TempA = _dataCoreValue,
                    TempB = b,
                    TempC = c,
                    TimeReceived = DateTime.Now
                };
                EventHandler<MicroControllerDataEventArgs> handler = DataReceived;
                if (handler != null)
                {
                    handler(this, args);
                }
                Thread.Sleep(1000);
            }
        }

        private void SampleDataGenerator()
        {
            while (_running)
            {
                float a = _random.Next(10, 1000) / 10;
                float b = a + _random.Next(1, 10);
                float c = a - _random.Next(1, 10);
                MicroControllerDataEventArgs args = new MicroControllerDataEventArgs
                {
                    TempA = a,
                    TempB = b,
                    TempC = c,
                    TimeReceived = DateTime.Now
                };
                EventHandler<MicroControllerDataEventArgs> handler = DataReceived;
                if (handler != null)
                {
                    handler(this, args);
                }
                Thread.Sleep(1000);
            }
        }

        public void SendData(MicroControllerCommands command, byte data)
        {
            switch (command)
            {
                case MicroControllerCommands.SetHeaterPower:
                    (new Thread(() =>
                    {
                        Thread.Sleep(_random.Next(_minResponseDelay, _maxResponseDelay));
                        HeaterStatusReceived?.Invoke(this, new MicroControllerHeaterEventArgs { HeaterActive = Convert.ToBoolean(data), TimeReceived = DateTime.Now });
                    })).Start();
                    break;
                case MicroControllerCommands.SetPumpPower:
                    (new Thread(() =>
                    {
                        Thread.Sleep(_random.Next(_minResponseDelay, _maxResponseDelay));
                        PumpStatusReceived?.Invoke(this, new MicroControllerPumpEventArgs { PumpActive = Convert.ToBoolean(data), TimeReceived = DateTime.Now });
                    })).Start();
                    break;
                case MicroControllerCommands.SetTargetTemp:
                    (new Thread(() =>
                    {
                        Thread.Sleep(_random.Next(_minResponseDelay, _maxResponseDelay));
                        TargetStatusReceived?.Invoke(this, new MicroControllerTargetTempEventArgs { TargetActive = Convert.ToBoolean(data), TimeReceived = DateTime.Now });
                    })).Start();
                    break;
                case MicroControllerCommands.SetTargetValue:
                    (new Thread(() =>
                    {
                        Thread.Sleep(_random.Next(_minResponseDelay, _maxResponseDelay));
                        TargetValueReceived?.Invoke(this, new MicroControllerTargetValueEventArgs { TargetTemp = Convert.ToInt32(data), TimeReceived = DateTime.Now });
                    })).Start();
                    break;
                case MicroControllerCommands.SetAutomationActivation:
                    (new Thread(() =>
                    {
                        Thread.Sleep(_random.Next(_minResponseDelay, _maxResponseDelay));
                        AutomationProgressReceived?.Invoke(this, new MicroControllerAutomationProgressEventArgs { State = Convert.ToInt32(data), TimeReceived = DateTime.Now });
                    })).Start();
                    break;
            }
        }

        public void SendData(MicroControllerCommands command, byte[] data)
        {
            switch (command)
            {
                case MicroControllerCommands.SetAutomationParameters:
                    (new Thread(() =>
                    {
                        Thread.Sleep(_random.Next(_minResponseDelay, _maxResponseDelay));
                        AutomationParmetersReceived?.Invoke(this, new MicroControllerAutomationParametersEventArgs
                        {
                            MeskTemp = Convert.ToInt32(data[0]),
                            MeskTime = Convert.ToInt32(data[1]),
                            BoilTemp = Convert.ToInt32(data[2]),
                            BoilTime = Convert.ToInt32(data[3]),
                            TimeReceived = DateTime.Now
                        });
                    })).Start();
                    break;
            }
        }
    }
}
