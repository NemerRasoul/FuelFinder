using FuelFinder.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelFinder.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public WeatherService() 
        {
            // SMHI kräver en "user agent" för att tillåta anropet
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "FuelFinderApp/1.0");
        }

        public async Task<(double Temp, string Warning)> GetWeatherAsync(double lat, double lon) 
        {
            try 
            {
                string lonStr = lon.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
                string latStr = lat.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);

                string url = $"https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/{lonStr}/lat/{latStr}/data.json";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "FuelFinderApp/1.0");


                //var response = await _httpClient.GetAsync(url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode) 
                {
                    return (0, $"Väder server svarar inte. Fel: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                var firstForecast = doc.RootElement.GetProperty("timeSeries")[0];

                var tempElement = firstForecast.GetProperty("parameters")
                    .EnumerateArray()
                    .FirstOrDefault(p => p.GetProperty("name")
                    .GetString() == "t");

                if (tempElement.ValueKind == JsonValueKind.Undefined) 
                {
                    return (0, "Temperaturdata saknas i svaret");
                }

                double temp = tempElement.GetProperty("values")[0].GetDouble();
               

                //algoritm för halkvarning
                string warning = temp <= 0 ? "Varning: Risk för halka!" : "Väglag: OK";
                return (temp, warning);
            }
            catch (Exception ex) 
            {
                //return (0, $"Väderdata ej tillgänglig: {ex.Message}");
                return (0, $"Väderfel: {ex.Message}");
               
            }
        }
    }
}
