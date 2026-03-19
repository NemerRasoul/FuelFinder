# FuelFinder ⛽
A .NET MAUI mobile application for finding and comparing fuel prices across Swedish counties, with real-time traffic and weather information.

## ✨ Main Features
- **Fuel Prices:** Browse live fuel prices by county and fuel type (95, diesel, ethanol, 98) with automatic fallback to local data if the API is unavailable.
- **Best Price Calculator:** Calculate the cheapest station and total cost based on how many liters you want to fill up, with an option to show live prices only.
- **Weather Info:** Fetches current temperature and road condition warnings via SMHI's open API.
- **Traffic Messages:** Displays real-time traffic messages from Sveriges Radio's API with color-coded priority levels.
- **User Accounts:** Register and log in with accounts stored in MongoDB.
- **Fuel Log:** Log your fill-ups with automatic fuel consumption calculation and a points system.
- **Guest Mode:** Continue without an account to access fuel prices, weather and traffic. Note: Fuel logging requires a registered account.

## 🛠 Tech Stack
- **Framework:** .NET MAUI (Android & Windows)
- **Language:** C#
- **Database:** MongoDB Atlas
- **APIs:** SMHI (weather), Sveriges Radio (traffic), henrikhjelm.se (fuel prices)
- **Architecture:** Onion Architecture with MVVM

## 🚀 Getting Started

### Requirements
- Visual Studio 2022 with MAUI workload installed
- .NET 9 SDK
- Android Emulator or physical device

### Installation
1. Clone the repository
2. Open `FuelFinder.sln` in Visual Studio
3. >**Note:** The steps below are only if you want to be able to login and use the logsystem and map pins correctly.
4. In MongoDB Atlas, create a database called `FuelFinderDB` or to whatever you like ( make sure if it's not called **FuelFinderDB
    to rename it to whatever the name is in FuelFinder.Infrastructure/Repositories/UserRepository.cs ->  var database = client.GetDatabase("your-db-name-here");** )  
5. Add your MongoDB connection string via User Secrets (Windows):
```json
{
  "ConnectionStrings": {
    "MongoDB": "your-connection-string-here"
  }
}
```
6. For Android: add your MongoDB connection string to `Constant.cs`:
```csharp
public const string MongoDBConnectionString = "your-connection-string-here";
```
7. Run the project on Android or Windows

> **Note:** A MongoDB Atlas connection string is required for user accounts and fuel logging. Without it, you can still use the app in guest mode to browse fuel prices, weather and traffic. User Secrets only works on Windows — Android requires the connection string in `Constant.cs`.
