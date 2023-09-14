using BeerBrewerApp.ViewModels;

namespace BeerBrewerApp.Views;

public partial class MonitorView : ContentPage
{
	public MonitorView(MonitorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}