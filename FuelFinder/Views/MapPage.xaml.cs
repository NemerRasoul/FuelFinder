using FuelFinder.Application.Interfaces;
using FuelFinder.Application.Services;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;


namespace FuelFinder.Views;

public partial class MapPage : ContentPage
{
    private readonly MemoryLayer _pinLayer;
    private readonly TapGestureTracker _tapTracker = new TapGestureTracker();
    private const string GlobalPinsKey = "GlobalPins";
    private string UserPinsKey => $"UserPins_{_username}"; // Unikt nyckel för varje användare

    private readonly string _username;
    private readonly bool _isAdmin;
    public MapPage(IUserService userService)
	{
		InitializeComponent();

        _username = userService.CurrentUser?.UserName ?? "guest";
        _isAdmin =  userService.CurrentUser?.IsAdmin ?? false;

        _pinLayer = new MemoryLayer { Name = "Pins" };

       
        var map = new Mapsui.Map();
        

		map.Layers.Add(OpenStreetMap.CreateTileLayer());

       
        map.Layers.Add(_pinLayer);
        
        mapControl.Map = map;

        //  Centrera karta i Eskilstuna        
        var (x, y) = SphericalMercator.FromLonLat(16.51, 59.37);
        mapControl.Map.Navigator.CenterOn(x, y);
        mapControl.Map.Navigator.ZoomToLevel(10);

        // Koordinater vid tryck på kartan (enkel klick)
        mapControl.MapPointerPressed += OnPointerPressed;

        // Lägga till pins (dubbel klick)
        mapControl.MapTapped += OnMapTapped;

        LoadPins();

    }

    private void LoadPins()
    {
        // laddar globala pins för alla användare
        var globalJson = Preferences.Get(GlobalPinsKey, null);
        if (!string.IsNullOrEmpty(globalJson))
        {

            var pins = System.Text.Json.JsonSerializer.Deserialize<List<PinData>>(globalJson) ?? new();
            foreach (var pin in pins)
                AddPinInternal(pin.Lat, pin.Lon, pin.Label, isGlobal: true);
        }

        // Ladda användarspecifika pins
        var userJson = Preferences.Get(UserPinsKey, null);
        if (!string.IsNullOrEmpty(userJson))
        {
            var userPins = System.Text.Json.JsonSerializer.Deserialize<List<PinData>>(userJson) ?? new();
            foreach (var pin in userPins)
                AddPinInternal(pin.Lat, pin.Lon, pin.Label, isGlobal: false);
        }
    }

    private void SavePins(double lat, double lon, string label, bool isGlobal)
    {
        var key = isGlobal ? GlobalPinsKey : UserPinsKey;

        var json = Preferences.Get(key, null);

        var pins = string.IsNullOrEmpty(json)
            ? new List<PinData>()
            : System.Text.Json.JsonSerializer.Deserialize<List<PinData>>(json) ?? new();

        pins.Add(new PinData { Lat = lat, Lon = lon, Label = label });

        Preferences.Set(key, System.Text.Json.JsonSerializer.Serialize(pins));
    }


    private void OnPointerPressed(object? sender, MapEventArgs e)
    {
        if (e.WorldPosition == null) 
            return;

        _tapTracker.Restart(e.ScreenPosition);

        var lonLat = SphericalMercator.ToLonLat(e.WorldPosition.X, e.WorldPosition.Y);

        MainThread.BeginInvokeOnMainThread(() =>
            CoordLabel.Text = $"Lat: {lonLat.lat:F5}  Lon: {lonLat.lon:F5}");
    }

    private async void OnMapTapped(object? sender, MapEventArgs e)
    {
       
        if (e.WorldPosition == null)
            return;


        var lonLat = SphericalMercator.ToLonLat(e.WorldPosition.X, e.WorldPosition.Y);

        _tapTracker.TapIfNeeded(e.ScreenPosition, maxTapDistance: 10, (position, gestureType) =>
        {
            if (gestureType != GestureType.DoubleTap)
                return false;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                string? name = await Shell.Current.DisplayPromptAsync(
                    "Ny station",
                    "Ange namn på stationen:",
                    placeholder: "T.ex. OKQ8 Eskilstuna");

                if (string.IsNullOrWhiteSpace(name)) return;

                // Admins kan välja global eller privat, vanliga användare får bara privat
                bool addAsGlobal = false;
                if (_isAdmin)
                {
                    addAsGlobal = await Shell.Current.DisplayAlert
                    ( 
                        "Typ av pin",
                        "Lägg till som global station (alla ser) eller bara din?",
                        "Global",
                        "Min"
                    );
                }


                AddPin(lonLat.lat, lonLat.lon, name, isGlobal: addAsGlobal);
            });

            return true;
        });
    }

    private void AddPin(double lat, double lon, string label, bool isGlobal)
    {
        AddPinInternal(lat, lon, label, isGlobal);
        SavePins(lat, lon, label, isGlobal);
    }
    private void AddPinInternal(double lat, double lon, string label, bool isGlobal)
    {
         var (x, y) = SphericalMercator.FromLonLat(lon, lat);

         var feature = new GeometryFeature
         {
             Geometry = new NetTopologySuite.Geometries.Point(x, y)
         };

         feature.Styles.Add(new SymbolStyle
         {
             SymbolScale = 0.5,

             // Blå = global (alla ser), Röd = användarens egna
             Fill = new Mapsui.Styles.Brush(isGlobal
                ? Mapsui.Styles.Color.Blue
                : Mapsui.Styles.Color.Red),
         });

         feature["Label"] = label;
         feature["IsGlobal"] = isGlobal.ToString();


         var features = _pinLayer.Features.ToList();
         features.Add(feature);
         _pinLayer.Features = features;

         MainThread.BeginInvokeOnMainThread(() =>
             mapControl.Map?.RefreshData());
    }
}

public class PinData
{
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Label { get; set; } = "";
}
