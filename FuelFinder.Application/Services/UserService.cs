using FuelFinder.Application.Interfaces;
using FuelFinder.Domain.Entities.Models;
using FuelFinder.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelFinder.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public User CurrentUser {get; private set; } // Håller koll på vem som är inloggad

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> LoginAsync(string username, string password) 
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

           var user = await _userRepository.GetUserByUsernameAsync(username);
          

            if (user != null && user.Password == password) 
            {
                CurrentUser = user; 
                return true;
            }
            return false;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password) 
        {
            if (string.IsNullOrWhiteSpace(username) || password.Length < 4)
                return (false, "Användarnamn krävs och lösenordet måste vara minst 4 tecken.");

            
            var existingUser = await _userRepository.GetUserByUsernameAsync(username);


            if (existingUser != null)
            {
                return (false, "Användarnamnet är tyvärr redan upptaget.");
            }

            
            var newUser = new User
            {
                UserName = username,
                Password = password,
                TotalPoints = 0,
                FuelLogs = new List<FuelLog>()
            };

            await _userRepository.CreateUserAsync(newUser);

            CurrentUser = newUser;
            Debug.WriteLine($"Nytt konto skapat för: {newUser.UserName}");

            return (true, "Konto skapat!");
        }

        public async Task AddFuelLogAsync(FuelLog log) 
        {
            if (CurrentUser == null)
            {
                return;
            }

            double consumption = 0;

            var lastLog = CurrentUser.FuelLogs.OrderByDescending(log => log.Odometer).FirstOrDefault();

            if (lastLog != null && log.Odometer > lastLog.Odometer)
            {

                double distanceKm = log.Odometer - lastLog.Odometer;

                // svenska mil 
                double distanceMiles = distanceKm / 10;

                if (distanceMiles > 0)
                {
                    consumption = Math.Round(log.Liters / distanceMiles, 2);
                }
            }

            log.Consumption = consumption;
            log.PointsEarned = 10;
            log.Date = DateTime.Now;

            CurrentUser.FuelLogs.Add(log);
            CurrentUser.TotalPoints += log.PointsEarned;

            await _userRepository.UpdateUserAsync(CurrentUser);



        }
    }
    
}
