namespace ImageDetectionMvc.Models
{
    public class Detection
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public string VideoName { get; set; }
        public string ClassName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public int CriticalLevel { get; set; }
        public DateTime DateTimeDetection { get; set; }
    }
}
