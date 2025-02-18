namespace ImageDetectionApi.Models
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

    //       {
    //     "Object_id": 862,
    //     "Title": "Заплатка",
    //     "ImageName": "862_ec82425e-2761-40b2-851a-d1d46eaf5b20.jpg",
    //     "VideoName": "2_Front_09-29-2016_13.11.38_13.23.06.mp4",
    //     "Class_name": "Заплатка",
    //     "Latitude": "11111",
    //     "Longitude": "22222",
    //     "Status": "new",
    //     "CriticalLevel": 2,
    //     "DateTimeDetection": "DateTimeDetection"
    // }
    }
}
