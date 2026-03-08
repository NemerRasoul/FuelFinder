using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.Domain.Entities.Models
{
    public class FuelStation
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string FullKey { get; set; }
        public bool IsLive { get; set; } = true;
        public string StatusText => IsLive ? "" : "(EJ LIVE PRIS)";
    }
}
