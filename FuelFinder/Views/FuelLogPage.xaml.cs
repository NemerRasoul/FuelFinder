using FuelFinder.Application.Interfaces;
using FuelFinder.Domain.Entities.Models;
using FuelFinder.ViewModels;
using System.Collections.ObjectModel;

namespace FuelFinder.Views;

public partial class FuelLogPage : ContentPage
{
	private readonly IUserService _userService;
    private readonly MainViewModel _mainViewModel;
    public FuelLogPage(IUserService userService, MainViewModel mainViewModel)
	{
		InitializeComponent();
        _userService = userService;
        _mainViewModel = mainViewModel;

        if (_userService.CurrentUser != null)
        {
            LogCollection.ItemsSource = _userService.CurrentUser.FuelLogs;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_userService.CurrentUser == null)
        {
            await DisplayAlert("Inloggning krävs",
                "OBS! Du måste vara inloggad för att använda bränsleloggen. VARNING: Om du försöker använda bränsleloggen utan inlogg kommer Appen sluta fungera.",
                "OK");
           
        }
    }

    private async void OnSaveLogClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(StationEntry.Text) ||
            !double.TryParse(LitersEntry.Text, out double liters) ||
            !double.TryParse(OdometerEntry.Text, out double odometer))
        {
            await DisplayAlert("Fel", "Vänligen fyll i alla fält korrekt.", "OK");
            return;
        }

        var newLog = new FuelLog
        {
            StationName = StationEntry.Text,
            Liters = liters,
            Odometer = odometer,
            FuelType = _mainViewModel.SelectedFuelType

        };

        await _userService.AddFuelLogAsync(newLog);

       

        StationEntry.Text = string.Empty;
        LitersEntry.Text = string.Empty;
        OdometerEntry.Text = string.Empty;

        LogCollection.ItemsSource = null;
        LogCollection.ItemsSource = _userService.CurrentUser.FuelLogs;

        await DisplayAlert("Sparat!", $"Loggen har sparats. Du fick 10 poäng!", "OK");


    }
}