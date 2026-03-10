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
    private const string PinsKey = "Pins";
    public MapPage()
	{
		InitializeComponent();

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
        var json = Preferences.Get(PinsKey, null);
        if (string.IsNullOrEmpty(json)) return;

        var pins = System.Text.Json.JsonSerializer.Deserialize<List<PinData>>(json) ?? new();
        foreach (var pin in pins)
            AddPinInternal(pin.Lat, pin.Lon, pin.Label);
    }

    private void SavePins()
    {
        var pinDataList = new List<PinData>();

        foreach (var feature in _pinLayer.Features)
        {
            if (feature is GeometryFeature gf &&
            gf.Geometry is NetTopologySuite.Geometries.Point point)
            {
                var lonLat = SphericalMercator.ToLonLat(point.X, point.Y);
                pinDataList.Add(new PinData
                {
                    Lat = lonLat.lat,
                    Lon = lonLat.lon,
                    Label = feature["Label"]?.ToString() ?? ""
                });
            }
        }

        var json = System.Text.Json.JsonSerializer.Serialize(pinDataList);
        Preferences.Set(PinsKey, json);
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

                AddPin(lonLat.lat, lonLat.lon, name);
            });

            return true;
        });
    }

    private void AddPin(double lat, double lon, string label)
    {
        AddPinInternal(lat, lon, label);
        SavePins();
    }
    private void AddPinInternal(double lat, double lon, string label)
    {
         var (x, y) = SphericalMercator.FromLonLat(lon, lat);

         var feature = new GeometryFeature
         {
             Geometry = new NetTopologySuite.Geometries.Point(x, y)
         };

         feature.Styles.Add(new SymbolStyle
         {
             SymbolScale = 0.5,
             Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.FromString("Red")),
         });

         feature["Label"] = label;

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
