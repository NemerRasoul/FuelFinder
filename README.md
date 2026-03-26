# FuelFinder ⛽
A .NET MAUI application for finding and comparing fuel prices across Swedish counties, with real-time traffic and weather information and a interactive map.

## ✨ Main Features
- **Fuel Prices:** Browse live fuel prices by county and fuel type (95, diesel, ethanol, 98) with automatic fallback to local data if the API is unavailable.
- **Best Price Calculator:** Calculate the cheapest station and total cost based on how many liters you want to fill up, with an option to show live prices only.
- **Weather Info:** Fetches current temperature and road condition warnings via SMHI's open API.
- **Traffic Messages:** Displays real-time traffic messages from Sveriges Radio's API with color-coded priority levels.
- **User Accounts:** Register and log in with accounts stored in MongoDB.
- **Fuel Log:** Log your fill-ups with automatic fuel consumption calculation and a points system.
- **Map from Mapsui**: Interactive map which you can add pins to by double clicking/double tapping anywhere. Single click/tap shows coordinates.
- **Guest Mode:** Continue without an account to access fuel prices, weather and traffic. Note: Fuel logging requires a registered account.

### How the app looks
*(Click the pictures for a more zoomed in version)*

<div align="center">
  <a href="https://github.com/user-attachments/assets/b5f155f7-7734-4faa-abbc-8ac334c6ebea"><img src="https://github.com/user-attachments/assets/b5f155f7-7734-4faa-abbc-8ac334c6ebea" width="30%" /></a>
  <a href="https://github.com/user-attachments/assets/e201de04-3f8c-49c7-b687-d1b871b26da2"><img src="https://github.com/user-attachments/assets/e201de04-3f8c-49c7-b687-d1b871b26da2" width="30%" /></a>
  <a href="https://github.com/user-attachments/assets/2f52fb11-d6bf-43ba-b008-0ab07baa7e2a"><img src="https://github.com/user-attachments/assets/2f52fb11-d6bf-43ba-b008-0ab07baa7e2a" width="30%" /></a>
</div>
<br />
<div align="center">
  <a href="https://github.com/user-attachments/assets/ab571859-6c3e-486a-99a9-b55f2b74d804"><img src="https://github.com/user-attachments/assets/ab571859-6c3e-486a-99a9-b55f2b74d804" width="30%" /></a>
  <a href="https://github.com/user-attachments/assets/7ced0619-1b41-4564-9a9d-cd7aeb406f34"><img src="https://github.com/user-attachments/assets/7ced0619-1b41-4564-9a9d-cd7aeb406f34" width="30%" /></a>
</div>

  

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
- Windows PC

- ### Optional
- Android Emulator (requires Android SDK via Visual Studio)
- Physical Android device (requires USB debugging enabled)

### Installation
1. Clone the repository
2. Open `FuelFinder.sln` in Visual Studio
3. > **Note:** The steps below are only if you want to be able to login and use the logsystem and map pins correctly.
4. In MongoDB Atlas, create a database called `FuelFinderDB` or to whatever you like ( make sure if it's not called **FuelFinderDB** to rename it to whatever the name is in `FuelFinder.Infrastructure/Repositories/UserRepository.cs` →
   `var database = client.GetDatabase("your-db-name-here");` )
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

## 📱 Install on Physical Android Device
- Enable **Developer Options** on your phone — go to Settings → About Phone → tap **Build Number** 7 times
- Enable **USB Debugging** in Developer Options
- Connect your phone via USB
- In Visual Studio, select your phone from the device dropdown instead of the emulator
- Hit Run — it installs and runs directly on your phone
