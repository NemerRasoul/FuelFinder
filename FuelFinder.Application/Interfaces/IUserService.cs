using FuelFinder.Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.Application.Interfaces
{
    public interface IUserService
    {
        User CurrentUser { get; }
        Task<bool> LoginAsync(string username, string password);
        Task<(bool Success, string Message)> RegisterAsync(string username, string password);
        Task AddFuelLogAsync(FuelLog log);
    }
}
