using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeerBrewerApp.Services;
using BeerBrewerApp.Helpers;

namespace BeerBrewerApp.ViewModels
{
    public partial class MonitorViewModel : ObservableObject
    {
        /*################### Filter Parameters ########################*/
        // Intervall between temperature measurements culling in minutes
        private int TemperatureDataCullingIntervall = 10;
        // Ration of culling (e.g. 1:Ration)
        private int TemperatureDataCullingRation = 10;
        // Extremum threshold for data reduction filter
        private int TemperatureExtremumThreshold = 5;
        private DateTime TemperatureLastFilterTime = DateTime.Now;

        /*################### Data Generator ###########################*/
        //private IMicroControllerInterface microControllerInterface;

        /*################### Chart Parameters #########################*/
        private ObservableCollection<DateTimePoint> _observableValues;
        public ObservableCollection<ISeries> Series { get; private set; }
        public Axis[] XAxes { get; private set; }
        public Axis[] YAxes { get; private set; }

        /*############# Temperature Control Parameters #################*/
        [ObservableProperty]
        private string _currentTemperatureString;
        [ObservableProperty]
        private string _currentTargetTemperatureString;
        private int CurrentTargetTemperature;
        private IAlertService _alertService;
        private ICommandService _commandService;
        public ICommand IncrementSetCommand { get; private set; }
        public ICommand DecrementSetCommand { get; private set; }
        public ICommand ManualSetCommand { get; private set; }

        /*############## Quick Functions Parameters ###################*/
        [ObservableProperty]
        string _automationImagePath;
        [ObservableProperty]
        string _heaterImagePath;
        [ObservableProperty]
        string _pumpImagePath;
        [ObservableProperty]
        string _targetTempImagePath;
        private bool _automation;
        private bool _heater;
        private bool _pump;
        private bool _targetTemp;
        public ICommand AutomationToggleCommand { get; private set; }
        public ICommand HeaterToggleCommand { get; private set; }
        public ICommand PumpToggleCommand { get; private set; }
        public ICommand TargetTempToggleCommand { get; private set; }

