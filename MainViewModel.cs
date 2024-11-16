using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace AsteroidTracker
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;
        private DateTime _selectedDate;

        public ObservableCollection<AsteroidModel> Asteroids { get; } = new();

        public string ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
                OnPropertyChanged(nameof(ApiKey));
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public ICommand SaveApiKeyCommand { get; }
        public ICommand RefreshDataCommand { get; }

        public MainViewModel()
        {
            _httpClient = new HttpClient();
            _selectedDate = DateTime.Today;

            SaveApiKeyCommand = new RelayCommand(SaveApiKey);
            RefreshDataCommand = new RelayCommand(async () => await RefreshAsteroidData());

            LoadSavedApiKey();
        }

        private void SaveApiKey()
        {
            Properties.Settings.Default.ApiKey = ApiKey;
            Properties.Settings.Default.Save();
        }

        private void LoadSavedApiKey()
        {
            ApiKey = Properties.Settings.Default.ApiKey;
        }

        private async Task RefreshAsteroidData()
        {
            try
            {
                var startDate = SelectedDate.ToString("yyyy-MM-dd");
                var endDate = SelectedDate.AddDays(7).ToString("yyyy-MM-dd");

                var response = await _httpClient.GetStringAsync(
                    $"https://api.nasa.gov/neo/rest/v1/feed?start_date={startDate}&end_date={endDate}&api_key={ApiKey}");

                var data = JObject.Parse(response);

                Asteroids.Clear();

                var nearEarthObjects = data["near_earth_objects"];
                foreach (var dateEntry in nearEarthObjects.Children<JProperty>())
                {
                    foreach (var asteroid in dateEntry.Value)
                    {
                        Asteroids.Add(new AsteroidModel
                        {
                            Name = (string)asteroid["name"],
                            CloseApproachDate = DateTime.Parse((string)asteroid["close_approach_data"][0]["close_approach_date"]),
                            DistanceKm = double.Parse((string)asteroid["close_approach_data"][0]["miss_distance"]["kilometers"]),
                            SpeedKps = double.Parse((string)asteroid["close_approach_data"][0]["relative_velocity"]["kilometers_per_second"]),
                            DiameterMeters = ((double)asteroid["estimated_diameter"]["meters"]["estimated_diameter_min"] +
                                            (double)asteroid["estimated_diameter"]["meters"]["estimated_diameter_max"]) / 2,
                            IsHazardous = (bool)asteroid["is_potentially_hazardous_asteroid"],
                            OrbitClass = (string)asteroid["orbital_data"]["orbit_class"]["orbit_class_type"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error loading asteroid data: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
