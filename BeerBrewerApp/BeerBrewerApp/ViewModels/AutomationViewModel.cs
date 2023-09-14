using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeerBrewerApp.Helpers;
using BeerBrewerApp.Services;

namespace BeerBrewerApp.ViewModels
{
    public enum AutomationGraphIndex : int
    {
        InitialTemp = 0,
        MeskStart = 1,
        MeskEnd = 2,
        BoilStart = 3,
        BoilEnd = 4,
        FinishTemp = 5
    }
    public partial class AutomationViewModel : ObservableObject
    {
        private IAlertService _alertService;
        private IAutomationService _automationService;

        /*################### Chart Parameters #########################*/
        private ObservableCollection<ObservablePoint> _observableValues;
        public ObservableCollection<ISeries> Series { get; private set; }
        public Axis[] XAxes { get; private set; }
        public Axis[] YAxes { get; private set; }
        private int _strokeSize = 5;

        /*################# Automation Parameters ######################*/
        [ObservableProperty]
        private string _meskTempString;
        [ObservableProperty]
        private string _meskTimeString;
        [ObservableProperty]
        private string _boilTempString;
        [ObservableProperty]
        private string _boilTimeString;
        [ObservableProperty]
        private string _startAutomationProgramString;
        private int _meskTemp;
        private int _meskTime;
        private int _boilTemp;
        private int _boilTime;
        private int _transitionTime;
        private bool _automationProgramStarted;
        public ICommand IncrementMeskTempCommand { get; private set; }
        public ICommand IncrementMeskTimeCommand { get; private set; }
        public ICommand IncrementBoilTempCommand { get; private set; }
        public ICommand IncrementBoilTimeCommand { get; private set; }
        public ICommand DecrementMeskTempCommand { get; private set; }
        public ICommand DecrementMeskTimeCommand { get; private set; }
        public ICommand DecrementBoilTempCommand { get; private set; }
        public ICommand DecrementBoilTimeCommand { get; private set; }
        public ICommand ManualSetMeskTempCommand { get; private set; }
        public ICommand ManualSetMeskTimeCommand { get; private set; }
        public ICommand ManualSetBoilTempCommand { get; private set; }
        public ICommand ManualSetBoilTimeCommand { get; private set; }
        public ICommand AutomationProgramStartCommand { get; private set; }

        /*################# Automation Progress ######################*/

        [ObservableProperty]
        string _heatingMeskPath;
        [ObservableProperty]
        string _meskingPath;
        [ObservableProperty]
        string _heatingBoilPath;
        [ObservableProperty]
        string _boilingPath;
        [ObservableProperty]
        string _coolingPath;
        [ObservableProperty]
        bool _heatingMeskActive;
        [ObservableProperty]
        bool _meskingActive;
        [ObservableProperty]
        bool _heatingBoilActive;
        [ObservableProperty]
        bool _boilingActive;
        [ObservableProperty]
        bool _coolingActive;

