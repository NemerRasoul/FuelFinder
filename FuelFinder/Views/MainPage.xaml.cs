using FuelFinder.ViewModels;
using FuelFinder.Views;

namespace FuelFinder.Views;

public partial class MainPage : ContentPage
{
  

    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = mainViewModel;
    }
    private async void OnSelectCountyClicked(object sender, EventArgs e)
    {
        var countyPage = new CountyPage();
      
        countyPage.BindingContext = this.BindingContext;

        await Navigation.PushAsync(countyPage);
    }
}