        public MonitorViewModel(IAlertService alertService, ICommandService commandService)
        {
            _alertService = alertService;
            _commandService = commandService;

            _observableValues = new ObservableCollection<DateTimePoint>();
            _currentTemperatureString = "##";
            _currentTargetTemperatureString = "60";
            CurrentTargetTemperature = 60;

            _automation = false;
            AutomationImagePath = "Resources/Images/robot_inactive2.png";
            _heater = false;
            HeaterImagePath = "Resources/Images/fire_inactive2.png";
            _pump = false;
            PumpImagePath = "Resources/Images/pump_inactive2.png";
            _targetTemp = false;
            TargetTempImagePath = "Resources/Images/dial_inactive.png";
            AutomationToggleCommand = new RelayCommand(AutomationToggle);
            HeaterToggleCommand = new AsyncRelayCommand(HeaterToggle);
            PumpToggleCommand = new AsyncRelayCommand(PumpToggle);
            TargetTempToggleCommand = new AsyncRelayCommand(SetToggle);
            IncrementSetCommand = new AsyncRelayCommand(IncrementSet);
            DecrementSetCommand = new AsyncRelayCommand(DecrementSet);
            ManualSetCommand = new RelayCommand(OpenManualSetPopup);
            XAxes = new Axis[]
            {
                new Axis
                {
                    //Name = "X Axis",
                    NamePaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)),
                    LabelsPaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)),
                    TextSize = 10,
                    Labeler = value => value.AsDate().ToString("HH:mm:ss"),
                    UnitWidth = TimeSpan.FromMinutes(1).Ticks,
                    MinStep = TimeSpan.FromMinutes(1).Ticks,
                    SeparatorsPaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)) {StrokeThickness = 2}
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    //Name = "Y Axis",
                    NamePaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)),
                    LabelsPaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)),
                    TextSize = 10,
                    MaxLimit = 110,
                    MinLimit = 0,
                    SeparatorsPaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)) {StrokeThickness = 2}
                }
            };

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    Values = _observableValues,
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null,
                    Stroke = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Primary)) {StrokeThickness = 2}
                }
            };
            _commandService.NewTemperatureData += HandleNewData;
        }

        private void HandleNewData(object sender, MicroControllerDataEventArgs args)
        {
            float[] temps = { args.TempA, args.TempB, args.TempC };
            CurrentTemperatureString = String.Format("{0:N1}", args.TempA);
            AddItem(temps);
        }

        [RelayCommand]
        public void AddItem(float[] items)
        {
            if (DateTime.Compare(TemperatureLastFilterTime.AddMinutes(TemperatureDataCullingIntervall), DateTime.Now) < 0)
            {
                ExtremumDataFilter();
                TemperatureLastFilterTime = DateTime.Now;
            }
            _observableValues.Add(new DateTimePoint(DateTime.Now, items[0]));
        }

        public async void AutomationToggle()
        {
            // TODO add pending icon
            // TODO handle shutdown timeout
            if (!_automation)
            {
                // Turn off all other control when automation is active
                if (_heater) await HeaterToggle();
                if (_pump) await PumpToggle();
                if (_targetTemp) await SetToggle();
            }
            _automation = !_automation;
            AutomationImagePath = _automation ? "Resources/Images/robot.png" : "Resources/Images/robot_inactive2.png";
        }

        public async Task HeaterToggle()
        {
            // Disable while automation is active
            if (_automation) return;
            bool success = await _commandService.SetHeaterActivation(!_heater);
            if (success)
            {
                _heater = !_heater;
                HeaterImagePath = _heater ? "Resources/Images/fire.png" : "Resources/Images/fire_inactive2.png";
            }
        }

        public async Task PumpToggle()
        {
            // Disable while automation is active
            if (_automation) return;
            bool success = await _commandService.SetPumpActivation(!_pump);
            if (success)
            {
                _pump = !_pump;
                PumpImagePath = _pump ? "Resources/Images/pump.png" : "Resources/Images/pump_inactive2.png";
            }
        }

        public async Task SetToggle()
        {
            // Disable while automation is active
            if (_automation) return;
            bool success = await _commandService.SetTargetTempActivation(!_targetTemp);
            if (success)
            {
                _targetTemp = !_targetTemp;
                TargetTempImagePath = _targetTemp ? "Resources/Images/dial.png" : "Resources/Images/dial_inactive.png";
            }
        }

        public async Task IncrementSet()
        {
            int newTemp = CurrentTargetTemperature + 1;
            bool success = await _commandService.SetTargetTempValue(newTemp);
            if (success)
            {
                CurrentTargetTemperature = newTemp;
                CurrentTargetTemperatureString = CurrentTargetTemperature.ToString();
            }
        }
        public async Task DecrementSet()
        {
            int newTemp = CurrentTargetTemperature - 1;
            bool success = await _commandService.SetTargetTempValue(newTemp);
            if (success)
            {
                CurrentTargetTemperature = newTemp;
                CurrentTargetTemperatureString = CurrentTargetTemperature.ToString();
            }
        }
        public void OpenManualSetPopup()
        {
            _alertService.ShowPopup("Set", "Set target temperature:", ManualSetApply);
        }
        [RelayCommand]
        public async void ManualSetApply(string setValue)
        {
            if (setValue == null) return;
            int value;
            bool success = int.TryParse(setValue, out value);
            if (success && value < 110 && value > 0)
            {
                int newTemp = value;
                bool setSuccess = await _commandService.SetTargetTempValue(newTemp);
                if (setSuccess)
                {
                    CurrentTargetTemperature = newTemp;
                    CurrentTargetTemperatureString = CurrentTargetTemperature.ToString();
                }
            }
            else
            {
                _alertService.ShowAlert("Error", "Non-valid temperature!\nPossible values range between 0 to 110 C");
            }
        }

        public void ExtremumDataFilter()
        {
            int preCount = _observableValues.Count;
            int postCount = 0;
            for (int removed = 0; removed < preCount; removed++)
            {
                if (removed % TemperatureDataCullingRation == 0 || (
                    Math.Abs((decimal)(_observableValues[postCount].Value - _observableValues[postCount - 1].Value)) > TemperatureExtremumThreshold))
                    postCount++;
                else
                    _observableValues.RemoveAt(postCount);
            }
        }
    }
}
