using FuelFinder.Domain.Entities.Models;
using FuelFinder.Application.Interfaces;  
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FuelFinder.ViewModels
{
    public class TrafficViewModel : BaseViewModel
    {
        private readonly ITrafficService _trafficService;  // interface istället för klass
        private List<TrafficMessage> _allMessages = new List<TrafficMessage>();
        private bool _isRefreshing;
        private string _trafficSearchText = "";

        public ObservableCollection<TrafficMessage> TrafficMessages { get; } = new();

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string TrafficSearchText
        {
            get => _trafficSearchText;
            set
            {
                if (SetProperty(ref _trafficSearchText, value))
                    FilterTraffic();
            }
        }

        public ICommand LoadTrafficCommand { get; }

        // Ta emot ITrafficService via DI istället för att skapa den direkt
        public TrafficViewModel(ITrafficService trafficService)
        {
            _trafficService = trafficService;
            LoadTrafficCommand = new Command(async () => await LoadTrafficAsync());
            _ = LoadTrafficAsync();
        }

        private async Task LoadTrafficAsync()
        {
            IsRefreshing = true;
            var messages = await _trafficService.GetTrafficMessagesAsync();
            _allMessages = messages.ToList();
            IsRefreshing = false;
            FilterTraffic();
        }

        private void FilterTraffic()
        {
            if (string.IsNullOrWhiteSpace(TrafficSearchText))
            {
                TrafficMessages.Clear();
                foreach (var m in _allMessages) TrafficMessages.Add(m);
                return;
            }

            var filtered = _allMessages.Where(msg =>
                msg.Title.Contains(TrafficSearchText, StringComparison.OrdinalIgnoreCase) ||
                msg.Description.Contains(TrafficSearchText, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            TrafficMessages.Clear();
            foreach (var m in filtered) TrafficMessages.Add(m);
        }
    }
}