using FuelFinder.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FuelFinder.Application.Factories
{
    public class TrafficMessageFactory : TrafficMessageCreator
    {
        public override TrafficMessage Create(JsonElement messageElement)
        {
            var title = messageElement.GetProperty("title").GetString();
            var desc = messageElement.GetProperty("description").GetString();
            var prio = messageElement.GetProperty("priority").GetInt32();
            var dateStr = messageElement.GetProperty("createddate").GetString();
            var lat = messageElement.GetProperty("latitude").GetDouble();
            var lon = messageElement.GetProperty("longitude").GetDouble();

            // Plockar ut enbart siffrorna från den konstiga datumsträngen
            var match = Regex.Match(dateStr, @"\d+");
            string timeFormatted = "Okänd tid";

            if (match.Success)
            {
                long ms = long.Parse(match.Value);
                var date = DateTimeOffset.FromUnixTimeMilliseconds(ms).ToLocalTime();
                timeFormatted = date.ToString("HH:mm");
            }

            return new TrafficMessage
            {
                Title = title,
                Description = desc,
                Priority = prio,
                FormattedTime = timeFormatted,
                Latitude = lat,
                Longitude = lon
            };
        }
    }
}
