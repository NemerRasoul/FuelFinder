using FuelFinder.Application.Interfaces;
using FuelFinder.Application.Services;
using FuelFinder.Domain.Interfaces;
using FuelFinder.Infrastructure.Repositories;
using FuelFinder.ViewModels;
using FuelFinder.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

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
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MapPage>();



            var connectionString = builder.Configuration.GetConnectionString("MongoDB");
            // ==================================================== ny här

            
            // När någon ber om IUserRepository ge dem UserRepository-klassen
            builder.Services.AddSingleton<IUserRepository>(new UserRepository(connectionString));


            builder.Services.AddSingleton<IFuelStorage>(
            new FuelRepository(fileName => FileSystem.OpenAppPackageFileAsync(fileName))
            );

            //   APPLICATION 
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IFuelService, FuelService>();

            // builder.Services.AddSingleton<IWeatherService, WeatherService>(); 

            // Använder en singleton instans
            builder.Services.AddSingleton<IWeatherService>(WeatherService.Instance);

            builder.Services.AddSingleton<ITrafficService, TrafficService>();

            //   VIEWMODELS
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<TrafficViewModel>();
            builder.Services.AddTransient<CalculatorViewModel>();

            //   VIEWS 
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
