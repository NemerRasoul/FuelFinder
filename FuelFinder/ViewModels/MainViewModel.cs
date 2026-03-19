using FuelFinder.Application.Interfaces;
using FuelFinder.Domain.Entities.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FuelFinder.ViewModels
{
    public class County
    {
        public string Name { get; set; }
        public string UrlName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
    }

    public class MainViewModel : BaseViewModel
    {
        private readonly IWeatherService _weatherService;
        private readonly IFuelService _fuelService;

        private string _selectedFuelType = "95";
        private List<FuelStation> _allStations = new();
        private County _selectedCounty;
        private string _searchText;
        private bool _isRefreshing;
        private string _weatherInfo;
        private string _weatherWarning;

        public ObservableCollection<string> FuelTypes { get; } = new() { "95", "diesel", "etanol", "98" };
        public ObservableCollection<FuelStation> Stations { get; } = new();
        public ICommand LoadDataCommand { get; }

        public ObservableCollection<County> Counties { get; } = new()
        {
            new County { Name = "Södermanland",      UrlName = "sodermanlands-lan",      Latitude = 59.3667, Longitude = 16.5166, Radius = 0.8 },
            new County { Name = "Blekinge",          UrlName = "blekinge-lan",           Latitude = 56.1616, Longitude = 15.5866, Radius = 0.6 },
            new County { Name = "Dalarna",           UrlName = "dalarnas-lan",           Latitude = 60.6065, Longitude = 15.6355, Radius = 1.5 },
            new County { Name = "Gävleborg",         UrlName = "gavleborgs-lan",         Latitude = 60.6749, Longitude = 17.1413, Radius = 1.2 },
            new County { Name = "Gotland",           UrlName = "gotlands-lan",           Latitude = 57.4684, Longitude = 18.4867, Radius = 0.8 },
            new County { Name = "Halland",           UrlName = "hallands-lan",           Latitude = 56.6745, Longitude = 12.8578, Radius = 0.7 },
            new County { Name = "Jämtland",          UrlName = "jamtlands-lan",          Latitude = 63.1792, Longitude = 14.6357, Radius = 2.0 },
            new County { Name = "Jönköping",         UrlName = "jonkoping-lan",          Latitude = 57.7826, Longitude = 14.1618, Radius = 0.9 },
            new County { Name = "Kalmar",            UrlName = "kalmar-lan",             Latitude = 56.6616, Longitude = 16.3556, Radius = 1.0 },
            new County { Name = "Kronoberg",         UrlName = "kronobergs-lan",         Latitude = 56.8777, Longitude = 14.8091, Radius = 0.9 },
            new County { Name = "Norrbotten",        UrlName = "norrbottens-lan",        Latitude = 66.8309, Longitude = 20.3990, Radius = 3.5 },
            new County { Name = "Örebro",            UrlName = "orebro-lan",             Latitude = 59.2741, Longitude = 15.2066, Radius = 0.9 },
            new County { Name = "Östergötland",      UrlName = "ostergotlands-lan",      Latitude = 58.4108, Longitude = 15.6214, Radius = 0.9 },
            new County { Name = "Skåne",             UrlName = "skane-lan",              Latitude = 55.9903, Longitude = 13.5958, Radius = 0.9 },
            new County { Name = "Stockholm",         UrlName = "stockholms-lan",         Latitude = 59.3293, Longitude = 18.0686, Radius = 0.7 },
            new County { Name = "Uppsala",           UrlName = "uppsala-lan",            Latitude = 59.8586, Longitude = 17.6389, Radius = 0.9 },
            new County { Name = "Värmland",          UrlName = "varmlands-lan",          Latitude = 59.7294, Longitude = 13.2357, Radius = 1.2 },
            new County { Name = "Västerbotten",      UrlName = "vasterbottens-lan",      Latitude = 64.7507, Longitude = 18.5507, Radius = 2.5 },
            new County { Name = "Västernorrland",    UrlName = "vasternorrlands-lan",    Latitude = 62.4400, Longitude = 17.3375, Radius = 1.3 },
            new County { Name = "Västmanland",       UrlName = "vastmanlands-lan",       Latitude = 59.6099, Longitude = 16.5448, Radius = 0.8 },
            new County { Name = "Västra Götaland",   UrlName = "vastra-gotalands-lan",   Latitude = 58.2528, Longitude = 13.0596, Radius = 1.3 }
        };

        // enda konstruktorn - tar emot båda interfaces via DI
        public MainViewModel(IWeatherService weatherService, IFuelService fuelService)
        {
            _weatherService = weatherService;
            _fuelService = fuelService;
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            SelectedCounty = Counties[0]; // Sörmland som default län
        }

        public string SelectedFuelType
        {
            get => _selectedFuelType;
            set
            {
                if (SetProperty(ref _selectedFuelType, value))
                    _ = LoadDataAsync();
            }
        }

        public string WeatherInfo
        {
            get => _weatherInfo;
            set => SetProperty(ref _weatherInfo, value);
        }

        public string WeatherWarning
        {
            get => _weatherWarning;
            set => SetProperty(ref _weatherWarning, value);
        }

        public County SelectedCounty
        {
            get => _selectedCounty;
            set
            {
                if (SetProperty(ref _selectedCounty, value))
                    _ = LoadDataAsync();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public async Task LoadDataAsync()
        {
            Console.WriteLine($"[DEBUG] LoadDataAsync: SelectedCounty={SelectedCounty?.Name ?? "NULL"}");

            if (SelectedCounty == null)
            {
                return;
            }

            IsRefreshing = true;

            var weatherTask = _weatherService.GetWeatherAsync(SelectedCounty.Latitude, SelectedCounty.Longitude);
            var pricesTask = _fuelService.GetPricesAsync(SelectedCounty.UrlName, SelectedFuelType, SearchText);
            await Task.WhenAll(weatherTask, pricesTask);

            var weather = weatherTask.Result;
            var prices = pricesTask.Result;

            WeatherInfo = $"{SelectedCounty.Name} Temperatur: {weather.Temp} C";
            WeatherWarning = weather.Warning;
           

            Console.WriteLine($"[DEBUG] Antal stationer: {prices?.Count() ?? 0}");
            _allStations = prices.ToList();


            MainThread.BeginInvokeOnMainThread(() =>
            {
                Stations.Clear();
               
                foreach (var station in prices) Stations.Add(station);

                IsRefreshing = false;
            });
            
        }

        public void FilterStations()
        {
            var toShow = string.IsNullOrWhiteSpace(SearchText)
            ? _allStations
            : _allStations.Where(s => s.Name.Contains(SearchText,
            StringComparison.OrdinalIgnoreCase)).ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Stations.Clear();
                foreach (var s in toShow) Stations.Add(s);
            });
        }
    }
}