using FuelFinder.Domain.Entities.Models;
using FuelFinder.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelFinder.Infrastructure.Repositories
{
    public class GuestUserRepository : IUserRepository
    {
        public Task<User> GetUserByUsernameAsync(string username)
            => Task.FromResult<User>(null);

        public Task CreateUserAsync(User user)
            => Task.CompletedTask;

        public Task UpdateUserAsync(User user)
            => Task.CompletedTask;
    }
}
