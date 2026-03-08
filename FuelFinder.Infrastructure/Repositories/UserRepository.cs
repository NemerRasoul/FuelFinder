using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelFinder.Domain.Entities.Models;
using FuelFinder.Domain.Entities;
using FuelFinder.Domain.Interfaces;


namespace FuelFinder.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(string connectionString) 
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("FuelFinderDB");  
            _userCollection = database.GetCollection<User>("Users");
        }

        public async Task<User> GetUserByUsernameAsync(string username) =>
            await _userCollection.Find(u => u.UserName.ToLower() == username.ToLower()).FirstOrDefaultAsync();

        public async Task CreateUserAsync(User user) =>
            await _userCollection.InsertOneAsync(user);

        public async Task UpdateUserAsync(User user) =>
            await _userCollection.ReplaceOneAsync(u => u.UserName == user.UserName, user);
    }
}

