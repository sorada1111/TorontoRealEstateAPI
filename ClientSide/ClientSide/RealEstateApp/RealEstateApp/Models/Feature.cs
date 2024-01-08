namespace RealEstateApp.Models
{
    public class Feature
    {
        public string? Size { get; set; }
        public string? Rooms { get; set; }
        public string? Bathrooms { get; set; }
        public bool HasParking { get; set; }
        public int WalkScore { get; set; }
        public int TransitScore { get; set; }
        public int BikeScore { get; set; }
    }
}
