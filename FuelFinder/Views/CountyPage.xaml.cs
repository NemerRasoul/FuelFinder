using FuelFinder.ViewModels;
using System.Threading.Tasks;

namespace FuelFinder.Views;

public partial class CountyPage : ContentPage
{
	public CountyPage()
	{
		InitializeComponent();
	}

    private async void OnCountySelected(object sender, SelectionChangedEventArgs e)
    {
		var selected = e.CurrentSelection.FirstOrDefault() as County;

		if (selected != null) 
		{
			var vm = BindingContext as MainViewModel;
			if (vm != null) 
			{
				vm.SelectedCounty = selected;
			}

			await Navigation.PopAsync();
		}

		//((CollectionView)sender).SelectedItem = null;
    }
}