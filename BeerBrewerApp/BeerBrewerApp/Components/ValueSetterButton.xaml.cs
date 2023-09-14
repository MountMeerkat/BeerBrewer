using System.Windows.Input;

namespace BeerBrewerApp.Components;

public partial class ValueSetterButton : ContentView
{
    public static readonly BindableProperty SetterTitleProperty = BindableProperty.Create(nameof(SetterTitle), typeof(string), typeof(ValueSetterButton), string.Empty);
    public static readonly BindableProperty SetterTypeProperty = BindableProperty.Create(nameof(SetterType), typeof(string), typeof(ValueSetterButton), string.Empty);
    public static readonly BindableProperty SetterValueProperty = BindableProperty.Create(nameof(SetterValue), typeof(string), typeof(ValueSetterButton), string.Empty);
    public static readonly BindableProperty IncrementCommandProperty = BindableProperty.Create(nameof(IncrementCommand), typeof(ICommand), typeof(ValueSetterButton));
    public static readonly BindableProperty DecrementCommandProperty = BindableProperty.Create(nameof(DecrementCommand), typeof(ICommand), typeof(ValueSetterButton));
    public static readonly BindableProperty ManualSetCommandProperty = BindableProperty.Create(nameof(ManualSetCommand), typeof(ICommand), typeof(ValueSetterButton));

    public string SetterTitle
    {
        get => (string)GetValue(SetterTitleProperty);
        set => SetValue(SetterTitleProperty, value);
    }
    public string SetterType
    {
        get => (string)GetValue(SetterTypeProperty);
        set => SetValue(SetterTypeProperty, value);
    }
    public string SetterValue
    {
        get => (string)GetValue(SetterValueProperty);
        set => SetValue(SetterValueProperty, value);
    }
    public ICommand IncrementCommand
    {
        get => (ICommand)GetValue(IncrementCommandProperty);
        set => SetValue(IncrementCommandProperty, value);
    }
    public ICommand DecrementCommand
    {
        get => (ICommand)GetValue(DecrementCommandProperty);
        set => SetValue(DecrementCommandProperty, value);
    }
    public ICommand ManualSetCommand
    {
        get => (ICommand)GetValue(ManualSetCommandProperty);
        set => SetValue(ManualSetCommandProperty, value);
    }
    public ValueSetterButton()
	{
		InitializeComponent();
	}
}