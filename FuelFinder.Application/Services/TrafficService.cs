
using System;
using FuelFinder.Application.Interfaces;
using FuelFinder.Domain.Entities.Models;
using FuelFinder.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FuelFinder.Application.Services
{
    public class TrafficService : ITrafficService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<TrafficMessage>> GetTrafficMessagesAsync(string countyName = "") 
        {
            try 
            {
                string url = "http://api.sr.se/api/v2/traffic/messages?format=json&pagination=false";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode) 
                {
                    return new List<TrafficMessage>();
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var messagesArray = doc.RootElement.GetProperty("messages");

                var trafficList = new List<TrafficMessage>();

                foreach (var message in messagesArray.EnumerateArray()) 
                {
                    var title = message.GetProperty("title").GetString();
                    var desc = message.GetProperty("description").GetString();
                    var prio = message.GetProperty("priority").GetInt32();
                    var dateStr = message.GetProperty("createddate").GetString();

                    //Plockar ut enbart siffrorna från den konstiga datumsträngen
                    var match = Regex.Match(dateStr, @"\d+");
                    string timeFormatted = "Okänd tid";

                    if (match.Success) 
                    {
                        // omvandlar ms till hh:mm
                        long ms = long.Parse(match.Value);
                        var date = DateTimeOffset.FromUnixTimeMilliseconds(ms).ToLocalTime();
                        timeFormatted = date.ToString("HH:mm");
                    }

                    // Hämtar koordinater så vi kan filtrera på länet senare
                    var lat = message.GetProperty("latitude").GetDouble();
                    var lon = message.GetProperty("longitude").GetDouble();

                    trafficList.Add(new TrafficMessage
                    {
                        Title = title,
                        Description = desc,
                        Priority = prio,
                        FormattedTime = timeFormatted,
                        Latitude = lat,
                        Longitude = lon
                    });
                }

                return trafficList.OrderBy(msg => msg.Priority).ToList();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Trafikfel: {ex.Message}");
                return new List<TrafficMessage>();
            }
        }
    }
}
