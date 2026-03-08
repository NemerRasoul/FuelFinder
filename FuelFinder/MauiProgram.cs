using FuelFinder.Application.Interfaces;
using FuelFinder.Domain.Interfaces;
using FuelFinder.Infrastructure.Repositories;
using FuelFinder.Application.Services;
using FuelFinder.ViewModels;

using FuelFinder.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FuelFinder
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Configuration.AddUserSecrets<App>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });



            var connectionString = builder.Configuration.GetConnectionString("MongoDB");
            // ==================================================== ny här

            
            // Vi säger: "När någon ber om IUserRepository, ge dem UserRepository-klassen"
            builder.Services.AddSingleton<IUserRepository>(new UserRepository(connectionString));


            builder.Services.AddSingleton<IFuelStorage>(
            new FuelRepository(fileName => FileSystem.OpenAppPackageFileAsync(fileName))
            );

            // 3. Registrera APPLICATION (Tjänster/Logik)
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IFuelService, FuelService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<ITrafficService, TrafficService>();

            // 4. Registrera VIEWMODELS
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<TrafficViewModel>();
            builder.Services.AddTransient<CalculatorViewModel>();

            // 5. Registrera VIEWS (Sidor)
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<TrafficPage>();
            builder.Services.AddTransient<CalculatorPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<CountyPage>();

            builder.Services.AddSingleton<AppShell>();


            // =================================================== gammalt neråt
            /*

             builder.Services.AddSingleton(new MongoService(connectionString));

             builder.Services.AddSingleton<UserService>();
             builder.Services.AddSingleton<MainViewModel>();
             builder.Services.AddSingleton<MainPage>();
             builder.Services.AddTransient<TrafficPage>();
             builder.Services.AddTransient<CalculatorPage>();
             builder.Services.AddTransient<LoginPage>();

             */


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
