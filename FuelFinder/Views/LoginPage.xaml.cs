using FuelFinder.Application.Interfaces;
using FuelFinder.Application.Services;


namespace FuelFinder.Views;

public partial class LoginPage : ContentPage
{
    private readonly IUserService _userService;
    private readonly AppShell _appShell;

    public LoginPage(IUserService userService, AppShell appShell)
    {
        InitializeComponent();
        _userService = userService;
        _appShell = appShell;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool success = await _userService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);
        if (success)
        {
            _appShell.BindingContext = _userService.CurrentUser;
            Microsoft.Maui.Controls.Application.Current.MainPage = _appShell;
        }
        else
        {
            ErrorLabel.Text = "Fel användarnamn eller lösenord. Försök igen.";
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var (success, message) = await _userService.RegisterAsync(UsernameEntry.Text, PasswordEntry.Text);
        if (success)
        {
            await DisplayAlert("Välkommen!", "Ditt konto har skapats!", "OK");
            _appShell.BindingContext = _userService.CurrentUser;
            Microsoft.Maui.Controls.Application.Current.MainPage = _appShell;
        }
        else
        {
            ErrorLabel.Text = message;
        }
    }
}