using FuelFinder.Application.Interfaces;
using FuelFinder.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelFinder.Application.Services
{
    public class FuelService : IFuelService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IFuelStorage _fuelStorage;

        public FuelService(IFuelStorage fuelStorage)
        {
            _fuelStorage = fuelStorage;
        }

        public async Task<List<FuelStation>> GetPricesAsync(string urlName, string fuelType, string searchCity = "")
        {
            Dictionary<string, string> liveData = new();
            Dictionary<string, string> fallbackData = new();
            List<FuelStation> finalStations = new List<FuelStation>();

            //  Hämta Fallback-data
            try
            {
                var json = await _fuelStorage.GetFallBackJsonAsync();
                fallbackData = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            
            catch {  }

            // Hämta Live-data 
            try
            {
                string url = $"https://henrikhjelm.se/api/getdata.php?lan={urlName}";

                // LÖSNINGEN: Läs in som JsonElement så kraschar inte appen på "created_at_unix"
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string, JsonElement>>(url);

                if (response != null)
                {
                    // Konvertera alla värden  till strängar
                    foreach (var kvp in response)
                    {
                        liveData[kvp.Key] = kvp.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API-fel: {ex.Message}");
            }

            //  Slå ihop alla nycklar
            var allKeys = liveData.Keys.Union(fallbackData.Keys).Distinct();

            foreach (var key in allKeys)
            {
                if (key.Contains("HenrikHjelm") || key.Contains("http") || key.Contains("created_at") || key.Contains("unix"))
                    continue;

                
                string cleanKey = key.Replace("-", "");
                string cleanUrlName = urlName.Replace("-", "");

                if (!cleanKey.StartsWith(cleanUrlName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // BRÄNSLEFILTER: Kolla att det är rätt bränsle (t.ex. __95)
                if (!key.EndsWith($"__{fuelType}", StringComparison.OrdinalIgnoreCase))
                    continue;

                // SÖKFILTER
                if (!string.IsNullOrWhiteSpace(searchCity) && !key.Contains(searchCity, StringComparison.OrdinalIgnoreCase))
                    continue;

                string price = "0";
                bool isLive = false;

                //  PRIORITERA LIVE-DATA
                if (liveData.TryGetValue(key, out string livePrice) &&
                    !string.IsNullOrWhiteSpace(livePrice) &&
                    livePrice != "0") //&&
                   // livePrice != "0.00")
                {
                    price = livePrice;
                    isLive = true;
                }
                //  FALLBACK-DATA
                else if (fallbackData.ContainsKey(key))
                {
                    price = fallbackData[key];
                    isLive = false;
                }

                
                if (!string.IsNullOrEmpty(price))
                {
                    finalStations.Add(new FuelStation
                    {
                        FullKey = key,
                        Name = CleanName(key, fuelType),
                        Price = price + " kr",
                        IsLive = isLive
                    });
                }
            }

            
            // Om API:et och Fallback-filen har lite olika stavning på länets namn 
            // grupperar vi dem på det städade namnet och prioriterar live priset
            var cleanedStations = finalStations
               .GroupBy(s => s.Name)
               .Select(group => group.OrderByDescending(s => s.IsLive).First())
               .OrderBy(s => s.Name)
               .ToList();

            return cleanedStations;
        }

        private string CleanName(string rawName, string fuelType)
        {
            int firstUnderscore = rawName.IndexOf('_');
            if (firstUnderscore == -1) return rawName;

            string clean = rawName.Substring(firstUnderscore + 1).Replace("_", " ");
            return clean.Replace(fuelType, "", StringComparison.OrdinalIgnoreCase).Trim();
        }
    }
}