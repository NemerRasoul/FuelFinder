using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelFinder.Domain.Entities.Models;

namespace FuelFinder.Application.Interfaces
{
    public interface IFuelService
    {
        Task<List<FuelStation>> GetPricesAsync(string urlName, string fuelType, string searchCity = "");
    }
}