        public AutomationViewModel(IAlertService alertService, IAutomationService automationService)
        {
            // Services
            _alertService = alertService;
            _automationService = automationService;

            // Chart
            _observableValues = new ObservableCollection<ObservablePoint>();
            XAxes = new Axis[]
            {
                new Axis
                {
                    //Name = "X Axis",
                    NamePaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)),
                    LabelsPaint = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Tertiary)),
                    TextSize = 10,
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
                new LineSeries<ObservablePoint>
                {
                    Values = _observableValues,
                    LineSmoothness = 0,
                    GeometrySize = 22,
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null,
                    Stroke = new SolidColorPaint(ColorConverter.ConvertHexToSkColor(StaticParameters.Primary)) {
                        StrokeThickness = _strokeSize,
                        StrokeCap = SKStrokeCap.Round,
                        PathEffect = new DashEffect( new float[2] {3 * _strokeSize, 2 * _strokeSize })
                    }
                }
            };

            // Automation Parameters
            _meskTemp = _automationService.GetMeskTemp();
            _meskTime = _automationService.GetMeskTime();
            _boilTemp = _automationService.GetBoilTemp();
            _boilTime = _automationService.GetBoilTime();
            _transitionTime = 20;
            _meskTempString = _meskTemp.ToString();
            _meskTimeString = _meskTime.ToString();
            _boilTempString = _boilTemp.ToString();
            _boilTimeString = _boilTime.ToString();
            _startAutomationProgramString = "Start!";
            _automationProgramStarted = false;
            IncrementMeskTempCommand = new RelayCommand(IncrementMeskTemp);
            IncrementMeskTimeCommand = new RelayCommand(IncrementMeskTime);
            IncrementBoilTempCommand = new RelayCommand(IncrementBoilTemp);
            IncrementBoilTimeCommand = new RelayCommand(IncrementBoilTime);
            DecrementMeskTempCommand = new RelayCommand(DecrementMeskTemp);
            DecrementMeskTimeCommand = new RelayCommand(DecrementMeskTime);
            DecrementBoilTempCommand = new RelayCommand(DecrementBoilTemp);
            DecrementBoilTimeCommand = new RelayCommand(DecrementBoilTime);
            ManualSetMeskTempCommand = new RelayCommand(ManualSetMeskTemp);
            ManualSetMeskTimeCommand = new RelayCommand(ManualSetMeskTime);
            ManualSetBoilTempCommand = new RelayCommand(ManualSetBoilTemp);
            ManualSetBoilTimeCommand = new RelayCommand(ManualSetBoilTime);
            AutomationProgramStartCommand = new AsyncRelayCommand(StartAutomationProgram);

            // Default mesk model
            _observableValues.Add(new ObservablePoint(0, 0)); // Initial Temp
            _observableValues.Add(new ObservablePoint(_transitionTime, _meskTemp)); // Mesk Start
            _observableValues.Add(new ObservablePoint(_transitionTime + _meskTime, _meskTemp)); // Mesk End
            _observableValues.Add(new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp)); // Boil Start
            _observableValues.Add(new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp)); // Boil End
            _observableValues.Add(new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0)); // End

            // Configure initial automation icon paths
            HeatingMeskPath = "Resources/Images/automation_heating_inactive.png";
            MeskingPath = "Resources/Images/automation_mesking_inactive.png";
            HeatingBoilPath = "Resources/Images/automation_heating_inactive.png";
            BoilingPath = "Resources/Images/automation_boiling_inactive.png";
            CoolingPath = "Resources/Images/automation_cooling_inactive.png";
            HeatingMeskActive = false;
            MeskingActive = false;
            HeatingBoilActive = false;
            BoilingActive = false;
            CoolingActive = false;
        }

        public void IncrementMeskTemp()
        {
            int newTemp = _meskTemp + 1;
            if (newTemp < 0 || newTemp > 110) return;

            _meskTemp = newTemp;
            _automationService.SetMeskTemp(_meskTemp);

            _observableValues[(int)AutomationGraphIndex.MeskStart] = new ObservablePoint(_transitionTime, _meskTemp);
            _observableValues[(int)AutomationGraphIndex.MeskEnd] = new ObservablePoint(_transitionTime + _meskTime, _meskTemp);
            MeskTempString = _meskTemp.ToString();
        }
        public void IncrementMeskTime()
        {
            int newTime = _meskTime + 1;
            if (newTime < 0) return;

            _meskTime = newTime;
            _automationService.SetMeskTime(_meskTime);

            _observableValues[(int)AutomationGraphIndex.MeskEnd] = new ObservablePoint(_transitionTime + _meskTime, _meskTemp);
            _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.FinishTemp] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0);
            MeskTimeString = _meskTime.ToString();
        }
        public void IncrementBoilTemp()
        {
            int newTemp = _boilTemp + 1;
            if (newTemp < 0 || newTemp > 110) return;

            _boilTemp = newTemp;
            _automationService.SetBoilTemp(_boilTemp);

            _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
            BoilTempString = _boilTemp.ToString();
        }
        public void IncrementBoilTime()
        {
            int newTime = _boilTime + 1;
            if (newTime < 0) return;

            _boilTime = newTime;
            _automationService.SetBoilTime(_boilTime);

            _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.FinishTemp] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0);
            BoilTimeString = _boilTime.ToString();
        }
        public void DecrementMeskTemp()
        {
            int newTemp = _meskTemp - 1;
            if (newTemp < 0 || newTemp > 110) return;

            _meskTemp = newTemp;
            _automationService.SetMeskTemp(_meskTemp);

            _observableValues[(int)AutomationGraphIndex.MeskStart] = new ObservablePoint(_transitionTime, _meskTemp);
            _observableValues[(int)AutomationGraphIndex.MeskEnd] = new ObservablePoint(_transitionTime + _meskTime, _meskTemp);
            MeskTempString = _meskTemp.ToString();
        }
        public void DecrementMeskTime()
        {
            int newTime = _meskTime - 1;
            if (newTime < 0) return;

            _meskTime = newTime;
            _automationService.SetMeskTime(_meskTime);

            _observableValues[(int)AutomationGraphIndex.MeskEnd] = new ObservablePoint(_transitionTime + _meskTime, _meskTemp);
            _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.FinishTemp] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0);
            MeskTimeString = _meskTime.ToString();
        }
        public void DecrementBoilTemp()
        {
            int newTemp = _boilTemp - 1;
            if (newTemp < 0 || newTemp > 110) return;

            _boilTemp = newTemp;
            _automationService.SetBoilTemp(_boilTemp);

            _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
            BoilTempString = _boilTemp.ToString();
        }
        public void DecrementBoilTime()
        {
            int newTime = _boilTime - 1;
            if (newTime < 0) return;

            _boilTime = newTime;
            _automationService.SetBoilTime(_boilTime);

            _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
            _observableValues[(int)AutomationGraphIndex.FinishTemp] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0);
            BoilTimeString = _boilTime.ToString();
        }
        public void ManualSetMeskTemp() { _alertService.ShowPopup("Set", "Set target temperature:", ManualSetMeskTempApply); }
        public void ManualSetMeskTime() { _alertService.ShowPopup("Set", "Set target temperature:", ManualSetMeskTimeApply); }
        public void ManualSetBoilTemp() { _alertService.ShowPopup("Set", "Set target temperature:", ManualSetBoilTempApply); }
        public void ManualSetBoilTime() { _alertService.ShowPopup("Set", "Set target temperature:", ManualSetBoilTimeApply); }
        [RelayCommand]
        public void ManualSetMeskTempApply(string val)
        {
            if (val == null) return;
            int value;
            bool success = int.TryParse(val, out value);
            if (success && value <= 110 && value > 0)
            {
                _meskTemp = value;
                _automationService.SetMeskTemp(_meskTemp);

                _observableValues[(int)AutomationGraphIndex.MeskStart] = new ObservablePoint(_transitionTime, _meskTemp);
                _observableValues[(int)AutomationGraphIndex.MeskEnd] = new ObservablePoint(_transitionTime + _meskTime, _meskTemp);
                MeskTempString = _meskTemp.ToString();
            }
            else
            {
                _alertService.ShowAlert("Error", "Non-valid temperature!\nPossible values range between 0 to 110 C");
            }
        }
        [RelayCommand]
        public void ManualSetMeskTimeApply(string val)
        {
            if (val == null) return;
            int value;
            bool success = int.TryParse(val, out value);
            if (success && value > 0)
            {
                _meskTime = value;
                _automationService.SetMeskTime(_meskTime);

                _observableValues[(int)AutomationGraphIndex.MeskEnd] = new ObservablePoint(_transitionTime + _meskTime, _meskTemp);
                _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
                _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
                _observableValues[(int)AutomationGraphIndex.FinishTemp] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0);
                MeskTimeString = _meskTime.ToString();
            }
            else
            {
                _alertService.ShowAlert("Error", "Non-valid temperature!\nPossible values range between 0 to 110 C");
            }
        }
        [RelayCommand]
        public void ManualSetBoilTempApply(string val)
        {
            if (val == null) return;
            int value;
            bool success = int.TryParse(val, out value);
            if (success && value <= 110 && value > 0)
            {
                _boilTemp = value;
                _automationService.SetBoilTemp(_boilTemp);

                _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
                _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
                BoilTempString = _boilTemp.ToString();
            }
            else
            {
                _alertService.ShowAlert("Error", "Non-valid temperature!\nPossible values range between 0 to 110 C");
            }
        }
        [RelayCommand]
        public void ManualSetBoilTimeApply(string val)
        {
            if (val == null) return;
            int value;
            bool success = int.TryParse(val, out value);
            if (success && value <= 110 && value > 0)
            {
                _boilTime = value;
                _automationService.SetBoilTime(_boilTime);

                _observableValues[(int)AutomationGraphIndex.BoilStart] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime, _boilTemp);
                _observableValues[(int)AutomationGraphIndex.BoilEnd] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime, _boilTemp);
                _observableValues[(int)AutomationGraphIndex.FinishTemp] = new ObservablePoint(_transitionTime + _meskTime + _transitionTime + _boilTime + _transitionTime, 0);
                BoilTimeString = _boilTime.ToString();
            }
            else
            {
                _alertService.ShowAlert("Error", "Non-valid temperature!\nPossible values range between 0 to 110 C");
            }
        }

        public async Task StartAutomationProgram()
        {
            if (!_automationProgramStarted)
            {
                _automationProgramStarted = await _automationService.Start();
                StartAutomationProgramString = "Stop!";
                Debug.WriteLine("Error", "Starting Automation");
            }
            else
            {
                _automationProgramStarted = await _automationService.Stop();
                StartAutomationProgramString = "Start!";
                Debug.WriteLine("Error", "Stopping Automation");
            }
        }
    }
}
