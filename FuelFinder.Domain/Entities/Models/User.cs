using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;



namespace FuelFinder.Domain.Entities.Models
{
    public class FuelLog 
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StationName { get; set; }
        public string FuelType { get; set; } 
        public double Liters { get; set; }
        public double Odometer { get; set; }
        public double PricePerLiter { get; set; }
        public double TotalCost { get; set; }
        public DateTime Date { get; set; } 
        public double Consumption { get; set; }

        // Poängsystem (extra)
        public int PointsEarned { get; set; }
    }

    public class User 
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<FuelLog> FuelLogs { get; set; } = new List<FuelLog>();
        public int TotalPoints { get; set; }
    }
}
