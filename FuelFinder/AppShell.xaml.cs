using FuelFinder.Application.Services;

namespace FuelFinder
{
    public partial class AppShell : Shell
    {
        public AppShell()//UserService userService)
        {
            InitializeComponent();
            //BindingContext = userService.CurrentUser;
        }
    }
}
