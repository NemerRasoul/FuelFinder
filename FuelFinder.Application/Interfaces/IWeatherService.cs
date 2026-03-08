using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.Application.Interfaces
{
    public interface IWeatherService
    {
        Task<(double Temp, string Warning)> GetWeatherAsync(double lat, double lon);
    }
}
