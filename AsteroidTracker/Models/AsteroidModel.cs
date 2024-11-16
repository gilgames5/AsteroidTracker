namespace AsteroidTracker.Models
{
    public class AsteroidModel
    {
        public string Name { get; set; }
        public DateTime CloseApproachDate { get; set; }
        public double DistanceKm { get; set; }
        public double SpeedKps { get; set; }
        public double DiameterMeters { get; set; }
        public bool IsHazardous { get; set; }
        public string OrbitClass { get; set; }

        public string FootballFieldsComparison
        {
            get
            {
                const double footballFieldLength = 100; // meters
                double fields = DiameterMeters / footballFieldLength;
                return $"This asteroid is approximately {fields:F1} football fields in length";
            }
        }
    }
}
