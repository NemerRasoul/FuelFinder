using FuelFinder.Domain.Entities;
using FuelFinder.Domain.Entities.Models;
using FuelFinder.Domain.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

        public async Task CreateUserAsync(User user)
        {
            try
            {
                await _userCollection.InsertOneAsync(user);
                Debug.WriteLine($"User saved: {user.UserName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MongoDB error: {ex.Message}");
            }
        }

        public async Task UpdateUserAsync(User user) =>
            await _userCollection.ReplaceOneAsync(u => u.UserName == user.UserName, user);
    }
}

