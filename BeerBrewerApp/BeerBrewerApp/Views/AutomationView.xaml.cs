using BeerBrewerApp.ViewModels;

namespace BeerBrewerApp.Views;

public partial class AutomationView : ContentPage
{
	public AutomationView(AutomationViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}