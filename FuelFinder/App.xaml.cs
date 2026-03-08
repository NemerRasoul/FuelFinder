using FuelFinder.Views;


namespace FuelFinder
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly LoginPage _loginPage;
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            _loginPage = loginPage;
            //MainPage = new NavigationPage(loginPage);
        }

       protected override Window CreateWindow(IActivationState? activationState)
       {
            //return new Window(new AppShell());
            return new Window(new NavigationPage(_loginPage));

        }
    }
}