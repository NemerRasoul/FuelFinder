using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.Application.Interfaces
{
    public interface IFuelStorage
    {
        Task<string> GetFallBackJsonAsync();
    }
}
