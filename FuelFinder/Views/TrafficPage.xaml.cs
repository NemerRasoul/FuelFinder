using FuelFinder.ViewModels;

namespace FuelFinder.Views;

public partial class TrafficPage : ContentPage
{
	public TrafficPage(TrafficViewModel trafficViewModel)
	{
		InitializeComponent();
		BindingContext = trafficViewModel;
    }
}