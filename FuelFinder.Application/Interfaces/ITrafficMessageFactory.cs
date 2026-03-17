using FuelFinder.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelFinder.Application.Interfaces
{
    public interface ITrafficMessageFactory
    {
        TrafficMessage Create(JsonElement messageElement);
    }
}
