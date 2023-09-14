using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Services
{
    public interface IAutomationService
    {
        public abstract Task<bool> Start();
        public abstract Task<bool> Stop();
        public abstract void SetMeskTime(int time);
        public abstract void SetMeskTemp(int temp);
        public abstract void SetBoilTime(int time);
        public abstract void SetBoilTemp(int temp);
        public abstract int GetMeskTime();
        public abstract int GetMeskTemp();
        public abstract int GetBoilTime();
        public abstract int GetBoilTemp();
    }

    internal class AutomationService : IAutomationService
    {
        ICommandService _commandService;
        IConfigurationService _configurationService;

        public static readonly string MESKTIME = "MeskTimeKey";
        public static readonly string MESKTEMP = "MeskTempKey";
        public static readonly string BOILTIME = "BoilTimeKey";
        public static readonly string BOILTEMP = "BoilTempKey";

        int _meskTime = 60, _meskTemp = 60, _boilTime = 60, _boilTemp = 100;
        bool _automationActive = false;

        public AutomationService(ICommandService commandService, IConfigurationService configurationService)
        {
            _commandService = commandService;
            _configurationService = configurationService;

            // Set default values if no configuration is avaiable
            if (!_configurationService.Exists(MESKTIME))
                _configurationService.SetInt(MESKTIME, _meskTime);
            if (!_configurationService.Exists(MESKTEMP))
                _configurationService.SetInt(MESKTEMP, _meskTemp);
            if (!_configurationService.Exists(BOILTIME))
                _configurationService.SetInt(BOILTIME, _boilTime);
            if (!_configurationService.Exists(BOILTEMP))
                _configurationService.SetInt(BOILTEMP, _boilTemp);

            // Load configurations
            _meskTime = _configurationService.GetInt(MESKTIME);
            _meskTemp = _configurationService.GetInt(MESKTEMP);
            _boilTime = _configurationService.GetInt(BOILTIME);
            _boilTemp = _configurationService.GetInt(BOILTEMP);
        }


        public async Task<bool> Start()
        {
            if (_automationActive) return true;
            bool active = await _commandService.SetAutomationParameters(_meskTemp, _meskTime, _boilTemp, _boilTime);
            active &= await _commandService.StartAutomationProgram(true);

            return active;
        }

        public async Task<bool> Stop()
        {
            if (!_automationActive) return false;
            bool active = await _commandService.StartAutomationProgram(false);
            return !active;
        }

        public void SetMeskTime(int time)
        {
            _meskTime = time;
            _configurationService.SetInt(MESKTIME, _meskTime);
        }
        public void SetMeskTemp(int temp)
        {
            _meskTemp = temp;
            _configurationService.SetInt(MESKTEMP, _meskTemp);
        }
        public void SetBoilTime(int time)
        {
            _boilTime = time;
            _configurationService.SetInt(BOILTIME, _boilTime);
        }
        public void SetBoilTemp(int temp)
        {
            _boilTemp = temp;
            _configurationService.SetInt(BOILTEMP, _boilTemp);
        }
        public int GetMeskTime()
        {
            return _meskTime;
        }
        public int GetMeskTemp()
        {
            return _meskTemp;
        }
        public int GetBoilTime()
        {
            return _boilTime;
        }
        public int GetBoilTemp()
        {
            return _boilTemp;
        }
    }
}
