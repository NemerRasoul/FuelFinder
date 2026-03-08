using FuelFinder.ViewModels;

namespace FuelFinder.Views;

public partial class CalculatorPage : ContentPage
{
	public CalculatorPage(MainViewModel mainViewModel)
	{
		InitializeComponent();
		BindingContext = new CalculatorViewModel(mainViewModel);
    }
}