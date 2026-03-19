using FuelFinder.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelFinder.Application.Factories
{
    public abstract class TrafficMessageCreator
    {
        public abstract TrafficMessage Create(JsonElement messageElement);
    }
}
