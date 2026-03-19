using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.ViewModels
{
    public class CalculatorViewModel : BaseViewModel
    {
        public MainViewModel MainViewModel => _mainViewModel;
        private readonly MainViewModel _mainViewModel;

        // Input från användaren
        private string _litersToFill;
        public string LitersToFill
        {
            get => _litersToFill;
            set => SetProperty(ref _litersToFill, value);
        }

        private bool _onlyLivePrices;
        public bool OnlyLivePrices
        {
            get => _onlyLivePrices;
            set => SetProperty(ref _onlyLivePrices, value);
        }

        private string _bestStationResult;
        public string BestStationResult
        {
            get => _bestStationResult;
            set => SetProperty(ref _bestStationResult, value);
        }

        public Command FindBestPriceCommand { get; }

        public CalculatorViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            FindBestPriceCommand = new Command(CalculateBestDeal);
        }

        private void CalculateBestDeal()
        {
            try
            {
                var county = _mainViewModel.SelectedCounty;
                var fuel = _mainViewModel.SelectedFuelType;

                if (_mainViewModel.Stations == null || !_mainViewModel.Stations.Any())
                {
                    BestStationResult = "Hittade inga stationer. Gå till bränslefliken och ladda in alla priser först!";
                    return;
                }

                var stations = OnlyLivePrices
                    ? _mainViewModel.Stations.Where(s => s.IsLive).ToList()
                    : _mainViewModel.Stations.ToList();

                if (!stations.Any())
                {
                    BestStationResult = "Inga live-priser hittades för detta län.";
                    return;
                }

                if (double.TryParse(LitersToFill?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double liters))
                {

                    var cheapestStation = stations
                        .OrderBy(s => ParsePrice(s.Price))
                        .FirstOrDefault();

                    if (cheapestStation != null)
                    {
                        double pricePerLiter = ParsePrice(cheapestStation.Price);
                        double totalCost = liters * pricePerLiter;


                        BestStationResult = $"I {county.Name} är {cheapestStation.Name} billigast för {fuel}.\n\n" +
                                     $"Pris: {cheapestStation.Price}\n" +
                                     $"Total kostnad ({liters}L): {totalCost:F2} kr";

                    }

                }
                else
                {
                    BestStationResult = "Vänligen skriv in hur många liter du vill tanka (t.ex. 50).";
                }
            }
            catch (Exception ex)
            {
                BestStationResult = "Ett fel uppstod vid beräkningen.";
                Debug.WriteLine($"Kalkylatorfel: {ex.Message}");
            }
        }

        // Hjälpmetod för att säkert omvandla "17.29 kr" till siffran 17.29
        private double ParsePrice(string priceString)
        {
            if (string.IsNullOrWhiteSpace(priceString)) return double.MaxValue;

            // Ta bort " kr", ersätt komma med punkt och tvätta bort mellanslag
            string cleanPrice = priceString.Replace(" kr", "").Replace(",", ".").Trim();

            if (double.TryParse(cleanPrice, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return double.MaxValue;
        }
    }
       
}

