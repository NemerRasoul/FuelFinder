using FuelFinder.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.Infrastructure.Repositories
{
    public class FuelRepository : IFuelStorage
    {
        private readonly Func<string, Task<Stream>> _fileOpener;

        public FuelRepository(Func<string, Task<Stream>> fileOpener)
        {
            _fileOpener = fileOpener;
        }

        public async Task<string> GetFallBackJsonAsync()
        {
            using var stream = await _fileOpener("fuel_fallback.json");
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}